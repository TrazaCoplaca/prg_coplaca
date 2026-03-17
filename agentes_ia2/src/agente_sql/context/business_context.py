BUSINESS_CONTEXT = """
El sistema trabaja con trazabilidad agrícola y empaquetado.

Reglas de negocio importantes:

- La tabla principal en esta fase es TrazaCaja.
- El campo Socio identifica al productor.
- El campo Finca no es un identificador global.
- Una finca se identifica dentro de cada socio.
- Por tanto, la relación correcta con la tabla Finca requiere:
  Finca.CodigoSocio = TrazaCaja.Socio
  AND Finca.Codigo = TrazaCaja.Finca
- El campo Cooperativa distingue registros de distintas cooperativas que pueden
  compartir el mismo CodigoCaja. Inclúyelo siempre en las consultas de TrazaCaja.
- Nunca unir Finca solo por el código de finca.
"""