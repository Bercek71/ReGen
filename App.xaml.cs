using QuestPDF.Infrastructure;

namespace ReGen;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    public App()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }
}