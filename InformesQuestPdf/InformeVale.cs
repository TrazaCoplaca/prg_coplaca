

using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using System.Diagnostics;
using System.IO;
using System.Printing;
using System.Linq;

public class DatosCabecera
{
    public string Grupo { get; set; }
    public string Direccion1 { get; set; }
    public string Direccion2 { get; set; }
    public string ID { get; set; }
    public string Fecha { get; set; }
    public string Semana { get; set; }
    public string Matricula { get; set; }
    public string MatTractora { get; set; }
    public string Temperatura { get; set; }
    public string Termografo { get; set; }
    public string Transportista { get; set; }
    public string NifTrans { get; set; }
    public string Destino { get; set; }
    public string Cargador { get; set; }
    public string NifCargador { get; set; }
    public string Direccion { get; set; }
    public string Poblacion { get; set; }
    public string Naviera { get; set; }
    public string Buque { get; set; }
    public string DestinoBarco { get; set; }
    public string Receptor { get; set; }
    public string Observaciones { get; set; }
    public string Orden { get; set; }
    public string isla { get; set; }
    public string biTemp { get; set; }
    public string Temp2 { get; set; }
    public string nTerm2 { get; set; }
    public string nTerm3 { get; set; }
    public string OrdExp { get; set; }
}

public class LineaEmbarque
{
    public string Producto { get; set; }
    public string TipoProd { get; set; }
    public string Descripcion { get; set; }
    public string PesoNeto { get; set; }
    public string Cajas { get; set; }
    public string NumPalet { get; set; }
    public string TotalKilos { get; set; }
    public string asterisco { get; set; }
    public string desPalet { get; set; }
    public string desCaja { get; set; }
}

public class InformeVale
{
    public void Generar(DatosCabecera cabecera, List<LineaEmbarque> lineas, string rutaPdf)
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
                                text.Line("COPLACA SOC. COOP.").FontSize(9).Bold();
                                text.Line("CIF: F38008579").FontSize(8);
                                text.Line("Av. Francisco la Roche 11").FontSize(8);
                                text.Line("38001 - Santa Cruz de Tenerife").FontSize(8);
                                text.Line("Tfno: 922 268 300").FontSize(8);
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
                            right.Item().Text(t => { t.Span("Fecha: "); t.Span(cabecera.Fecha).Bold(); 
                                t.Span(" Semana/Orden: "); t.Span(cabecera.Semana).Bold();t.Span("/").Bold(); t.Span(cabecera.Orden).Bold();
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
                    string Expresion ="";
                    if (!string.IsNullOrWhiteSpace(cabecera.biTemp) && cabecera.biTemp.ToLower() == "true")
                        Expresion ="Bitemperatura ";
                    if (!string.IsNullOrWhiteSpace(cabecera.Temp2))
                        Expresion += $" {cabecera.Temp2};";
                    if (!string.IsNullOrWhiteSpace(cabecera.nTerm2))
                        Expresion += $" Term. nº 2: {cabecera.nTerm2};";
                    if (!string.IsNullOrWhiteSpace(cabecera.nTerm2))
                        Expresion += $" Term. nº 3: {cabecera.nTerm3};";
                    if (!string.IsNullOrWhiteSpace(cabecera.OrdExp))
                        Expresion += $" NºOrdenº: {cabecera.OrdExp}";

                    if (!string.IsNullOrWhiteSpace(Expresion))
                        col.Item().Padding(5).Background(Colors.Grey.Lighten3).Text($"Datos de Interes: {Expresion}").FontSize(9).Italic().FontColor(Colors.Black);


                    if (!string.IsNullOrWhiteSpace(cabecera.Observaciones))
                        col.Item().Padding(5).Background(Colors.Grey.Lighten3).Text($"Observaciones: {cabecera.Observaciones}").FontSize(9).Italic().FontColor(Colors.Black);
  
                    col.Item().Text("(*)").FontSize(9).Bold();
                    col.Item().Text("Origen de todos los productos ESPAÑA").FontSize(6);
                    col.Item().Text("1 Marca amparada Plátano de Canarias IGP").FontSize(6);
                    col.Item().Text("2 Producto certificado GLOBAL GAP, con nº GGN 4049928187833").FontSize(6);
                    col.Item().Text("3 Consejo Regulador de Agricultura Ecológica Nº ES-ECO-014-IC").FontSize(6);
                    col.Item().Text("4 Producto Certificado BIOSUISSE ORGANIC").FontSize(6);
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
