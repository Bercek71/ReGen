using QuestPDF.Infrastructure;
using ReGen.Extensions;

namespace ReGen;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    public App()
    {
        QuestPDF.Settings.License = LicenseType.Community;
        TmpHelper.InitializeTmpWorkSpace();
        TmpHelper.ClearChartsDirectory();
        TmpHelper.ClearMonthOldDocuments();
    }
}