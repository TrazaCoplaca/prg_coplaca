import os
from functools import lru_cache

import pyodbc
from dotenv import load_dotenv

from agente_sql.infrastructure.logger import get_logger

load_dotenv()

logger = get_logger(__name__)


def get_connection() -> pyodbc.Connection:

    server = os.getenv("DB_SERVER")
    database = os.getenv("DB_NAME")
    driver = os.getenv("DB_DRIVER")

    connection_string = (
        f"DRIVER={{{driver}}};"
        f"SERVER={server};"
        f"DATABASE={database};"
        "Trusted_Connection=yes;"
        "TrustServerCertificate=yes;"
    )

    logger.debug("Conectando a SQL Server: server=%s, db=%s", server, database)
    conn = pyodbc.connect(connection_string)
    return conn


@lru_cache(maxsize=1)
def get_schema() -> dict:
    """Devuelve el schema de la BD. El resultado se cachea en memoria."""

    logger.info("Cargando schema de la base de datos (primera vez)...")
    conn = get_connection()
    cursor = conn.cursor()

    cursor.execute("""
        SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE
        FROM INFORMATION_SCHEMA.COLUMNS
        ORDER BY TABLE_NAME
    """)

    schema: dict[str, list[tuple[str, str]]] = {}

    for table, column, data_type in cursor.fetchall():
        if table not in schema:
            schema[table] = []
        schema[table].append((column, data_type))

    conn.close()
    logger.info("Schema cargado: %d tablas encontradas.", len(schema))
    return schema


def schema_to_text(schema: dict) -> str:

    texto = ""
    for tabla, columnas in schema.items():
        texto += f"\nTabla: {tabla}\n"
        for col, tipo in columnas:
            texto += f" - {col} ({tipo})\n"
    return texto