using InformesQuestPdf.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Windows.Documents;

namespace InformesQuestPdf
{
    public class InformeValeDatosInteres
    {
        public void Generar(DatosCabecera cabecera, List<LineaEmbarque> lineas, List<LineaDatosInteres> lineaDatosInteres, string rutaPdf)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var rutaLogo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plantillas/Logo-Coplaca_Apaisado.png");

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontFamily("Helvetica").FontSize(10));

                    page.Header().Column(col =>
                    {
                        col.Item().PaddingBottom(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten1);
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(left =>
                            {
                                left.Item().Height(25).Image(rutaLogo);
                                left.Item().AlignLeft().Text(text =>
                                {
                                    text.AlignCenter();
                                    text.Line("COPLACA SOC. COOP.CAN.").FontSize(9).Bold();
                                    text.Line("CIF: F38008579").FontSize(8);
                                    text.Line("Av. Francisco la Roche 11").FontSize(8);
                                    text.Line("38001 - Santa Cruz de Tenerife").FontSize(8);
                                    text.Line("Tfno: 922 286 300").FontSize(8);
                                });
                            });

                            row.RelativeItem().Column(right =>
                            {
                                right.Item().AlignMiddle().AlignCenter().Text("VALE DE SALIDA")
                                    .FontSize(20).Bold().FontColor(Colors.Green.Medium);
                                right.Item().AlignCenter().Text(text =>
                                {
                                    text.Line(cabecera.Grupo).FontSize(10).FontColor(Colors.Grey.Darken2);
                                    text.Line(cabecera.Direccion1).FontSize(9);
                                    text.Line(cabecera.Direccion2).FontSize(9);
                                });
                            });
                        });
                    });

                    page.Content().Column(col =>
                    {
                        col.Spacing(10);

                        col.Item().Border(1).Padding(10).Background(Colors.Grey.Lighten3).Row(datos =>
                        {
                            datos.RelativeItem().Column(left =>
                            {
                                left.Spacing(5);
                                left.Item().Text(t => { t.Span("Nº de Hoja: "); t.Span(cabecera.ID).Bold(); });
                                left.Item().Text(t => { t.Span("Matrícula Remolque: "); t.Span(cabecera.Matricula).Bold(); });
                                left.Item().Text(t => { t.Span("Transportista: "); t.Span(cabecera.Transportista).Bold(); });
                                left.Item().Text(t => { t.Span(cabecera.NifTrans).Bold(); });
                                left.Item().Text(t => { t.Span("Matrícula Tractora: "); t.Span(cabecera.MatTractora).Bold(); });
                                left.Item().Text(t => { t.Span("Temperatura Transporte: "); t.Span(cabecera.Temperatura).Bold(); });
                                left.Item().Text(t => { t.Span("NºTermógrafo: "); t.Span(cabecera.Termografo).Bold(); });
                                left.Item().Text(t => { t.Span("Destino: "); t.Span(cabecera.isla).Bold(); });
                                left.Item().Text(t => { t.Span("Consignado a: "); t.Span("COPLACA").Bold(); });
                            });

                            datos.RelativeItem().Column(right =>
                            {
                                right.Spacing(5);
                                right.Item().Text(t => {
                                    t.Span("Fecha: "); t.Span(cabecera.Fecha).Bold();
                                    t.Span(" Semana/Orden: "); t.Span(cabecera.Semana).Bold(); t.Span("/").Bold(); t.Span(cabecera.Orden).Bold();
                                });
                                right.Item().Text(t => { t.Span("Cargador: "); t.Span(cabecera.Cargador).Bold(); });
                                right.Item().Text(t => { t.Span(""); t.Span(cabecera.NifCargador).Bold(); });
                                right.Item().Text(t => { t.Span(""); t.Span(cabecera.Direccion).Bold(); });
                                right.Item().Text(t => { t.Span(""); t.Span(cabecera.Poblacion).Bold(); });
                                right.Item().Text(t => { t.Span("Naviera: "); t.Span(cabecera.Naviera).Bold(); });
                                right.Item().Text(t => { t.Span("Buque: "); t.Span(cabecera.Buque).Bold(); });
                                right.Item().Text(t => { t.Span("Destino: "); t.Span(cabecera.DestinoBarco).Bold(); });
                                right.Item().Text(t => { t.Span("Receptor: "); t.Span(cabecera.Receptor).Bold(); });
                            });
                        });

                        col.Item().Text("\nLíneas del Embarque:").FontSize(12).Bold();

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(35);
                                columns.ConstantColumn(60);
                                columns.RelativeColumn(2);
                                columns.ConstantColumn(25);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Prod.").SemiBold().FontSize(8);
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("").SemiBold().FontSize(8);
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Descripción").SemiBold().FontSize(7);
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("(*)").SemiBold().FontSize(5);
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Cajas").SemiBold().FontSize(8);
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("P.Neto (Kg)").SemiBold().FontSize(8);
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Tipo.Caja").SemiBold().FontSize(7);
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Tipo Palet").SemiBold().FontSize(7);
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Nº.Palés").SemiBold().FontSize(8);
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Total(Kg)").SemiBold().FontSize(8);
                            });

                            foreach (var linea in lineas)
                            {
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(linea.Producto).FontSize(8);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(linea.TipoProd).FontSize(8);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(linea.Descripcion).FontSize(7);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(linea.asterisco).FontSize(5);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text(linea.Cajas).FontSize(8);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text(linea.PesoNeto).FontSize(8);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text(linea.desCaja).FontSize(5);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text(linea.desPalet).FontSize(5);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text(linea.NumPalet).FontSize(8);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text(linea.TotalKilos).FontSize(8);
                            }

                            table.Cell().ColumnSpan(10).PaddingTop(10).PaddingBottom(5).AlignRight().Text("Resumen total de líneas del embarque:").Italic().FontSize(9);

                            var totalCajas = lineas.Sum(l => int.TryParse(l.Cajas, out var c) ? c : 0);
                            var totalPesoNeto = lineas.Sum(l => int.TryParse(l.PesoNeto, out var p) ? p : 0);
                            var totalPalets = lineas.Sum(l => int.TryParse(l.NumPalet, out var n) ? n : 0);
                            var totalKilos = lineas.Sum(l => int.TryParse(l.TotalKilos, out var k) ? k : 0);

                            table.Cell().ColumnSpan(4).Background(Colors.Grey.Lighten3).BorderTop(1).BorderColor(Colors.Grey.Darken2).Padding(5).AlignRight().Text("Totales:").Bold();
                            table.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignRight().Text(totalCajas.ToString()).Bold();
                            //table.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignRight().Text(totalPesoNeto.ToString()).Bold();
                            table.Cell().Padding(5);
                            table.Cell().Padding(5);
                            table.Cell().Padding(5);
                            table.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignRight().Text(totalPalets.ToString()).Bold();
                            table.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignRight().Text(totalKilos.ToString()).Bold();
                        });
                        

                        if (lineaDatosInteres != null && lineaDatosInteres.Count > 0)
                        {
                            col.Item().Column(c2 =>
                            {
                                // Título
                                c2.Item()
                                    .PaddingBottom(5)
                                    .Text("DATOS ADICIONALES:")
                                    .FontSize(10)
                                    .Bold()
                                    .Underline();
                                    

                                // Tabla
                                c2.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                      
                                        columns.RelativeColumn();      // Producto
                                        columns.RelativeColumn();      // Termógrafo
                                        columns.ConstantColumn(75);    // Temp
                                    });

                                    // Cabecera
                                    table.Header(header =>
                                    {
                                       
                                        header.Cell().Text("Producto").Bold();
                                        header.Cell().Text("Termógrafo").Bold();
                                        header.Cell().Text("Temperatura").Bold();
                                    });

                                    // Filas
                                    foreach (var d in lineaDatosInteres)
                                    {
                                       
                                        table.Cell().Text(d.Producto);
                                        table.Cell().Text(d.Termografo);
                                        table.Cell()
                                           .AlignCenter()
                                           .Text(d.Temp);
                                        
                                    }
                                });
                            });
                        }
                        col.Item().Text(t =>
                        {
                            t.Line("(*)").FontSize(9).Bold();
                            t.Line("1 Marca amparada Plátano de Canarias IGP").FontSize(6);
                            t.Line("2 Producto certificado GLOBAL G.A.P., con nº GGN 4049928187833").FontSize(6);
                            t.Line("3 Consejo Regulador de Agricultura Ecológica Nº ES-ECO-014-IC").FontSize(6);
                            t.Line("4 Producto Certificado BIOSUISSE ORGANIC").FontSize(6);
                        });

                    });

                    page.Footer().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        // Fila 1
                        table.Cell().Text("Fruta empaquetada para COPLACA por:").FontSize(8).AlignCenter();
                        table.Cell().Text("Fecha").FontSize(8).AlignCenter();
                        table.Cell().Text("Firma del Almacén").FontSize(8).AlignCenter();

                        // Fila 2
                        table.Cell().Text(cabecera.Grupo).FontSize(9).Bold().AlignCenter();
                        table.Cell().Text(cabecera.Fecha).FontSize(9).AlignCenter();
                        table.Cell(); // Celda vacía para firma manual
                    });
                });
            })
            .GeneratePdf(rutaPdf);

            Process.Start(new ProcessStartInfo(rutaPdf) { UseShellExecute = true });
        }
       



    }
}
