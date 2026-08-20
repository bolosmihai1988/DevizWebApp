using DevizWebApp.Models;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;

namespace DevizWebApp.Models
{
    public class FacturaDocument : IDocument
    {
        private readonly Factura _f;

        public FacturaDocument(Factura factura)
        {
            _f = factura;
        }

        public DocumentMetadata GetMetadata() =>
            DocumentMetadata.Default;

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

            // ==========================
            // TOTALURI FACTURA
            // ==========================

            decimal totalFaraTVA =
                _f.Items.Sum(x => x.ValoareFaraTVA);

            decimal totalTVA =
                _f.Items.Sum(x => x.TotalTVA);

            decimal totalGeneral =
                _f.Items.Sum(x => x.ValoareCuTVA);

            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);

                page.DefaultTextStyle(x =>
                    x.FontSize(10)
                     .FontFamily("Calibri"));

                // ==========================
                // HEADER
                // ==========================

                page.Header().Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        // FURNIZOR
                        row.RelativeItem()
                           .Border(1)
                           .Padding(8)
                           .Column(c =>
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

                        // CUMPARATOR
                        row.RelativeItem()
                           .Border(1)
                           .Padding(8)
                           .Column(c =>
                           {
                               c.Item().Text("Cumpărător:").SemiBold();
                               c.Item().Text(_f.ClientNume ?? "").SemiBold();

                               if (!string.IsNullOrWhiteSpace(_f.ClientCUI))
                                   c.Item().Text($"C.I.F.: {_f.ClientCUI}");

                               if (!string.IsNullOrWhiteSpace(_f.ClientAdresa))
                                   c.Item().Text($"Sediu: {_f.ClientAdresa}");
                           });
                    });

                    col.Item()
                       .PaddingTop(10)
                       .AlignCenter()
                       .Text("FACTURĂ")
                       .FontSize(16)
                       .SemiBold();

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

                    col.Item()
                       .PaddingTop(10)
                       .LineHorizontal(1);
                });

                // ==========================
                // CONTINUT
                // ==========================

                page.Content()
                    .PaddingTop(10)
                    .Column(col =>
                    {
                        col.Item().Table(t =>
                        {
                            // ==========================
                            // COLOANE
                            // ==========================

                            t.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(30);  // Nr.
                                c.RelativeColumn();    // Denumire
                                c.ConstantColumn(38);  // UM
                                c.ConstantColumn(45);  // Cantitate
                                c.ConstantColumn(75);  // Pret unitar fara TVA
                                c.ConstantColumn(70);  // Valoare
                                c.ConstantColumn(65);  // TVA
                            });

                            // ==========================
                            // HEADER TABEL
                            // ==========================

                            t.Header(h =>
                            {
                                h.Cell()
                                 .Element(HeaderCell)
                                 .Text("Nr.\ncrt.");

                                h.Cell()
                                 .Element(HeaderCell)
                                 .Text("Denumirea produselor sau serviciilor");

                                h.Cell()
                                 .Element(HeaderCell)
                                 .Text("U.M.");

                                h.Cell()
                                 .Element(HeaderCell)
                                 .Text("Cant.");

                                h.Cell()
                                 .Element(HeaderCell)
                                 .Text("Preț unitar\nfără TVA");

                                h.Cell()
                                 .Element(HeaderCell)
                                 .Text("Valoare\nRON");

                                h.Cell()
                                 .Element(HeaderCell)
                                 .Text("TVA 21%\nRON");
                            });

                            // ==========================
                            // PRODUSE / SERVICII
                            // ==========================

                            int nr = 1;

                            foreach (var it in _f.Items)
                            {
                                t.Cell()
                                 .Element(RowCellRight)
                                 .Text(nr.ToString());

                                t.Cell()
                                 .Element(RowCell)
                                 .Text(it.Denumire ?? "");

                                t.Cell()
                                 .Element(RowCellRight)
                                 .Text(it.UM ?? "");

                                t.Cell()
                                 .Element(RowCellRight)
                                 .Text($"{it.Cantitate:0.##}");

                                // Pret unitar fara TVA
                                t.Cell()
                                 .Element(RowCellRight)
                                 .Text($"{it.PretUnitarFaraTVA:F2}");

                                // Valoare fara TVA
                                t.Cell()
                                 .Element(RowCellRight)
                                 .Text($"{it.ValoareFaraTVA:F2}");

                                // TVA total pe linie
                                t.Cell()
                                 .Element(RowCellRight)
                                 .Text($"{it.TotalTVA:F2}");

                                nr++;
                            }

                            // ==========================
                            // TOTAL TABEL
                            // ==========================

                            t.Cell()
                             .ColumnSpan(4)
                             .Element(TotalLabelCell)
                             .Text("TOTAL");

                            // Coloana Pret unitar ramane goala
                            t.Cell()
                             .Element(TotalValueCell)
                             .Text("");

                            // Total valoare fara TVA
                            t.Cell()
                             .Element(TotalValueCell)
                             .Text($"{totalFaraTVA:F2}");

                            // Total TVA
                            t.Cell()
                             .Element(TotalValueCell)
                             .Text($"{totalTVA:F2}");
                        });

                        // ==========================
                        // TOTALURI GENERALE
                        // ==========================

                        col.Item()
                           .PaddingTop(12)
                           .AlignRight()
                           .Column(totalCol =>
                           {
                               totalCol.Item()
                                       .Text($"TOTAL FĂRĂ TVA: {totalFaraTVA:F2} RON")
                                       .FontSize(10)
                                       .SemiBold();

                               totalCol.Item()
                                       .Text($"TVA 21%: {totalTVA:F2} RON")
                                       .FontSize(10)
                                       .SemiBold();

                               totalCol.Item()
                                       .PaddingTop(3)
                                       .Text($"TOTAL GENERAL: {totalGeneral:F2} RON")
                                       .FontSize(13)
                                       .SemiBold()
                                       .FontColor(Colors.Blue.Darken2);
                           });

                        // ==========================
                        // SEMNATURI
                        // ==========================

                        col.Item()
                           .PaddingTop(20)
                           .Row(r =>
                           {
                               r.RelativeItem()
                                .Text(
                                    "Semnătura și ștampila furnizorului: ____________________")
                                .FontSize(9);

                               r.RelativeItem()
                                .AlignRight()
                                .Text(
                                    "Semnătura de primire: ____________________")
                                .FontSize(9);
                           });
                    });

                // Nu mai afisam textul
                // "TVA 21% inclus in preturile introduse"
                // in partea de jos a facturii.
            });
        }

        // ==========================
        // STIL HEADER
        // ==========================

        private static IContainer HeaderCell(IContainer c)
        {
            return c.Border(1)
                .Background(Colors.Grey.Lighten3)
                .Padding(4)
                .AlignCenter()
                .DefaultTextStyle(x =>
                    x.SemiBold()
                     .FontSize(8));
        }

        // ==========================
        // STIL RAND
        // ==========================

        private static IContainer RowCell(IContainer c)
        {
            return c.BorderLeft(1)
                .BorderRight(1)
                .BorderBottom(1)
                .Padding(4)
                .DefaultTextStyle(x =>
                    x.FontSize(9));
        }

        private static IContainer RowCellRight(IContainer c)
        {
            return RowCell(c).AlignRight();
        }

        // ==========================
        // STIL TOTAL
        // ==========================

        private static IContainer TotalLabelCell(IContainer c)
        {
            return c.BorderTop(1)
                .BorderLeft(1)
                .BorderRight(1)
                .BorderBottom(1)
                .Background(Colors.Grey.Lighten3)
                .Padding(6)
                .AlignRight()
                .DefaultTextStyle(x =>
                    x.SemiBold()
                     .FontSize(10));
        }

        private static IContainer TotalValueCell(IContainer c)
        {
            return c.BorderTop(1)
                .BorderLeft(1)
                .BorderRight(1)
                .BorderBottom(1)
                .Background(Colors.Grey.Lighten3)
                .Padding(6)
                .AlignRight()
                .DefaultTextStyle(x =>
                    x.SemiBold()
                     .FontSize(10));
        }
    }
}