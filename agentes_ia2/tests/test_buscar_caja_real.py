# tests/test_buscar_caja_real.py

from agente_sql.services.quick_queries import sql_buscar_caja
from agente_sql.tools.tools_sql import ejecutar_sql


def main() -> None:
    print("=== Prueba real: buscar caja por CodigoCaja ===")
    entrada = input("Introduce el CodigoCaja: ").strip()

    if not entrada.isdigit():
        print("Error: debes introducir un número entero.")
        return

    codigo_caja = int(entrada)

    sql = sql_buscar_caja(codigo_caja)

    print("\nSQL generada:")
    print(sql)

    try:
        resultados = ejecutar_sql(sql)
    except Exception as e:
        print(f"\nError al ejecutar la consulta: {e}")
        return

    print(f"\nFilas devueltas: {len(resultados)}")

    if not resultados:
        print("No se encontraron resultados.")
        return

    print("\nPrimer resultado:")
    for clave, valor in resultados[0].items():
        print(f"- {clave}: {valor}")


if __name__ == "__main__":
    main()