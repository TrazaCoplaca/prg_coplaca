# Agente SQL

Aplicación en Python para consultar una base de datos SQL Server mediante lenguaje natural y mediante consultas rápidas predefinidas.

El proyecto incluye:

- modo consola
- API con FastAPI
- interfaz web
- consultas rápidas sin pasar por IA
- exportación de resultados

---

## Requisitos

- Python 3.11.9 o superior
- Acceso a una base de datos SQL Server
- Variables de entorno configuradas en un archivo `.env`

---

## Instalación

```bash
py -m venv .venv
.venv\Scripts\activate
python -m pip install --upgrade pip
python -m pip install -e .

## Modo Consola

```bash
python main.py
```

## Modo API

```bash
python api_server.py
```

## Con el servidor arrancado - navegador

```bash
http://localhost:8000/ui/index.html
```