from openai import OpenAI
from tenacity import retry, stop_after_attempt, wait_exponential, before_sleep_log
import logging

from agente_sql.infrastructure.config import get_settings
from agente_sql.infrastructure.sqlserver import get_schema, schema_to_text
from agente_sql.infrastructure.logger import get_logger
from agente_sql.context.business_context import BUSINESS_CONTEXT
from agente_sql.context.table_context import TABLE_CONTEXT
from agente_sql.allowed_tables import ALLOWED_TABLES

settings = get_settings()

client = OpenAI(api_key=settings.openai_api_key)

logger = get_logger(__name__)

schema = get_schema()
# Filtrar: el LLM solo ve las tablas de ALLOWED_TABLES (insensible a mayúsculas)
_allowed_lower = {t.lower() for t in ALLOWED_TABLES}
schema_filtrado = {t: cols for t, cols in schema.items() if t.lower() in _allowed_lower}
schema_text = schema_to_text(schema_filtrado)
logger.debug("Schema filtrado: %d tablas visibles al LLM de %d totales", len(schema_filtrado), len(schema))


def limpiar_sql(sql: str) -> str:
    sql = sql.strip()
    sql = sql.replace("```sql", "")
    sql = sql.replace("```SQL", "")
    sql = sql.replace("```", "")

    if sql.endswith(";"):
        sql = sql[:-1]

    return sql.strip()

def es_consulta_agregada(sql: str) -> bool:
    sql_lower = sql.lower()
    return any(func in sql_lower for func in ["count(", "sum(", "avg(", "min(", "max("])

def tiene_top(sql: str) -> bool:
    return " top " in f" {sql.lower()} "


@retry(
    stop=stop_after_attempt(3),
    wait=wait_exponential(multiplier=1, min=1, max=10),
    before_sleep=before_sleep_log(logger, logging.WARNING),
    reraise=True,
)
def generar_sql(pregunta: str) -> str:
    """Genera SQL a partir de lenguaje natural. Reintenta hasta 3 veces."""

    logger.debug("Generando SQL para: %s", pregunta)

    tablas_permitidas = ", ".join(sorted(ALLOWED_TABLES))

    prompt = f"""
Eres un experto en SQL Server.

Este es el esquema de la base de datos:

{schema_text}

Contexto de negocio:
{BUSINESS_CONTEXT}

Contexto de tablas:
{TABLE_CONTEXT}

REGLAS OBLIGATORIAS

- Genera únicamente una consulta SQL válida para SQL Server.
- Usa solo SELECT.
- No uses INSERT, UPDATE, DELETE, DROP, ALTER, TRUNCATE ni EXEC.
- No expliques nada.
- No uses markdown.
- Devuelve únicamente la sentencia SQL final.

TABLA PRINCIPAL

La tabla principal es:

TrazaCaja

Contiene la trazabilidad de cada caja producida.

COLUMNAS IMPORTANTES DE TrazaCaja

CodigoCaja (bigint)
FechaConf (date)
HoraConf (varchar)
Socio (int)
Finca (int)
Confeccion (int)
CajasPalet (int)
KilosNetos (real)
Envase (int)
Albaran (int)
Cooperativa (nvarchar)
Traspasado (bit)
Linea (int)
Variedad (int)
npalet (int)
PesoEnvase (real)
Categoria (int)
NumeroCajaEan (nvarchar)
PesoPalet (real)
CodProducto (int)
Parcela (smallint)
Lote (nvarchar)

CAMPOS DE PALET — DISTINCIÓN CRÍTICA

Cada fila de TrazaCaja representa UNA caja individual.

npalet (int)
  Número de orden del palet dentro de un albarán o producción concretos.
  Es LOCAL: se repite entre distintos albaranes, fechas, socios y cooperativas.

  NUNCA usar npalet solo sin contexto adicional (Albaran, Socio, FechaConf).
  Si se usa solo, un COUNT(*) devolverá cientos de miles de filas incorrectas.

  Uso correcto:
  "cajas del palet 3 del albarán 11477334"
  → WHERE tc.npalet = 3 AND tc.Albaran = 11477334

Linea (int), Parcela (smallint), Finca (int)
  Tampoco son identificadores globales. Igual que npalet:
  solo son significativos dentro de un contexto (Socio, Albaran, FechaConf...).

CajasPalet (int)
  Número de cajas asignadas al palet según la confección.
  Es el mismo valor para todas las cajas del mismo palet.
  Valores típicos: 36, 52, 60.

  "cajas de 36 por palet"
  → WHERE tc.CajasPalet = 36

REGLA CRÍTICA — CONSULTAS DE CANTIDAD DE CAJAS POR PALET

  Si el usuario pregunta:
  "¿cuántas cajas tiene/lleva el palet N?"
  "¿cuántas cajas hay en el palet N?"

  La respuesta correcta es el valor de CajasPalet, NO un COUNT(*).
  Usar CajasPalet directamente:

  SELECT TOP 1 tc.CajasPalet
  FROM TrazaCaja tc
  WHERE tc.npalet = 3
    AND tc.Albaran = <albaran>   -- OBLIGATORIO si se conoce
    AND tc.Socio   = <socio>     -- OBLIGATORIO si se conoce

  Si el usuario pregunta por un npalet sin aportar contexto suficiente
  (por ejemplo sin Albaran, Socio, Lote o FechaConf),
  NO usar COUNT(*) sobre todas las filas con ese npalet.
  En ese caso:
  - devolver información identificativa del contexto
  - o usar CajasPalet si la pregunta es cuántas cajas lleva ese palet


REGLAS DE TIPOS

- Columnas numéricas (int, bigint, smallint, real) deben compararse usando "=".
- No usar LIKE para columnas numéricas.
- LIKE solo debe usarse para columnas de texto (varchar, nvarchar).
- Evitar conversiones innecesarias como CONVERT(nvarchar(...)) si las columnas ya son numéricas.

REGLAS DE BÚSQUEDA

Si el usuario proporciona:

Código de caja  
→ usar igualdad exacta

Ejemplo:

WHERE tc.CodigoCaja = 2851701

No usar LIKE para CodigoCaja.

RELACIONES ENTRE TABLAS

Join con Socio:

Socio.Codigo = TrazaCaja.Socio

Join con Finca:

La finca NO es un identificador global.

El código de finca se reutiliza entre distintos socios.

La relación correcta es:

Finca.CodigoSocio = TrazaCaja.Socio
AND Finca.Codigo = TrazaCaja.Finca

Nunca hacer join solo por:

Finca.Codigo = TrazaCaja.Finca

Si no se necesita información de finca, no hacer join con Finca.

REGLAS DE GENERACIÓN SQL

- Usa nombres reales de columnas.
- Usa alias cortos para las tablas (ej: tc, s, f).
- Ordena resultados por fecha y hora cuando tenga sentido.

Ejemplo:

ORDER BY tc.FechaConf DESC, tc.HoraConf DESC

PREGUNTA DEL USUARIO

{pregunta}
"""

    response = client.responses.create(
        model="gpt-5.2",
        input=prompt
    )

    sql = response.output[0].content[0].text
    sql = limpiar_sql(sql)

    logger.info("SQL generado: %s", sql)
    return sql


@retry(
    stop=stop_after_attempt(3),
    wait=wait_exponential(multiplier=1, min=1, max=10),
    before_sleep=before_sleep_log(logger, logging.WARNING),
    reraise=True,
)
def interpretar_resultados(
    pregunta: str,
    sql: str,
    resultados: list[dict],
    filas: int,
) -> str:
    """Convierte los resultados de la consulta SQL en lenguaje natural."""

    if not resultados:
        logger.info("Sin resultados para: %s", pregunta)
        return "No se encontraron datos que coincidan con tu consulta."

    logger.debug("Interpretando %d filas para: %s", filas, pregunta)

    agregada = es_consulta_agregada(sql)
    limitada = tiene_top(sql)

    prompt = f"""
Eres un asistente que responde preguntas sobre datos de una base de datos.

El usuario preguntó: "{pregunta}"

Se ejecutó esta consulta SQL:
{sql}

La consulta devolvió {filas} filas en esta ejecución.

Tipo de consulta:
- Consulta agregada: {"sí" if agregada else "no"}
- Consulta limitada con TOP: {"sí" if limitada else "no"}

Los resultados obtenidos son:
{resultados}

Reglas de respuesta:
- Responde en español claro y natural.
- No muestres el SQL.
- No muestres los datos en bruto salvo que sea imprescindible.
- Si la consulta es de listado, resume sin inventar cantidades totales globales.
- Si la consulta está limitada con TOP, indica que se muestran resultados parciales devueltos por la consulta.
- Solo menciones cantidades exactas totales si la consulta es claramente agregada (por ejemplo COUNT, SUM, AVG, MIN, MAX).
- Si no es una consulta agregada, no afirmes frases como "hay X cajas en total" salvo que ese total esté realmente calculado por la SQL.
- Si hay varios registros, puedes resumir patrones útiles, pero sin deducir totales que no aparecen en la consulta.
"""

    response = client.responses.create(
        model="gpt-5.2",
        input=prompt
    )

    respuesta = response.output[0].content[0].text.strip()
    logger.debug("Respuesta generada (%d chars)", len(respuesta))
    return respuesta
