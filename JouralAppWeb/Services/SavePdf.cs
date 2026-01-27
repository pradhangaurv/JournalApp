using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using JouralAppWeb.Database;
using System.Text.RegularExpressions;

namespace JouralAppWeb.Services
{
    public class SavePdf : IDocument
    {
        private readonly List<JournalEntry> _entries;

        public SavePdf(List<JournalEntry> entries)
        {
            _entries = entries;
        }

        public DocumentMetadata GetMetadata()
        {
            return new DocumentMetadata
            {
                Title = "Journal Entries",
                Author = "Journal App"
            };
        }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(11));

                // ===== HEADER =====
                page.Header()
                    .AlignCenter()
                    .Text("Journal Entries")
                    .FontSize(20)
                    .Bold();

                // ===== CONTENT =====
                page.Content().Column(column =>
                {
                    foreach (var entry in _entries)
                    {
                        column.Item()
                            .PaddingVertical(10)
                            .BorderBottom(1)
                            .Column(e =>
                            {
                                e.Spacing(5);

                                // Date + Mood
                                e.Item().Row(row =>
                                {
                                    row.RelativeItem().Text(entry.EntryDate.ToString("dd MMM yyyy"))
                                        .Bold();

                                    row.ConstantItem(120)
                                        .AlignRight()
                                        .Text(entry.PrimaryMood)
                                        .Bold();
                                });

                                // Category
                                if (!string.IsNullOrWhiteSpace(entry.Category))
                                {
                                    e.Item().Text($"Category: {entry.Category}")
                                        .Italic()
                                        .FontSize(10);
                                }

                                // Tags
                                if (!string.IsNullOrWhiteSpace(entry.Tags))
                                {
                                    e.Item().Text($"Tags: {entry.Tags}")
                                        .FontSize(10);
                                }

                                // Content (HTML stripped)
                                e.Item().Text(StripHtml(entry.Content))
                                    .FontSize(11);
                            });
                    }
                });

                // ===== FOOTER =====
                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Generated on ");
                        text.Span(DateTime.Now.ToString("dd MMM yyyy"));
                    });
            });
        }

        private static string StripHtml(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            return Regex.Replace(input, "<.*?>", string.Empty);
        }
    }
}
