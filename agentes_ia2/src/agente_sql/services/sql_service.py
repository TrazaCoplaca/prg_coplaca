import logging
import pyodbc

from agente_sql.agent.agente_sql import generar_sql, interpretar_resultados, limpiar_sql
from agente_sql.tools.tools_sql import ejecutar_sql
from agente_sql.infrastructure.sqlserver import get_schema, schema_to_text
from agente_sql.infrastructure.config import get_settings
from agente_sql.schemas.models import SQLResponse, SchemaResponse
from agente_sql.security import es_operacion_escritura
from agente_sql.allowed_tables import ALLOWED_TABLES
from openai import OpenAI
from agente_sql.services.quick_queries import (
    sql_buscar_caja,
    sql_lote_de_caja,
    sql_albaran_de_caja,
    sql_cajas_de_albaran,
    sql_productos_en_albaran,
    sql_cajas_de_socio,
    sql_info_palet,
)
from agente_sql.tools.tools_sql import ejecutar_sql
from agente_sql.schemas.models import SQLResponse


def procesar_consulta_rapida(tipo: str, valor: int) -> SQLResponse:
    if tipo == "buscar_caja":
        sql = sql_buscar_caja(valor)
    elif tipo == "lote_de_caja":
        sql = sql_lote_de_caja(valor)
    elif tipo == "albaran_de_caja":
        sql = sql_albaran_de_caja(valor)
    elif tipo == "cajas_de_albaran":
        sql = sql_cajas_de_albaran(valor)
    elif tipo == "productos_en_albaran":
        sql = sql_productos_en_albaran(valor)
    elif tipo == "cajas_de_socio":
        sql = sql_cajas_de_socio(valor)
    elif tipo == "info_palet":
        sql = sql_info_palet(valor)
    else:
        raise ValueError(f"Tipo de consulta rápida no soportado: {tipo}")

    resultados = ejecutar_sql(sql)
    filas = len(resultados)

    return SQLResponse(
        pregunta=f"quick:{tipo}:{valor}",
        sql=sql,
        resultados=resultados,
        filas=filas,
        mensaje=f"Consulta rápida ejecutada correctamente. Filas devueltas: {filas}.",
    )

logger = logging.getLogger(__name__)
settings = get_settings()
client = OpenAI(api_key=settings.openai_api_key)


def _es_error_tabla_no_permitida(error: ValueError) -> bool:
    """Detecta si el ValueError proviene de una tabla no permitida."""
    return "no está permitida o no existe" in str(error)


def corregir_sql(sql_fallido: str, error_msg: str) -> str:
    """
    Capa 2: pide al LLM que corrija un SQL que falló por usar
    una tabla no permitida. Devuelve el SQL corregido.
    """
    tablas_permitidas = ", ".join(sorted(ALLOWED_TABLES))

    prompt = f"""El siguiente SQL falló con este error:
ERROR: {error_msg}

SQL fallido:
{sql_fallido}

INSTRUCCIONES DE CORRECCIÓN:
- El error indica que se usó una tabla que NO está permitida.
- Reescribe el SQL usando ÚNICAMENTE las siguientes tablas permitidas:
  {tablas_permitidas}
- Si necesitas datos de una tabla no permitida, elimina ese JOIN y usa
  solo los campos disponibles en las tablas permitidas.
- Devuelve solo el SQL corregido, sin explicaciones, sin markdown.
"""

    response = client.responses.create(
        model="gpt-5.2",
        input=prompt
    )

    sql_corregido = response.output[0].content[0].text
    sql_corregido = limpiar_sql(sql_corregido)
    logger.info("SQL corregido (reintento): %s", sql_corregido)
    return sql_corregido


def procesar_pregunta(pregunta: str) -> SQLResponse:

    if es_operacion_escritura(pregunta):
        raise ValueError(
            "Operación no permitida: este agente solo permite consultas de lectura (SELECT)."
        )

    # --- Generar SQL inicial ---
    try:
        sql = generar_sql(pregunta)
    except Exception as e:
        raise RuntimeError(f"Error al generar SQL: {e}") from e

    # --- Ejecutar SQL (con reintento de corrección automática) ---
    try:
        resultados = ejecutar_sql(sql)

    except ValueError as e:
        if _es_error_tabla_no_permitida(e):
            # Capa 2: pedir al LLM que corrija el SQL
            logger.warning("Tabla no permitida detectada. Reintentando con corrección LLM. Error: %s", e)
            try:
                sql = corregir_sql(sql, str(e))
                resultados = ejecutar_sql(sql)
            except (ValueError, pyodbc.Error) as e2:
                raise ValueError(str(e2)) from e2
        else:
            raise ValueError(str(e)) from e

    except pyodbc.Error as e:
        raise RuntimeError(f"Error de base de datos: {e}") from e

    filas = len(resultados)
    mensaje = interpretar_resultados(pregunta, sql, resultados, filas)

    return SQLResponse(
        pregunta=pregunta,
        sql=sql,
        resultados=resultados,
        filas=filas,
        mensaje=mensaje,
    )


def obtener_schema() -> SchemaResponse:
    """Retorna el schema de la DB como modelo Pydantic."""
    schema = get_schema()
    return SchemaResponse(
        tablas=schema,
        total_tablas=len(schema),
    )