# tests/test_quick_queries.py

from agente_sql.services.quick_queries import (
    sql_buscar_caja,
    sql_lote_de_caja,
    sql_albaran_de_caja,
    sql_cajas_de_albaran,
    sql_productos_en_albaran,
    sql_cajas_de_socio,
)


def test_sql_buscar_caja():
    sql = sql_buscar_caja(2851701)

    assert "FROM TrazaCaja tc" in sql
    assert "LEFT JOIN Socio s" in sql
    assert "WHERE tc.CodigoCaja = 2851701" in sql
    assert "ORDER BY tc.FechaConf DESC, tc.HoraConf DESC" in sql


def test_sql_lote_de_caja():
    sql = sql_lote_de_caja(2851701)

    assert "tc.CodigoCaja" in sql
    assert "tc.Lote" in sql
    assert "FROM TrazaCaja tc" in sql
    assert "WHERE tc.CodigoCaja = 2851701" in sql


def test_sql_albaran_de_caja():
    sql = sql_albaran_de_caja(2851701)

    assert "tc.CodigoCaja" in sql
    assert "tc.Albaran" in sql
    assert "FROM TrazaCaja tc" in sql
    assert "WHERE tc.CodigoCaja = 2851701" in sql


def test_sql_cajas_de_albaran():
    sql = sql_cajas_de_albaran(11502097)

    assert "SELECT TOP (50)" in sql
    assert "FROM TrazaCaja tc" in sql
    assert "WHERE tc.Albaran = 11502097" in sql
    assert "tc.npalet" in sql
    assert "tc.CajasPalet" in sql
    assert "ORDER BY tc.FechaConf DESC, tc.HoraConf DESC, tc.CodigoCaja DESC" in sql


def test_sql_productos_en_albaran():
    sql = sql_productos_en_albaran(11502097)

    assert "SELECT DISTINCT" in sql
    assert "tc.Albaran" in sql
    assert "tc.CodProducto" in sql
    assert "FROM TrazaCaja tc" in sql
    assert "WHERE tc.Albaran = 11502097" in sql
    assert "ORDER BY tc.CodProducto" in sql


def test_sql_cajas_de_socio():
    sql = sql_cajas_de_socio(29)

    assert "SELECT TOP (50)" in sql
    assert "FROM TrazaCaja tc" in sql
    assert "LEFT JOIN Socio s" in sql
    assert "WHERE tc.Socio = 29" in sql
    assert "s.Nombre AS NombreSocio" in sql
    assert "ORDER BY tc.FechaConf DESC, tc.HoraConf DESC, tc.CodigoCaja DESC" in sql