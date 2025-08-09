using System.Diagnostics;
using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace ReGen.Extensions;

public static class DocumentExtension
{
    public static void SaveFileAndShowDefault(this IDocument document, string fileName)
    {

        var directory = Path.Combine(Path.GetTempPath(), "ReGenDocuments");
        
        var path = Path.Combine(directory, $"{Path.GetFileNameWithoutExtension(fileName)}.pdf");

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var bytes = document.GeneratePdf();
        if (!File.Exists(path))
        {
            File.Create(path).Close();
        }
        File.WriteAllBytes(path, bytes);
        var process = new Process
        {
            StartInfo = new ProcessStartInfo(path)
            {
                UseShellExecute = true
            }
        };

        process.Start();
        process.WaitForExit();
        
    }
    
}