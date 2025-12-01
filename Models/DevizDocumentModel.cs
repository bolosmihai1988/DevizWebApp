using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace DevizWebApp.Models
{
    public class DevizDocumentModel : IDocument
    {
        public string NumeService { get; set; } = "Sc Bls Service Automobile Srl";
        public int NrDeviz { get; set; }

        public string Firma { get; set; } = string.Empty;
        public string CUI { get; set; } = string.Empty;
        public string Adresa { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;

        public string Data { get; set; } = string.Empty;

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

        private string LogoPath => Path.Combine(AppContext.BaseDirectory, "logo.png");

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public Deviz ToDevizEntity()
        {
            return new Deviz
            {
                NrDeviz = this.NrDeviz,
                Firma = this.Firma ?? string.Empty,
                CUI = this.CUI ?? string.Empty,
                Adresa = this.Adresa ?? string.Empty,
                Telefon = this.Telefon ?? string.Empty,
                Data = this.Data ?? string.Empty,
                Masina = this.Masina ?? string.Empty,
                NrInmat = this.NrInmat ?? string.Empty,
                KM = this.KM ?? string.Empty,
                SerieCaroserie = this.SerieCaroserie ?? string.Empty,
                SerieMotor = this.SerieMotor ?? string.Empty,
                Constatare = this.Constatare ?? string.Empty,
                LucrariConvenite = this.LucrariConvenite ?? string.Empty,
                PieseAduseClient = this.PieseAduseClient ?? string.Empty
            };
        }

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
                    row.ConstantItem(80).Padding(0).Element(c =>
                    {
                        if (File.Exists(LogoPath))
                            c.Image(LogoPath);
                        else
                            c.AlignCenter().Padding(5)
                             .Background(Colors.Blue.Lighten3)
                             .Column(col =>
                             {
                                 col.Item().AlignCenter().Text("BLS").FontSize(18).SemiBold().FontColor(Colors.Blue.Darken2);
                                 col.Item().AlignCenter().Text("SERVICE").FontSize(10).SemiBold();
                             });
                    });

                    row.RelativeItem().Column(col =>
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
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text($"Firmă: {Firma}");
                            if (!string.IsNullOrWhiteSpace(CUI)) c.Item().Text($"CUI: {CUI}");
                            if (!string.IsNullOrWhiteSpace(Adresa)) c.Item().Text($"Adresă: {Adresa}");
                            c.Item().Text($"Telefon: {Telefon}");
                        });

                        row.RelativeItem().AlignRight().Column(c =>
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

                    double totalFaraTVA = Piese.Sum(x => x.PretFaraTVA) + Manopera.Sum(x => x.PretFaraTVA);
                    double totalTVA = Piese.Sum(x => x.TVA) + Manopera.Sum(x => x.TVA);
                    double totalCuTVA = Piese.Sum(x => x.PretCuTVA) + Manopera.Sum(x => x.PretCuTVA);

                    containerCol.Item().PaddingTop(12).Text($"TOTAL GENERAL: Fără TVA {totalFaraTVA:F2} | TVA {totalTVA:F2} | Cu TVA {totalCuTVA:F2}")
                        .FontSize(13).SemiBold().FontColor(Colors.Blue.Darken2);

                    containerCol.Item().PaddingTop(20).Row(r =>
                    {
                        r.RelativeItem().Text("Semnătura client: ____________________");
                        r.RelativeItem().Text("Semnătura operator: ____________________");
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
                        columns.RelativeColumn(); // Denumire
                        columns.ConstantColumn(50); // Pret/buc
                        columns.ConstantColumn(60); // Fara TVA
                        columns.ConstantColumn(60); // TVA
                        columns.ConstantColumn(60); // Cu TVA
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(3).Text("Denumire").SemiBold();
                        if (titlu == "PIESE")
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(3).AlignRight().Text("Pret/Buc").SemiBold();
                        else
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(3).AlignRight().Text("").SemiBold();

                        header.Cell().Background(Colors.Grey.Lighten3).Padding(3).AlignRight().Text("Fără TVA").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(3).AlignRight().Text("TVA").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(3).AlignRight().Text("Cu TVA").SemiBold();
                    });

                    foreach (var l in linii)
                    {
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text(l.Denumire ?? "");
                        if (titlu == "PIESE")
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).AlignRight().Text($"{(l.Cantitate > 0 ? l.PretCuTVA / l.Cantitate : l.PretCuTVA):F2}");
                        else
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).AlignRight().Text("");

                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).AlignRight().Text($"{l.PretFaraTVA:F2}");
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).AlignRight().Text($"{l.TVA:F2}");
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).AlignRight().Text($"{l.PretCuTVA:F2}");
                    }

                    double totalFaraTVA = linii.Sum(x => x.PretFaraTVA);
                    double totalTVA = linii.Sum(x => x.TVA);
                    double totalCuTVA = linii.Sum(x => x.PretCuTVA);

                    table.Cell().Background(Colors.Grey.Lighten3).Text("TOTAL").SemiBold();
                    if (titlu == "PIESE")
                        table.Cell().Background(Colors.Grey.Lighten3).Text(""); // coloana Pret/Buc total
                    table.Cell().Background(Colors.Grey.Lighten3).AlignRight().Text($"{totalFaraTVA:F2}").SemiBold();
                    table.Cell().Background(Colors.Grey.Lighten3).AlignRight().Text($"{totalTVA:F2}").SemiBold();
                    table.Cell().Background(Colors.Grey.Lighten3).AlignRight().Text($"{totalCuTVA:F2}").SemiBold();
                });
            });
        }
    }
}
