import logging
import logging.handlers
import os


def setup_logging(level: int = logging.DEBUG) -> None:
    """Configura logging centralizado: INFO en consola, DEBUG en fichero."""

    os.makedirs("logs", exist_ok=True)

    root = logging.getLogger()
    root.setLevel(level)

    # --- Consola: INFO y superior ---
    console = logging.StreamHandler()
    console.setLevel(logging.INFO)
    console.setFormatter(
        logging.Formatter("[%(levelname)s] %(message)s")
    )

    # --- Fichero rotativo: DEBUG y superior ---
    file_handler = logging.handlers.RotatingFileHandler(
        filename="logs/agente_sql.log",
        maxBytes=5 * 1024 * 1024,  # 5 MB
        backupCount=3,
        encoding="utf-8",
    )
    file_handler.setLevel(logging.DEBUG)
    file_handler.setFormatter(
        logging.Formatter(
            "%(asctime)s | %(levelname)-8s | %(name)s | %(message)s",
            datefmt="%Y-%m-%d %H:%M:%S",
        )
    )

    if not root.handlers:
        root.addHandler(console)
        root.addHandler(file_handler)


def get_logger(name: str) -> logging.Logger:
    return logging.getLogger(name)
