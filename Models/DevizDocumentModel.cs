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
        public string NumeService { get; set; } = "SC Bolos Service Auto SRL";
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

        public List<LinieDeviz> Piese { get; set; } = new();
        public List<LinieDeviz> Manopera { get; set; } = new();

        private string LogoPath => Path.Combine(AppContext.BaseDirectory, "logo.png");

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        // ✅ Ajută la DOWNLOAD (reconstruim modelul din DB)
        public void LoadFromDeviz(Deviz deviz, List<DevizItem> items)
        {
            NrDeviz = deviz.NrDeviz;
            Firma = deviz.Firma;
            CUI = deviz.CUI;
            Adresa = deviz.Adresa;
            Telefon = deviz.Telefon;
            Data = deviz.Data;

            Masina = deviz.Masina;
            NrInmat = deviz.NrInmat;
            KM = deviz.KM;
            SerieCaroserie = deviz.SerieCaroserie;
            SerieMotor = deviz.SerieMotor;

            Constatare = deviz.Constatare;
            LucrariConvenite = deviz.LucrariConvenite;
            PieseAduseClient = deviz.PieseAduseClient;

            Piese = items
                .Where(i => i.Tip == "piesa")
                .Select(i => new LinieDeviz
                {
                    Denumire = i.Denumire,
                    Cantitate = (double)i.Cantitate,
                    PretUnitar = (double)i.PretUnitar
                }).ToList();

            Manopera = items
                .Where(i => i.Tip == "manopera")
                .Select(i => new LinieDeviz
                {
                    Denumire = i.Denumire,
                    Cantitate = (double)i.Cantitate,
                    PretUnitar = (double)i.PretUnitar
                }).ToList();
        }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Calibri"));

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
                        var dataAfisata = string.IsNullOrWhiteSpace(Data) ? "____________________" : Data;
                        col.Item().AlignRight().Text($"DATA: {dataAfisata}").FontSize(12).SemiBold();
                        col.Item().AlignRight().Text("Prețuri fără TVA").FontSize(10).FontColor(Colors.Grey.Darken1);
                    });
                });

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
                        containerCol.Item().PaddingTop(10).Element(e => GenerateTableFaraTVA(e, "PIESE", Piese));

                    if (Manopera.Count > 0)
                        containerCol.Item().PaddingTop(10).Element(e => GenerateTableFaraTVA(e, "MANOPERĂ", Manopera));

                    double totalPiese = Piese.Sum(x => x.Total);
                    double totalManopera = Manopera.Sum(x => x.Total);
                    double totalGeneral = totalPiese + totalManopera;

                    containerCol.Item().PaddingTop(12)
                        .Text($"TOTAL GENERAL (fără TVA): {totalGeneral:F2} RON")
                        .FontSize(13).SemiBold().FontColor(Colors.Blue.Darken2);

                    containerCol.Item().PaddingTop(20).Row(r =>
                    {
                        r.RelativeItem().Text("Semnătura client: ____________________");
                        r.RelativeItem().Text("Semnătura operator: ____________________");
                    });
                });

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

        private void GenerateTableFaraTVA(IContainer container, string titlu, List<LinieDeviz> linii)
        {
            container.Column(col =>
            {
                col.Item().Text(titlu).FontSize(14).SemiBold().FontColor(Colors.Blue.Darken1);

                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();    // Denumire
                        columns.ConstantColumn(60);  // Cant
                        columns.ConstantColumn(80);  // Pret unitar
                        columns.ConstantColumn(80);  // Total
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(3).Text("Denumire").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(3).AlignRight().Text("Cant.").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(3).AlignRight().Text("Preț unitar").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(3).AlignRight().Text("Total").SemiBold();
                    });

                    foreach (var l in linii)
                    {
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text(l.Denumire ?? "");
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).AlignRight().Text($"{l.Cantitate:0.##}");
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).AlignRight().Text($"{l.PretUnitar:F2}");
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).AlignRight().Text($"{l.Total:F2}");
                    }

                    double total = linii.Sum(x => x.Total);

                    table.Cell().Background(Colors.Grey.Lighten3).Text("TOTAL").SemiBold();
                    table.Cell().Background(Colors.Grey.Lighten3).Text("");
                    table.Cell().Background(Colors.Grey.Lighten3).Text("");
                    table.Cell().Background(Colors.Grey.Lighten3).AlignRight().Text($"{total:F2}").SemiBold();
                });
            });
        }
    }
}
