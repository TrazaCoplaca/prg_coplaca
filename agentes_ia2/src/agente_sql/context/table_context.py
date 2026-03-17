TABLE_CONTEXT = """
=== ESQUEMA: dbo.Albaran ===

Tabla de cabecera de albaranes de recepción de producción.
Relación con TrazaCaja: 1 albarán → N cajas (TrazaCaja.Albaran = Albaran.Codigo).

Columnas y tipos:
  Codigo         int           NOT NULL   -- PK del albarán (join con TrazaCaja.Albaran)
  Socio          int           NULL       -- FK → Socio.Codigo
  Finca          int           NULL       -- Código local de finca (no global, igual que TrazaCaja.Finca)
  Fecha          datetime      NULL       -- Fecha del albarán
  Semana         int           NULL       -- Semana del año
  Observacion    nvarchar      NULL
  Ejercicio      smallint      NULL       -- Año/ejercicio
  Acreditado     tinyint       NULL
  Bichorio       tinyint       NULL
  Taladro        tinyint       NULL
  cochinilla     tinyint       NULL
  Negro          tinyint       NULL
  rosca          tinyint       NULL
  hongos         tinyint       NULL
  maduro         tinyint       NULL
  habichuelado   tinyint       NULL
  Rechazado      tinyint       NULL
  ResFito        tinyint       NULL
  Parcela        smallint      NULL
  PreTrsSocio    real          NULL
  Linea          int           NULL

NOTA: Socio, Finca, Parcela y Linea en Albaran NO son identificadores globales únicos.
  Tienen el mismo comportamiento que en TrazaCaja.

=== ESQUEMA: dbo.Finca ===

Columnas y tipos:
  CodigoSocio    int           NOT NULL   -- FK → Socio.Codigo
  Codigo         int           NOT NULL   -- Código local de finca (único dentro del socio)
  Descripcion    nvarchar      NULL       -- Nombre o descripción de la finca
  Direccion      nvarchar      NULL
  Poblacion      nvarchar      NULL
  Provincia      nvarchar      NULL
  Observacion    nvarchar      NULL
  CodLibro       int           NULL
  Parcela        tinyint       NULL
  Superficie     float         NULL       -- Superficie en hectáreas
  CodFincaCop    int           NULL
  TipProduccion  nvarchar      NULL

CLAVE COMPUESTA: (CodigoSocio, Codigo)
  El código de finca NO es único globalmente.
  Cada socio tiene su propia numeración de fincas (1, 2, 3...).
  Otro socio puede tener también una Finca 1, 2, etc.

=== RELACIONES ENTRE TABLAS ===

TrazaCaja → Socio
  JOIN Socio s ON s.Codigo = tc.Socio

TrazaCaja → Albaran  ← TIPOS COMPATIBLES: ambos int
  JOIN Albaran a ON a.Codigo = tc.Albaran
  Usar solo si se necesitan datos de cabecera (Fecha, Semana, Ejercicio...).
  Si solo filtra por albarán: WHERE tc.Albaran = <número>

TrazaCaja → Finca  ← JOIN COMPUESTO OBLIGATORIO
  JOIN Finca f
    ON f.CodigoSocio = tc.Socio
   AND f.Codigo      = tc.Finca
  NUNCA: JOIN Finca f ON f.Codigo = tc.Finca   (genera duplicados)
  Si solo filtra por finca: WHERE tc.Finca = <número> AND tc.Socio = <socio>
"""

