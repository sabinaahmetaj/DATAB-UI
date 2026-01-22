using System.Linq;
using PROJEKTDB.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PROJEKTDB.Services
{
    public static class InvoicePdfService
    {
        public static byte[] Generate(Fature fature)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var rows = fature.Rresht?.OrderBy(r => r.RreId).ToList() ?? new();
            var total = rows.Sum(r => (decimal)r.RreSasi * r.RreCmim);

            var customerName = $"{fature.Person?.PerEm} {fature.Person?.PerMb}".Trim();
            if (string.IsNullOrWhiteSpace(customerName))
                customerName = "—";

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    // ================= HEADER =================
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("GALERIA ART").SemiBold().FontSize(18);
                            col.Item().Text("Faturë Shitjeje").FontSize(11).FontColor(Colors.Grey.Darken2);
                            col.Item().Text("Adresa: __________").FontColor(Colors.Grey.Darken2);
                            col.Item().Text("Tel: __________").FontColor(Colors.Grey.Darken2);
                        });

                        row.ConstantItem(220).AlignRight().Column(col =>
                        {
                            col.Item().Text("FATURË").SemiBold().FontSize(16);
                            col.Item().Text($"Nr: #{fature.FatId}");
                            col.Item().Text($"Data: {fature.FatDat:dd/MM/yyyy HH:mm}");
                        });
                    });

                    // ================= CONTENT =================
                    page.Content().PaddingVertical(15).Column(col =>
                    {
                        col.Spacing(12);

                        // -------- Customer + Details boxes --------
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(c =>
                            {
                                c.Spacing(3);
                                c.Item().Text("Klienti").SemiBold();
                                c.Item().Text(customerName);
                                c.Item().Text($"ID: {fature.PerId}").FontColor(Colors.Grey.Darken2);
                            });

                            row.ConstantItem(16);

                            row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(c =>
                            {
                                c.Spacing(3);
                                c.Item().Text("Detaje").SemiBold();
                                c.Item().Text("Pagesa: Cash / Card").FontColor(Colors.Grey.Darken2);
                                c.Item().Text("Status: Paid").FontColor(Colors.Grey.Darken2);
                            });
                        });

                        // -------- Items table --------
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(30);  // #
                                columns.RelativeColumn(5);   // Produkti
                                columns.RelativeColumn(2);   // Sasia
                                columns.RelativeColumn(2);   // Cmimi
                                columns.RelativeColumn(2);   // Nentotali
                            });

                            // Header row
                            table.Header(header =>
                            {
                                header.Cell().Element(HeaderCell).Text("#");
                                header.Cell().Element(HeaderCell).Text("Produkti");
                                header.Cell().Element(HeaderCell).AlignRight().Text("Sasia");
                                header.Cell().Element(HeaderCell).AlignRight().Text("Çmimi");
                                header.Cell().Element(HeaderCell).AlignRight().Text("Nëntotali");

                                static IContainer HeaderCell(IContainer container) =>
                                    container
                                        .PaddingVertical(7)
                                        .PaddingHorizontal(8)
                                        .Background(Colors.Grey.Lighten4)
                                        .BorderBottom(1)
                                        .BorderColor(Colors.Grey.Lighten2)
                                        .DefaultTextStyle(x => x.SemiBold());
                            });

                            // Body rows
                            foreach (var r in rows)
                            {
                                var produkti = r.Pikture?.PikTit ?? r.PikId;
                                var sasi = (int)r.RreSasi;
                                var cmim = r.RreCmim;
                                var nentotal = (decimal)r.RreSasi * r.RreCmim;

                                table.Cell().Element(BodyCell).Text(r.RreId.ToString());
                                table.Cell().Element(BodyCell).Text(produkti);
                                table.Cell().Element(BodyCell).AlignRight().Text(sasi.ToString());
                                table.Cell().Element(BodyCell).AlignRight().Text(cmim.ToString("0.00"));
                                table.Cell().Element(BodyCell).AlignRight().Text(nentotal.ToString("0.00"));

                                static IContainer BodyCell(IContainer container) =>
                                    container
                                        .PaddingVertical(6)
                                        .PaddingHorizontal(8)
                                        .BorderBottom(1)
                                        .BorderColor(Colors.Grey.Lighten3);
                            }

                            // Nëse s’ka rreshta
                            if (rows.Count == 0)
                            {
                                table.Cell().ColumnSpan(5).Padding(10)
                                    .Text("Nuk ka rreshta në këtë faturë.")
                                    .FontColor(Colors.Grey.Darken2);
                            }
                        });

                        // -------- Totals box --------
                        col.Item().AlignRight().Width(260)
                            .Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10)
                            .Column(t =>
                            {
                                t.Spacing(6);

                                t.Item().Row(r =>
                                {
                                    r.RelativeItem().Text("Nëntotal").FontColor(Colors.Grey.Darken2);
                                    r.ConstantItem(90).AlignRight().Text($"{total:0.00} €");
                                });

                                t.Item().Row(r =>
                                {
                                    r.RelativeItem().Text("TVSH (0%)").FontColor(Colors.Grey.Darken2);
                                    r.ConstantItem(90).AlignRight().Text("0.00 €");
                                });

                                t.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                                t.Item().Row(r =>
                                {
                                    r.RelativeItem().Text("TOTAL").SemiBold().FontSize(12);
                                    r.ConstantItem(90).AlignRight().Text($"{total:0.00} €").SemiBold().FontSize(12);
                                });
                            });

                        col.Item().Text("Faleminderit për blerjen — GALERIA ART")
                            .FontColor(Colors.Grey.Darken2);
                    });

                    // ================= FOOTER =================
                    page.Footer().AlignCenter()
                        .Text("GALERIA ART • Faturë e gjeneruar automatikisht")
                        .FontSize(9)
                        .FontColor(Colors.Grey.Darken2);
                });
            }).GeneratePdf();
        }
    }
}
