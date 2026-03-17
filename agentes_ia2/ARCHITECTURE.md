# Arquitectura del proyecto

El proyecto está dividido en varios módulos con responsabilidades claras.

## agente_sql/

Contiene la lógica principal del agente.

### agent/
Orquestación del agente:
- generación de SQL
- ejecución de consultas
- interpretación de resultados

### api/
Endpoints FastAPI.

- query → consultas en lenguaje natural
- schema → información de esquema

### infrastructure/
Acceso a servicios externos.

- sqlserver.py → conexión a SQL Server
- config.py → variables de entorno
- logger.py → logging

### services/
Lógica de aplicación.

### context/
Contexto usado por el modelo:
- business_context.py
- table_context.py

### tools/
Ejecución segura de SQL.

## ui/
Interfaz web del agente.