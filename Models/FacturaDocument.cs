using DevizWebApp.Models;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;

namespace DevizWebApp.Models
{
    public class FacturaDocument : IDocument
    {
        private readonly Factura _f;

        public FacturaDocument(Factura factura) => _f = factura;

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        private static string Env(string key, string fallback = "")
            => Environment.GetEnvironmentVariable(key) ?? fallback;

        public void Compose(IDocumentContainer container)
        {
            string sellerName = Env("SELLER_NAME", "SC Bolos Service Auto SRL");
            string sellerJ = Env("SELLER_J");
            string sellerCui = Env("SELLER_CUI");
            string sellerAddr = Env("SELLER_ADDRESS");
            string sellerBank = Env("SELLER_BANK");
            string sellerIban = Env("SELLER_IBAN");
            string sellerCapital = Env("SELLER_CAPITAL");
            string serie = Env("INVOICE_SERIES", "");

            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Calibri"));

                page.Header().Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Border(1).Padding(8).Column(c =>
                        {
                            c.Item().Text("Furnizor:").SemiBold();
                            c.Item().Text(sellerName).SemiBold();

                            if (!string.IsNullOrWhiteSpace(sellerCui))
                                c.Item().Text($"C.I.F.: {sellerCui}");
                            if (!string.IsNullOrWhiteSpace(sellerJ))
                                c.Item().Text($"Nr. ord. reg. com.: {sellerJ}");
                            if (!string.IsNullOrWhiteSpace(sellerAddr))
                                c.Item().Text($"Sediu: {sellerAddr}");

                            if (!string.IsNullOrWhiteSpace(sellerBank))
                                c.Item().Text($"Banca: {sellerBank}");
                            if (!string.IsNullOrWhiteSpace(sellerIban))
                                c.Item().Text($"Cont (IBAN): {sellerIban}");
                            if (!string.IsNullOrWhiteSpace(sellerCapital))
                                c.Item().Text(sellerCapital);
                        });

                        row.ConstantItem(15);

                        row.RelativeItem().Border(1).Padding(8).Column(c =>
                        {
                            c.Item().Text("Cumpărător:").SemiBold();
                            c.Item().Text(_f.ClientNume ?? "").SemiBold();

                            if (!string.IsNullOrWhiteSpace(_f.ClientCUI))
                                c.Item().Text($"C.I.F.: {_f.ClientCUI}");
                            if (!string.IsNullOrWhiteSpace(_f.ClientAdresa))
                                c.Item().Text($"Sediu: {_f.ClientAdresa}");
                        });
                    });

                    col.Item().PaddingTop(10).AlignCenter().Text("FACTURĂ").FontSize(16).SemiBold();

                    // AICI e fixul: bold prin DefaultTextStyle, fără chain după Text(...)
                    col.Item()
                        .PaddingTop(6)
                        .AlignCenter()
                        .DefaultTextStyle(x => x.SemiBold())
                        .Text(t =>
                        {
                            if (!string.IsNullOrWhiteSpace(serie))
                                t.Span($"SERIA: {serie}   ");

                            t.Span($"NR. FACTURII: {_f.NrFactura:D4}   ");
                            t.Span($"DATA: {_f.Data}");
                        });

                    col.Item().PaddingTop(10).LineHorizontal(1);
                });

                page.Content().PaddingTop(10).Column(col =>
                {
                    col.Item().Table(t =>
                    {
                        t.ColumnsDefinition(c =>
                        {
                            c.ConstantColumn(35);
                            c.RelativeColumn();
                            c.ConstantColumn(45);
                            c.ConstantColumn(55);
                            c.ConstantColumn(80);
                            c.ConstantColumn(80);
                        });

                        t.Header(h =>
                        {
                            h.Cell().Element(HeaderCell).Text("Nr.\ncrt");
                            h.Cell().Element(HeaderCell).Text("Denumirea produselor sau a serviciilor");
                            h.Cell().Element(HeaderCell).Text("U.M.");
                            h.Cell().Element(HeaderCell).Text("Cant.");
                            h.Cell().Element(HeaderCell).Text("Preț unitar\n(RON)");
                            h.Cell().Element(HeaderCell).Text("Valoare\n(RON)");
                        });

                        int nr = 1;
                        foreach (var it in _f.Items)
                        {
                            t.Cell().Element(RowCellRight).Text(nr.ToString());
                            t.Cell().Element(RowCell).Text(it.Denumire ?? "");
                            t.Cell().Element(RowCellRight).Text(it.UM ?? "");
                            t.Cell().Element(RowCellRight).Text($"{it.Cantitate:0.##}");
                            t.Cell().Element(RowCellRight).Text($"{it.PretUnitar:F2}");
                            t.Cell().Element(RowCellRight).Text($"{it.TotalLinie:F2}");
                            nr++;
                        }

                        t.Cell().ColumnSpan(5).Element(TotalLabelCell).Text("TOTAL");
                        t.Cell().Element(TotalValueCell).Text($"{_f.TotalGeneral:F2}");
                    });

                    col.Item().PaddingTop(20).Row(r =>
                    {
                        r.RelativeItem().Text("Semnătura și ștampila furnizorului: ____________________").FontSize(9);
                        r.RelativeItem().AlignRight().Text("Semnătura de primire: ____________________").FontSize(9);
                    });
                });

                page.Footer().AlignCenter().Text("Fără TVA").FontSize(9).FontColor(Colors.Grey.Darken1);
            });
        }

        private static IContainer HeaderCell(IContainer c)
        {
            return c.Border(1)
                .Background(Colors.Grey.Lighten3)
                .Padding(4)
                .AlignCenter()
                .DefaultTextStyle(x => x.SemiBold().FontSize(9));
        }

        private static IContainer RowCell(IContainer c)
        {
            return c.BorderLeft(1).BorderRight(1).BorderBottom(1)
                .Padding(4)
                .DefaultTextStyle(x => x.FontSize(9));
        }

        private static IContainer RowCellRight(IContainer c) => RowCell(c).AlignRight();

        private static IContainer TotalLabelCell(IContainer c)
        {
            return c.BorderTop(1).BorderLeft(1).BorderRight(1).BorderBottom(1)
                .Padding(6)
                .AlignRight()
                .DefaultTextStyle(x => x.SemiBold().FontSize(10));
        }

        private static IContainer TotalValueCell(IContainer c)
        {
            return c.BorderTop(1).BorderLeft(1).BorderRight(1).BorderBottom(1)
                .Padding(6)
                .AlignRight()
                .DefaultTextStyle(x => x.SemiBold().FontSize(10));
        }
    }
}
