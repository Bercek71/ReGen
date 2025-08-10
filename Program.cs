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
            
            MigrateSettingsFromPreviousVersion();
            
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
    private static void MigrateSettingsFromPreviousVersion()
    {
        try
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var companyFolder = Path.Combine(localAppData, "ReGen");
            var appFolders = Directory.GetDirectories(companyFolder, "ReGen*")
                .OrderByDescending(d => Directory.GetCreationTime(d))
                .ToArray();

            if (appFolders.Any())
            {
                var previousVersionFolder = appFolders.First();
                
                var previousConfigPath = Path.Combine(previousVersionFolder, "user.config");

                // if (File.Exists(previousConfigPath) && !File.Exists(currentConfigath))
                // {
                //     Directory.CreateDirectory(Path.GetDirectoryName(currentConfigPath));
                //     File.Copy(previousConfigPath, currentConfigPath);
                //
                //     // Optionally clean up old config
                //     // File.Delete(previousConfigPath);
                // }
            }
        }
        catch (Exception ex)
        {
            // Log but don't crash
            System.Diagnostics.Debug.WriteLine($"Settings migration failed: {ex.Message}");
        }
    
}


}