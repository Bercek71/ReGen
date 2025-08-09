using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ReGen.Generators;

public class ReportDocument : IDocument
{
    private string ChartImagePath { get; }

    private ulong TestNumber { get; }
    
    public ReportDocument(string csvDataPath, ulong testNumber)
    {
        if (!File.Exists(csvDataPath))
        {
            throw new ArgumentException("CSV file path does not exist");
        }
        
        TestNumber = testNumber;
        ChartImagePath = Path.Combine(Path.GetTempPath(), "Reports", "Charts");
        
        // PlotGenerator.GeneratePlot(csvDataPath);
        // chartImagePath = chartPath;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4.Landscape());
            page.Margin(20);
            page.DefaultTextStyle(x => x.FontSize(10));

            page.Content().Column(col =>
            {
                // HEADER
                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(left =>
                    {
                        left.Item().Text("Result : Discharge").FontSize(14).Bold();
                        left.Item().Text($"Test: {TestNumber.ToString()}  Date: 06/13/2025   Channel: 1");
                        left.Item().Text("P.N.: Saft 2758   S.N.: 202207548");
                        left.Item().Text("Discharge Constant Current 23.000A.");
                        left.Item().Text("Threshold : 01:15:00 Vmin=20.000V Tmin=15.0°C Tmax=57.0°C");
                    });

                    row.ConstantItem(160).AlignRight().Column(right =>
                    {
                        // right.Item().Image("logo.png", ImageScaling.FitWidth);
                        right.Item().Text("JOB AIR TECHNIC");
                        right.Item().Text("OSTRAVA AIRPORT, CZECH REPUBLIC");
                        right.Item().Text("CZ.145.0054");
                    });
                });

                col.Item().PaddingVertical(5).LineHorizontal(1);

                // BODY
                col.Item().Row(row =>
                {
                    // LEFT DATA TABLE
                    row.ConstantItem(200).Column(left =>
                    {
                        left.Item().Text("WORK ORDER NO:").Bold();//.Text("1651651561");
                        left.Item().Text("A/C S/N:    A/C REG:");//.Text("03902 / VP - CAN");
                        left.Item().Text("TECHNICIAN/STAMP");//.Text("BEREGHÁZY / 1172");

                        left.Item().Text("Cycle: 1   Step: 1");
                        left.Item().Text("Sequence: CAPACITY TEST");
                        left.Item().Text("File: Step_1_1_1");

                        left.Item().PaddingTop(5).Text("Time: 01:01:05");
                        left.Item().Text("Volts: 29.044  → 23.399");
                        left.Item().Text("Amp: 0.0  → 23.0");
                        left.Item().Text("°C: 28.8  → 39.3");
                        left.Item().Text("Ah: 23.381");
                        left.Item().Text("%Cn: 101.658");
                    });

                    // CHART CENTER
                    // row.RelativeItem().Image(ChartImagePath);

                    // RIGHT OBSERVATION TABLE
                    row.ConstantItem(200).Column(right =>
                    {
                        right.Item().Text("Observation").Bold().Underline();
                        right.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(80);
                                c.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Time").Bold();
                                header.Cell().Text("Code").Bold();
                            });

                            table.Cell().Text("00:00:01");
                            table.Cell().Text("A_Cell");

                            table.Cell().Text("00:55:00");
                            table.Cell().Text("A_Time");

                            table.Cell().Text("01:01:06");
                            table.Cell().Text("STOP_OP");
                        });
                    });
                });
            });
        });
    }
}