import os
from fastapi import FastAPI, Request
from fastapi.responses import JSONResponse, HTMLResponse, RedirectResponse
from fastapi.staticfiles import StaticFiles

from agente_sql.infrastructure.logger import setup_logging, get_logger
from agente_sql.api.routes import query, schema, export
from fastapi.middleware.cors import CORSMiddleware

# ── Logging ──────────────────────────────────────────────────────────────────
setup_logging()
logger = get_logger(__name__)

# ── App ───────────────────────────────────────────────────────────────────────
app = FastAPI(
    title="Agente SQL",
    description=(
        "API para consultar una base de datos SQL Server "
        "usando lenguaje natural. Powered by GPT-5.2."
    ),
    version="1.0.0",
)

# ── CORS ───────────────────────────────────────────────────────────────────
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_methods=["*"],
    allow_headers=["*"],
)

# ── Routers ───────────────────────────────────────────────────────────────────
app.include_router(query.router, tags=["Consultas"])
app.include_router(schema.router, tags=["Schema"])
app.include_router(export.router, tags=["Export"])

# ── Static UI ────────────────────────────────────────────────────────────────
_UI_DIR = os.path.join(os.path.dirname(__file__), "..", "..", "..", "ui", "web")
_UI_DIR = os.path.normpath(_UI_DIR)

class NoCacheStaticFiles(StaticFiles):
    def is_not_modified(self, response_headers, request_headers) -> bool:
        return False

if os.path.isdir(_UI_DIR):
    app.mount("/ui", NoCacheStaticFiles(directory=_UI_DIR, html=True), name="web-ui")



# ── Ruta raíz ────────────────────────────────────────────────────────────────
@app.get("/", tags=["Sistema"], summary="Ruta raíz")
async def root() -> RedirectResponse:
    """Redirige a la interfaz web."""
    return RedirectResponse(url="/ui/index.html")


# ── Global exception handlers ─────────────────────────────────────────────────
@app.exception_handler(ValueError)
async def value_error_handler(request: Request, exc: ValueError) -> JSONResponse:
    logger.warning("ValueError en %s: %s", request.url.path, exc)
    return JSONResponse(
        status_code=422,
        content={"tipo": "consulta_no_permitida", "detalle": str(exc)},
    )


@app.exception_handler(RuntimeError)
async def runtime_error_handler(request: Request, exc: RuntimeError) -> JSONResponse:
    logger.error("RuntimeError en %s: %s", request.url.path, exc)
    return JSONResponse(
        status_code=500,
        content={"tipo": "error_interno", "detalle": str(exc)},
    )


@app.exception_handler(Exception)
async def generic_error_handler(request: Request, exc: Exception) -> JSONResponse:
    logger.exception("Error inesperado en %s", request.url.path)
    return JSONResponse(
        status_code=500,
        content={"tipo": "error_inesperado", "detalle": "Contacta con el administrador."},
    )


# ── Health check ──────────────────────────────────────────────────────────────
@app.get("/health", tags=["Sistema"], summary="Estado del servicio")
async def health() -> dict:
    return {"status": "ok"}