from fastapi import APIRouter

from agente_sql.schemas.models import (
    QueryRequest,
    QuickQueryRequest,
    SQLResponse,
    ErrorResponse,
)
from agente_sql.services.sql_service import (
    procesar_pregunta,
    procesar_consulta_rapida,
)
from agente_sql.infrastructure.logger import get_logger

logger = get_logger(__name__)

router = APIRouter()


@router.post(
    "/query",
    response_model=SQLResponse,
    responses={
        422: {"model": ErrorResponse, "description": "Consulta no permitida"},
        500: {"model": ErrorResponse, "description": "Error interno"},
    },
    summary="Consulta en lenguaje natural",
    description=(
        "Recibe una pregunta en lenguaje natural, genera el SQL correspondiente, "
        "lo ejecuta y devuelve la respuesta también en lenguaje natural."
    ),
)
async def query_endpoint(body: QueryRequest) -> SQLResponse:
    logger.info("POST /query — pregunta: %s", body.pregunta)
    return procesar_pregunta(body.pregunta)


@router.post(
    "/quick-query",
    response_model=SQLResponse,
    responses={
        400: {"model": ErrorResponse, "description": "Consulta rápida no válida"},
        500: {"model": ErrorResponse, "description": "Error interno"},
    },
    summary="Consulta rápida predefinida",
    description=(
        "Ejecuta una consulta rápida predefinida sin pasar por el agente IA. "
        "Pensado para búsquedas frecuentes como caja, albarán o socio."
    ),
)
async def quick_query_endpoint(body: QuickQueryRequest) -> SQLResponse:
    logger.info(
        "POST /quick-query — tipo: %s, valor: %s",
        body.type,
        body.value,
    )
    return procesar_consulta_rapida(body.type, body.value)