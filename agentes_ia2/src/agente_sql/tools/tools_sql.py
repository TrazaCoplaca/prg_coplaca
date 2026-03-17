import re
import logging

from agente_sql.infrastructure.sqlserver import get_connection
from agente_sql.security import contiene_sql_peligroso
from agente_sql.allowed_tables import ALLOWED_TABLES

logger = logging.getLogger(__name__)


def validar_sql(query: str) -> str:
    if not query or not query.strip():
        raise ValueError("La consulta está vacía.")

    q = query.strip()
    q_lower = q.lower()

    if ";" in q:
        raise ValueError("No se permiten múltiples sentencias ni ';'.")

    if not q_lower.startswith("select"):
        raise ValueError("Solo se permiten consultas SELECT.")

    keyword = contiene_sql_peligroso(q_lower)
    if keyword:
        raise ValueError(f"Consulta bloqueada por palabra no permitida: {keyword}")

    tablas = extraer_tablas(q)
    for tabla in tablas:
        tabla_normalizada = normalizar_nombre_tabla(tabla)

        if tabla_normalizada not in ALLOWED_TABLES:
            raise ValueError(f"La tabla '{tabla}' no está permitida o no existe.")

    # Aviso si se usa igualdad exacta en campos de texto típicos
    if re.search(r"\b(nombre|descripcion|descripción)\s*=\s*'.+?'", q_lower):
        logger.warning(
            "Comparación exacta detectada en campo de texto. "
            "Quizá debería usar LIKE: %s",
            q
        )

    q = asegurar_top(q)

    return q


def normalizar_nombre_tabla(tabla: str) -> str:
    """
    Convierte:
    - dbo.Categoria -> Categoria
    - [dbo].[Categoria] -> Categoria
    - Categoria -> Categoria
    """
    tabla = tabla.replace("[", "").replace("]", "")
    return tabla.split(".")[-1]


def extraer_tablas(query: str) -> list[str]:
    """
    Extrae nombres de tabla después de FROM y JOIN.
    Soporta:
    - Categoria
    - dbo.Categoria
    - [dbo].[Categoria]
    """
    tablas = []

    patrones = [
        r"\bfrom\s+([a-zA-Z_][a-zA-Z0-9_\.]*|\[[^\]]+\]\.\[[^\]]+\])",
        r"\bjoin\s+([a-zA-Z_][a-zA-Z0-9_\.]*|\[[^\]]+\]\.\[[^\]]+\])",
    ]

    for patron in patrones:
        encontrados = re.findall(patron, query, flags=re.IGNORECASE)
        tablas.extend(encontrados)

    return list(set(tablas))


def asegurar_top(query: str, top: int = 50) -> str:
    """
    Si la consulta no lleva TOP, añade TOP (50).
    Respeta el orden correcto cuando hay DISTINCT:
    SELECT DISTINCT TOP (50) ...
    """
    if re.search(r"\btop\s*\(", query, flags=re.IGNORECASE) or re.search(r"\btop\s+\d+", query, flags=re.IGNORECASE):
        return query

    # Caso: SELECT DISTINCT ...
    if re.match(r"^\s*select\s+distinct\b", query, flags=re.IGNORECASE):
        return re.sub(
            r"^\s*select\s+distinct\b",
            f"SELECT DISTINCT TOP ({top})",
            query,
            count=1,
            flags=re.IGNORECASE,
        )

    # Caso normal: SELECT ...
    return re.sub(
        r"^\s*select\b",
        f"SELECT TOP ({top})",
        query,
        count=1,
        flags=re.IGNORECASE,
    )


def ejecutar_sql(query: str) -> list[dict]:
    query_segura = validar_sql(query)

    conn = get_connection()
    cursor = conn.cursor()

    try:
        cursor.execute(query_segura)
        cols = [col[0] for col in cursor.description]
        rows = cursor.fetchall()
        resultados = [dict(zip(cols, row)) for row in rows]
        return resultados
    finally:
        conn.close()