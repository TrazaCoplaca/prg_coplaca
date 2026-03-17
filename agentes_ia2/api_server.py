"""
Punto de entrada para arrancar el servidor FastAPI con uvicorn.

Uso:
    python api_server.py
    # o en modo desarrollo con recarga automática:
    uvicorn agente_sql.api.app:app --reload --host 0.0.0.0 --port 8000

Documentación interactiva disponible en:
    http://localhost:8000/docs      (Swagger UI)
    http://localhost:8000/redoc     (ReDoc)
"""

import uvicorn

if __name__ == "__main__":
    uvicorn.run(
        "agente_sql.api.app:app",
        host="0.0.0.0",
        port=8000,
        reload=True,          # recarga automática al guardar cambios
        log_level="info",
    )
