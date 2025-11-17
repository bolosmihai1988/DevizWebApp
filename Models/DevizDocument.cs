using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DevizWebApp.Models
{
    public class DevizDocument : IDocument
    {
        public string NumeService { get; set; } = "Sc Bls Service Automobile Srl";
        public int NrDeviz { get; set; }
        public string Data { get; set; } = string.Empty;

        public string Firma { get; set; } = string.Empty;
        public string CUI { get; set; } = string.Empty;
        public string Adresa { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;

        public string Masina { get; set; } = string.Empty;
        public string NrInmat { get; set; } = string.Empty;
        public string KM { get; set; } = string.Empty;
        public string SerieCaroserie { get; set; } = string.Empty;
        public string SerieMotor { get; set; } = string.Empty;

        public string Constatare { get; set; } = string.Empty;
        public string LucrariConvenite { get; set; } = string.Empty;
        public string PieseAduseClient { get; set; } = string.Empty;

        public List<LinieDeviz> Piese { get; set; } = new List<LinieDeviz>();
        public List<LinieDeviz> Manopera { get; set; } = new List<LinieDeviz>();

        private string LogoPath => Path.Combine(AppContext.BaseDirectory, "wwwroot", "logo.png");

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Calibri"));

                // Header
                page.Header().PaddingBottom(5).Row(row =>
                {
                    row.ConstantColumn(80).Padding(0).Element(c =>
                    {
                        if (File.Exists(LogoPath))
                            c.Image(LogoPath, ImageScaling.FitArea);
                        else
                            c.AlignCenter().Padding(5)
                             .Background(Colors.Blue.Lighten3)
                             .Column(col =>
                             {
                                 col.Item().AlignCenter().Text("BLS").FontSize(18).SemiBold().FontColor(Colors.Blue.Darken2);
                                 col.Item().AlignCenter().Text("SERVICE").FontSize(10).SemiBold();
                             });
                    });

                    row.RelativeColumn().Column(col =>
                    {
                        col.Item().Text(NumeService)
                            .FontSize(18).SemiBold().FontColor(Colors.Blue.Darken2);

                        col.Item().AlignRight().Text($"DEVIZ NR.: {NrDeviz:D4}").FontSize(12).SemiBold();
                        col.Item().AlignRight().Text($"DATA: {Data}").FontSize(12).SemiBold();
                    });
                });

                // Content
                page.Content().PaddingVertical(8).Column(containerCol =>
                {
                    containerCol.Item().Row(row =>
                    {
                        row.RelativeColumn().Column(c =>
                        {
                            c.Item().Text($"Firmă: {Firma}");
                            if (!string.IsNullOrWhiteSpace(CUI)) c.Item().Text($"CUI: {CUI}");
                            if (!string.IsNullOrWhiteSpace(Adresa)) c.Item().Text($"Adresă: {Adresa}");
                            c.Item().Text($"Telefon: {Telefon}");
                        });

                        row.RelativeColumn().AlignRight().Column(c =>
                        {
                            c.Item().Text($"Mașină: {Masina}");
                            c.Item().Text($"Nr. înmatriculare: {NrInmat}");
                            c.Item().Text($"KM: {KM}");
                            c.Item().Text($"Serie caroserie: {SerieCaroserie}");
                            c.Item().Text($"Serie motor: {SerieMotor}");
                            c.Item().Text($"Constatare: {Constatare}");
                            c.Item().Text($"Lucrări convenite: {LucrariConvenite}");
                            c.Item().Text($"Piese aduse de client: {PieseAduseClient}");
                        });
                    });

                    if (Piese.Count > 0)
                        containerCol.Item().PaddingTop(10).Element(e => GenerateStyledTable(e, "PIESE", Piese));

                    if (Manopera.Count > 0)
                        containerCol.Item().PaddingTop(10).Element(e => GenerateStyledTable(e, "MANOPERĂ", Manopera));

                    double totalFaraTVA = Piese.Sum(x => x.PretFaraTVA * x.Cantitate) + Manopera.Sum(x => x.PretFaraTVA);
                    double totalTVA = Piese.Sum(x => x.TVA * x.Cantitate) + Manopera.Sum(x => x.TVA);
                    double totalCuTVA = Piese.Sum(x => x.PretCuTVA * x.Cantitate) + Manopera.Sum(x => x.PretCuTVA);

                    containerCol.Item().PaddingTop(12).Text($"TOTAL GENERAL: Fără TVA {totalFaraTVA:F2} | TVA {totalTVA:F2} | Cu TVA {totalCuTVA:F2}")
                        .FontSize(13).SemiBold().FontColor(Colors.Blue.Darken2);

                    containerCol.Item().PaddingTop(20).Row(r =>
                    {
                        r.RelativeColumn().Text("Semnătura client: ____________________");
                        r.RelativeColumn().Text("Semnătura operator: ____________________");
                    });
                });

                // Footer
                page.Footer().Column(col =>
                {
                    col.Item().AlignCenter().Text("Toate lucrările și piesele instalate beneficiază de garanție conform legislației în vigoare.")
                       .FontSize(9).Italic().FontColor(Colors.Grey.Darken1);

                    col.Item().AlignCenter().Text(x =>
                    {
                        x.Span("Pagina ");
                        x.CurrentPageNumber();
                        x.Span(" din ");
                        x.TotalPages();
                        x.Span($"  |  {NumeService}");
                    });
                });
            });
        }

        private void GenerateStyledTable(IContainer container, string titlu, List<LinieDeviz> linii)
        {
            container.Column(col =>
            {
                col.Item().Text(titlu).FontSize(14).SemiBold().FontColor(Colors.Blue.Darken1);
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.ConstantColumn(60);
                        columns.ConstantColumn(60);
                        columns.ConstantColumn(80);
                        columns.ConstantColumn(80);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(3).Text("Denumire").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(3).AlignRight().Text("Cantitate").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(3).AlignRight().Text("Fără TVA").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(3).AlignRight().Text("TVA").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(3).AlignRight().Text("Cu TVA").SemiBold();
                    });

                    foreach (var l in linii)
                    {
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text(l.Denumire ?? "");
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).AlignRight().Text($"{l.Cantitate:F2}");
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).AlignRight().Text($"{l.PretFaraTVA:F2}");
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).AlignRight().Text($"{l.TVA:F2}");
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).AlignRight().Text($"{l.PretCuTVA:F2}");
                    }

                    double totalFaraTVA = linii.Sum(x => x.PretFaraTVA * x.Cantitate);
                    double totalTVA = linii.Sum(x => x.TVA * x.Cantitate);
                    double totalCuTVA = linii.Sum(x => x.PretCuTVA * x.Cantitate);

                    table.Cell().Background(Colors.Grey.Lighten3).Text("TOTAL").SemiBold();
                    table.Cell().Background(Colors.Grey.Lighten3).Text("-"); // Cantitate total nu se afișează
                    table.Cell().Background(Colors.Grey.Lighten3).AlignRight().Text($"{totalFaraTVA:F2}").SemiBold();
                    table.Cell().Background(Colors.Grey.Lighten3).AlignRight().Text($"{totalTVA:F2}").SemiBold();
                    table.Cell().Background(Colors.Grey.Lighten3).AlignRight().Text($"{totalCuTVA:F2}").SemiBold();
                });
            });
        }
    }
}
