using System.IO;
using System.Windows.Controls;
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
                col.Item().Border(1, Colors.Black).Padding(10).Row(row =>
                {
                    row.RelativeItem().AlignLeft().Column(left =>
                    {
                        left.Item().Text("LOGO").Bold();    
                    });
                    
                    row.RelativeItem().Column(center =>
                    {
                        center.Item().Text("Shop Report").FontSize(24).Bold();
                    });

                    row.ConstantItem(200).AlignRight().Column(right =>
                    {
                        // right.Item().Image("logo.png", ImageScaling.FitWidth);
                        right.Item().Text("JOB AIR TECHNIC");
                        right.Item().Text("OSTRAVA AIRPORT, CZECH REPUBLIC");
                        right.Item().Text("CZ.145.0054");
                    });
                });
                
                col.Item().Row(row =>
                {
                    row.ConstantItem(150).Border(1, Colors.Black).Row(row1 =>
                    {
                        row1.ConstantItem(150).Padding(5).Column(infoCol =>
                        {
                            var dateNow = DateTime.Now;
                            infoCol.Item().Row(testNumeberRow =>
                            {
                                testNumeberRow.RelativeItem().Column(labelCol => labelCol.Item().Text("Test:").ExtraBold());
                                testNumeberRow.RelativeItem().Column(labelCol => labelCol.Item().Text($"{TestNumber}"));
                            });
                            infoCol.Item().Row(dateRow =>
                            {
                                dateRow.RelativeItem().Column(labelCol => labelCol.Item().Text("Date:").ExtraBold());
                                dateRow.RelativeItem().Column(labelCol => labelCol.Item().Text($"{dateNow.Day}/{dateNow.Month}/{dateNow.Year}"));
                            });
                            
                        });
                    });
                    row.RelativeItem().Border(1, Colors.Black).BorderLeft(0).Padding(5).AlignCenter().Row(infoRow =>
                    {
                        infoRow.RelativeItem().Text("P.N. : 3214-31");
                        infoRow.RelativeItem().Text($"S.N. : {Properties.Settings_Designer.Default.SerialNumber}");
                        infoRow.RelativeItem().AlignLeft().Text("AMDT: E");
                    });
                    row.ConstantItem(190).AlignRight().Border(1, Colors.Black).BorderLeft(0).Padding(5).Row(infoRow =>
                    {
                        infoRow.RelativeItem().Column(infoCol =>
                        {
                            infoCol.Item().Row(cmmRow =>
                            {
                                cmmRow.RelativeItem().Column(labelCol => labelCol.Item().Text("C.M.M. :").ExtraBold());
                                cmmRow.RelativeItem().Column(labelCol =>
                                    labelCol.Item().Text(Properties.Settings_Designer.Default.CMM));
                            });

                            infoCol.Item().Row(lastMaintenanceRow =>
                            {
                                var lastDate = Properties.Settings_Designer.Default.LastMaintenance;
                                
                                lastMaintenanceRow.RelativeItem()
                                    .Column(labelCol => labelCol.Item().Text("Last maintenance:"));
                                
                                lastMaintenanceRow.RelativeItem().Column(labelCol =>
                                    labelCol.Item().Text($"{lastDate.Month}/{lastDate.Day}/{lastDate.Year}"));
                            });
                            
                            infoCol.Item().Row(maintenanceCountRow =>
                            {
                                maintenanceCountRow.RelativeItem().Column(labelCol => labelCol.Item().Text("Maintenance count:"));
                                maintenanceCountRow.RelativeItem().Column(labelCol => labelCol.Item().Text(Properties.Settings_Designer.Default.MaintenanceCount.ToString()));
                            });
                        });
                    });

                });
                col.Item().Border(1, Colors.Black).AlignCenter().Padding(5).Column(dischargeInfoCol =>
                {
                    dischargeInfoCol.Item().Text("Discharge Constant Current 5.00A.").AlignCenter();
                    dischargeInfoCol.Item().Text("Threshold: 00:40:00 Vmin=5V.Tmin=10°C Tmax=45°C").AlignCenter();
                });
                

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