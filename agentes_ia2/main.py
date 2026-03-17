from agente_sql.infrastructure.logger import setup_logging, get_logger
from agente_sql.services.sql_service import procesar_pregunta

setup_logging()
logger = get_logger(__name__)

SALIR_CMDS = {"salir", "exit", "q", "quit"}


def main() -> None:
    print("\n🤖 Agente SQL — escribe tu pregunta o 'salir' para terminar.\n")
    logger.info("Sesión iniciada.")

    while True:
        try:
            pregunta = input("Pregunta: ").strip()
        except (EOFError, KeyboardInterrupt):
            print("\n\nHasta luego! 👋")
            logger.info("Sesión terminada por el usuario (Ctrl+C / EOF).")
            break

        if not pregunta:
            continue

        if pregunta.lower() in SALIR_CMDS:
            print("Hasta luego! 👋")
            logger.info("Sesión terminada por comando '%s'.", pregunta)
            break

        logger.info("Pregunta recibida: %s", pregunta)

        try:
            respuesta = procesar_pregunta(pregunta)

            print(f"\n📊 SQL generado:\n{respuesta.sql}\n")
            print(f"💬 {respuesta.mensaje}\n")

        except ValueError as e:
            print(f"\n⚠️  Aviso: {e}\n")
            logger.warning("ValueError: %s", e)

        except RuntimeError as e:
            print(f"\n❌ Error: {e}\n")
            logger.error("RuntimeError: %s", e)

        except Exception as e:
            print(f"\n❌ Error inesperado: {e}\n")
            logger.exception("Error inesperado en main loop.")


if __name__ == "__main__":
    main()