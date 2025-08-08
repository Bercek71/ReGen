using System.IO;
using System.Windows;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ReGen;

public class PdfGenerator
{
    private static Image GeneratePlot(string csvFilePath)
    {
        throw new NotImplementedException();
        // return new Image(null);
    }
    public static IDocument GeneratePdfReport(string userName, string csvFilePath)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(50);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(14));

                page.Header()
                    .Text("CSV Report")
                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                page.Content()
                    .PaddingVertical(10)
                    .Column(column =>
                    {
                        column.Spacing(10);

                        column.Item().Text($"User Name: {userName}");
                        column.Item().Text($"CSV File Path:");
                        column.Item().Text(csvFilePath).FontSize(12).FontColor(Colors.Grey.Darken1);
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Generated on ");
                        x.Span(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).SemiBold();
                    });
            });
        });
    }
}