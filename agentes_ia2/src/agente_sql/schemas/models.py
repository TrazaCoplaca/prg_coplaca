from pydantic import BaseModel, Field
from pydantic import BaseModel


class QuickQueryRequest(BaseModel):
    type: str
    value: int


class QueryRequest(BaseModel):
    pregunta: str = Field(..., min_length=3, examples=["¿Cuántos productos hay en cada categoría?"])


class SQLResponse(BaseModel):
    pregunta: str
    sql: str
    resultados: list[dict]
    filas: int
    mensaje: str


class SchemaResponse(BaseModel):
    tablas: dict
    total_tablas: int


class ErrorResponse(BaseModel):
    tipo: str
    detalle: str