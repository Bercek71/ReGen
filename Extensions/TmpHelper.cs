using System.IO;

namespace ReGen.Extensions;

public static class TmpHelper
{
    public static string ChartsDirectoryPath { get; } = Path.Combine(Path.GetTempPath(), "ReGenCharts");
    public static string DocumentDirectoryPath { get; } = Path.Combine(Path.GetTempPath(), "ReGenDocuments");

    public static void ClearChartsDirectory()
    {
        if (!Directory.Exists(ChartsDirectoryPath)) return;
        //Remove everything in directory but keep directory
        Directory.Delete(ChartsDirectoryPath, true);
        Directory.CreateDirectory(ChartsDirectoryPath);
    }

    public static void ClearMonthOldDocuments()
    {
        if (!Directory.Exists(DocumentDirectoryPath)) return;
        var files = Directory.GetFiles(DocumentDirectoryPath);
        foreach (var file in files)
        {
            var timeSpan = DateTime.Now - File.GetLastWriteTimeUtc(file);
            if (timeSpan > TimeSpan.FromDays(30)) File.Delete(file);
        }
    }

    public static void InitializeTmpWorkSpace()
    {
        if (!Directory.Exists(ChartsDirectoryPath)) Directory.CreateDirectory(ChartsDirectoryPath);

        if (!Directory.Exists(DocumentDirectoryPath)) Directory.CreateDirectory(DocumentDirectoryPath);
    }
}