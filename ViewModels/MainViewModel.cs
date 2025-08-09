using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;

using System.Windows.Input;
using Microsoft.Win32;
using QuestPDF.Fluent;
using ReGen.Extensions;
using ReGen.Generators;
using ReGen.Validation;

namespace ReGen.ViewModels;

public class MainViewModel : BaseViewModel
{
    private string _technicianName = Properties.Settings_Designer.Default.TechnicianName;
    private string _csvFilePath = string.Empty;
    
    private int? TechnicianStamp
    {
        get
        {

            if (int.TryParse(_technicianStampDisplay, out var result))
            {
                return result;
            }

            return null;
        }

    }
    
    private string _technicianStampDisplay = Properties.Settings_Designer.Default.TechnicianStamp.ToString();

    public string TechnicianStampDisplay
    {
        get => _technicianStampDisplay;
        set
        {
            if (_technicianStampDisplay == value) return;
            _technicianStampDisplay = value;
            OnPropertyChanged();
        }
    }



    public string TechnicianName
    {
        get => _technicianName;
        set
        {
            _technicianName = value; OnPropertyChanged();
        }
    }

    public string CsvFilePath
    {
        get => _csvFilePath;
        private set { _csvFilePath = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CsvFilePathDisplay));
        }
    }
    
    public string CsvFilePathDisplay
    {
        get
        {
            const int maxLength = 30;
            if (string.IsNullOrWhiteSpace(_csvFilePath)) return string.Empty;

            return _csvFilePath.Length > maxLength
                ? "..." + _csvFilePath[^maxLength..]  // Show only the end
                : _csvFilePath;
        }
    }

    public ICommand BrowseCsvCommand { get; }
    public ICommand GenerateReportCommand { get; }
    public ICommand ExitAppCommand { get; }
    
    public MainViewModel()
    {
        BrowseCsvCommand = new RelayCommand(BrowseCsvFile);
        GenerateReportCommand = new RelayCommand(GenerateReport);
        ExitAppCommand = new RelayCommand(ExitApp);
    }

    private void ExitApp(object? _)
    {
        Properties.Settings_Designer.Default.TechnicianName = TechnicianName;
        if (TechnicianStamp != null) Properties.Settings_Designer.Default.TechnicianStamp = TechnicianStamp.Value;
        Properties.Settings_Designer.Default.Save();
        Application.Current.Shutdown();
    }

    private void BrowseCsvFile(object? _)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "CSV files (*.csv)|*.csv",
            Title = "Select CSV file"
        };

        if (dialog.ShowDialog() == true)
            CsvFilePath = dialog.FileName;
    }

    private void GenerateReport(object? _)
    {
        if (string.IsNullOrWhiteSpace(CsvFilePath))
        {
            MessageBox.Show("Please select a CSV file.", "Missing File", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(TechnicianName))
        {
            MessageBox.Show("Please enter your name.", "Missing Name", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!TechnicianStamp.HasValue)
        {
            MessageBox.Show("Please enter your support stamp.", "Missing Stamp", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        PreviewReport(null!);
    }
    
    private void PreviewReport(object _)
    {
        if (!File.Exists(CsvFilePath))
        {
            MessageBox.Show("CSV file does not exist.", "Missing File", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        var reportDocument = new ReportDocument(CsvFilePath, Properties.Settings_Designer.Default.TestNumber);
        reportDocument.SaveFileAndShowDefault($"Test - {Properties.Settings_Designer.Default.TestNumber.ToString()}");
        Properties.Settings_Designer.Default.TestNumber++;
        Properties.Settings_Designer.Default.Save();
    }
}