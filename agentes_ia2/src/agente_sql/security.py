import re


PALABRAS_PROHIBIDAS = {
    "insert",
    "update",
    "delete",
    "drop",
    "alter",
    "truncate",
    "exec",
    "execute",
    "merge",
    "create",
    "grant",
    "revoke",
    "elimina",
    "eliminar",
    "borra",
    "borrar",
    "actualiza",
    "actualizar",
    "modifica",
    "modificar",
    "cambia",
    "cambiar",
    "añade",
    "agrega",
    "inserta",
}


def es_operacion_escritura(texto: str) -> bool:
    texto = texto.lower()
    return any(palabra in texto for palabra in PALABRAS_PROHIBIDAS)


def contiene_sql_peligroso(query: str) -> str | None:
    q = query.lower()

    for keyword in PALABRAS_PROHIBIDAS:
        if re.search(rf"\b{keyword}\b", q):
            return keyword

    return None