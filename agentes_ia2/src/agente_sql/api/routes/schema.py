from fastapi import APIRouter

from agente_sql.schemas.models import SchemaResponse
from agente_sql.services.sql_service import obtener_schema
from agente_sql.infrastructure.logger import get_logger

logger = get_logger(__name__)

router = APIRouter()


@router.get(
    "/schema",
    response_model=SchemaResponse,
    summary="Schema de la base de datos",
    description=(
        "Devuelve las tablas y columnas visibles para el agente. "
        "El resultado está cacheado en memoria y no genera una nueva consulta a la BD."
    ),
)
async def schema_endpoint() -> SchemaResponse:
    logger.info("GET /schema")
    return obtener_schema()
