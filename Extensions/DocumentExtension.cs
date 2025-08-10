using System.Diagnostics;
using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace ReGen.Extensions;

public static class DocumentExtension
{
    public static void SaveFileAndShowDefault(this IDocument document, string fileName)
    {
        var directory = TmpHelper.DocumentDirectoryPath;

        var path = Path.Combine(directory, $"{Path.GetFileNameWithoutExtension(fileName)}.pdf");

        if (!Directory.Exists(directory)) throw new InvalidOperationException("Document directory doesn't exist");

        var bytes = document.GeneratePdf();
        if (!File.Exists(path)) File.Create(path).Close();
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