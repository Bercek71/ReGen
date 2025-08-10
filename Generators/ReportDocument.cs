using System.Globalization;
using System.IO;
using System.Windows.Controls;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ReGen.Model;

namespace ReGen.Generators;

public class ReportDocument : IDocument
{
    private string ChartImagePath { get; }

    private ulong TestNumber { get; }
    
    private PdfData Data {get;}
    
    public ReportDocument(string chartImagePath, ulong testNumber, PdfData data)
    {
        if (!File.Exists(chartImagePath))
        {
            throw new ArgumentException("Chart image file is invalid");
        }
        
        TestNumber = testNumber;
        ChartImagePath = chartImagePath;
        this.Data = data;
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
                        infoRow.RelativeItem().Text($"S.N. : {Data.SerialNumber}");
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
                                    labelCol.Item().Text(Data.Cmm));
                            });

                            infoCol.Item().Row(lastMaintenanceRow =>
                            {
                                var lastDate = Data.LastMaintenance;
                                
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
                    dischargeInfoCol.Item().Text("Discharge Constant Current 6.00A.").AlignCenter();
                    dischargeInfoCol.Item().Text("Threshold: 00:40:00 Vmin=5V.Tmin=10°C Tmax=45°C").AlignCenter();
                });
                

                // BODY
        col.Item().Row(row =>
        {
            // LEFT DATA TABLE
            row.ConstantItem(150).Column(left =>
            {
                left.Item().Border(1, Colors.Black).Padding(10).Column(workOrderRow =>
                {
                    workOrderRow.Item().Text("WORK ORDER NO:").Bold(); //.Text("1651651561");
                    workOrderRow.Item().PaddingBottom(10).Text(Data.WorkOrder);



                    workOrderRow.Item().Text("A/C S/N:    A/C REG:").Bold(); //.Text("03902 / VP - CAN");
                    workOrderRow.Item().PaddingBottom(10).Text(Data.Acsn);

                    workOrderRow.Item().Text("TECHNICIAN/STAMP:").Bold();
                    workOrderRow.Item().Text(
                        $"{Data.TechnicianName} / {Data.TechnicianStamp.ToString()}");
                });
                left.Item().Border(1, Colors.Black).Padding(10).Column(seriesInfo =>
                {
                    
                    seriesInfo.Item().PaddingBottom(10).Row(timeRow =>
                    {
                        timeRow.RelativeItem().Text("Time").Bold();
                        timeRow.RelativeItem().Text("0");
                        timeRow.RelativeItem().Text(Data.TestDuration.ToString("g", CultureInfo.InvariantCulture));
                    });
                    
                    seriesInfo.Item().PaddingBottom(10).Row(voltageRow =>
                    {
                        voltageRow.RelativeItem().Text("Voltage").Bold();
                        voltageRow.RelativeItem().Text(Data.StartVoltage.ToString("F3", CultureInfo.InvariantCulture));
                        voltageRow.RelativeItem().Text(Data.EndVoltage.ToString("F3", CultureInfo.InvariantCulture));
                    });

                    seriesInfo.Item().Row(ampRow =>
                    {
                        ampRow.RelativeItem().Text("Amp").Bold();
                        ampRow.RelativeItem().Text(Data.StartAmp.ToString("F3", CultureInfo.InvariantCulture));
                        ampRow.RelativeItem().Text(Data.EndAmp.ToString("F3", CultureInfo.InvariantCulture));
                    });
                    
                });
                left.Item().Border(1, Colors.Black).Padding(10).Column(seriesInfo =>
                {
                    seriesInfo.Item().Row(ahRow =>
                    {
                        ahRow.RelativeItem().Text("Ah.").Bold();
                        ahRow.RelativeItem().Text(Data.Ah.ToString("F3", CultureInfo.InvariantCulture));
                    });  
                    seriesInfo.Item().Row(cnRow =>
                    {
                        cnRow.RelativeItem().Text("%Cn").Bold();
                        cnRow.RelativeItem().Text(Data.Ah.ToString("F3", CultureInfo.InvariantCulture));
                    });  
                        
                });
            });
        
            // CHART CENTER
            row.RelativeItem().Image(ChartImagePath);
        
            // RIGHT OBSERVATION TABLE
            row.ConstantItem(250).Border(1, Colors.Black).AlignRight().Padding(5).Column(right =>
            {
                right.Item().AlignRight().Row(headerRow =>
                {
                    headerRow.RelativeItem();
                    headerRow.ConstantItem(50).AlignCenter().Text("PASS").Bold();
                    headerRow.ConstantItem(50).AlignCenter().Text("FAULT").Bold();
                });
                right.Item().PaddingBottom(10).Row(visualInspectionCol =>
                {
                    visualInspectionCol.RelativeItem().Text("1. Visual inspection");
                    visualInspectionCol.ConstantItem(50).AlignCenter().Text("\u2610"); // Empty checkbox
                    visualInspectionCol.ConstantItem(50).AlignCenter().Text("\u2610"); // Empty checkbox
                });
                
                right.Item().PaddingBottom(10).Row(insulationTest =>
                {
                    insulationTest.RelativeItem().Text("2. INSULATION TEST \n VALUE: \u2610 MΩ");
                    insulationTest.ConstantItem(50).AlignCenter().Text("\u2610"); // Empty checkbox
                    insulationTest.ConstantItem(50).AlignCenter().Text("\u2610"); // Empty checkbox
                });
                
                right.Item().PaddingBottom(10).Row(outputVoltageTest =>
                {
                    outputVoltageTest.RelativeItem().Text("3. OUTPUT VOLTAGE TEST \n VALUE: \u2610 V DC");
                    outputVoltageTest.ConstantItem(50).AlignCenter().Text("\u2610"); // Empty checkbox
                    outputVoltageTest.ConstantItem(50).AlignCenter().Text("\u2610"); // Empty checkbox
                });
                
                right.Item().Row(capacityTest =>
                {
                    capacityTest.RelativeItem().Text("4. CAPACITY TEST");
                });
            });
        });
           });
        });
    }
}