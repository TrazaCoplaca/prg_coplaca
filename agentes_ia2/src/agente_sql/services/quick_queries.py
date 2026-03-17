import re

def sql_buscar_caja(codigo_caja: int) -> str:
    return f"""
SELECT TOP (50)
    tc.CodigoCaja,
    tc.Cooperativa,
    tc.FechaConf,
    tc.HoraConf,
    tc.Socio,
    s.Nombre AS NombreSocio,
    tc.Finca,
    tc.Parcela,
    tc.Linea,
    tc.Albaran,
    tc.Lote,
    tc.CodProducto,
    tc.Categoria,
    tc.Confeccion,
    tc.Envase,
    tc.Variedad,
    tc.CajasPalet,
    tc.KilosNetos,
    tc.Traspasado
FROM TrazaCaja tc
LEFT JOIN Socio s
    ON s.Codigo = tc.Socio
WHERE tc.CodigoCaja = {codigo_caja}
ORDER BY tc.FechaConf DESC, tc.HoraConf DESC
""".strip()


def sql_lote_de_caja(codigo_caja: int) -> str:
    return f"""
SELECT
    tc.CodigoCaja,
    tc.Lote
FROM TrazaCaja tc
WHERE tc.CodigoCaja = {codigo_caja}
""".strip()


def sql_albaran_de_caja(codigo_caja: int) -> str:
    return f"""
SELECT
    tc.CodigoCaja,
    tc.Albaran
FROM TrazaCaja tc
WHERE tc.CodigoCaja = {codigo_caja}
""".strip()


def sql_cajas_de_albaran(albaran: int) -> str:
    return f"""
SELECT TOP (50)
    tc.Cooperativa,
    tc.Albaran,
    tc.CodigoCaja,
    tc.FechaConf,
    tc.HoraConf,
    tc.Socio,
    tc.Finca,
    tc.Linea,
    tc.npalet,
    tc.CajasPalet,
    tc.Lote,
    tc.CodProducto,
    tc.Categoria,
    tc.Confeccion,
    tc.Envase,
    tc.Variedad,
    tc.KilosNetos,
    tc.Traspasado
FROM TrazaCaja tc
WHERE tc.Albaran = {albaran}
ORDER BY tc.FechaConf DESC, tc.HoraConf DESC, tc.CodigoCaja DESC
""".strip()


def sql_productos_en_albaran(albaran: int) -> str:
    return f"""
SELECT DISTINCT
    tc.Albaran,
    tc.CodProducto
FROM TrazaCaja tc
WHERE tc.Albaran = {albaran}
ORDER BY tc.CodProducto
""".strip()


def sql_cajas_de_socio(socio: int) -> str:
    return f"""
SELECT TOP (50)
    tc.Cooperativa,
    tc.CodigoCaja,
    tc.FechaConf,
    tc.HoraConf,
    tc.Socio,
    s.Nombre AS NombreSocio,
    tc.Finca,
    tc.Linea,
    tc.Albaran,
    tc.Lote,
    tc.CodProducto,
    tc.Categoria,
    tc.Confeccion,
    tc.Envase,
    tc.Variedad,
    tc.KilosNetos,
    tc.Traspasado
FROM TrazaCaja tc
LEFT JOIN Socio s
    ON s.Codigo = tc.Socio
WHERE tc.Socio = {socio}
ORDER BY tc.FechaConf DESC, tc.HoraConf DESC, tc.CodigoCaja DESC
""".strip()

def sql_info_palet(npalet: int) -> str:
    return f"""
SELECT TOP (100)
    cp.NPalet,
    cp.CodigoCaja,
    cp.Albaran,
    cp.Cooperativa,
    rop.OrdenCarga,
    rop.Ntermografo,
    rop.Descripcion
FROM CajasPalet cp
LEFT JOIN RelOCPalet rop
    ON rop.Npalet = cp.NPalet
WHERE cp.NPalet = {npalet}
ORDER BY cp.CodigoCaja
""".strip()