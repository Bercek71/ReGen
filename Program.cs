using System.IO;
using System.Windows;
using QuestPDF.Infrastructure;
using ReGen.Extensions;
using ReGen.Views;
using Velopack;

namespace ReGen;

public static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        try {
            // It's important to Run() the VelopackApp as early as possible in app startup.
            VelopackApp.Build()
                .OnFirstRun((v) =>
                {
                })
                .Run();
            
            // We can now launch the WPF application as normal.
            
            QuestPDF.Settings.License = LicenseType.Community;
            TmpHelper.InitializeTmpWorkSpace();
            TmpHelper.ClearChartsDirectory();
            TmpHelper.ClearMonthOldDocuments();
            var app = new App();

            var mainWindow = new MainWindow();
            app.Run(mainWindow);

        } catch (Exception ex) {
            MessageBox.Show("Unhandled exception: " + ex.ToString());
        }
    }
}