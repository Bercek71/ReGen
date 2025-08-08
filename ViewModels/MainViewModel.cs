using System.ComponentModel;
using System.Windows;

using System.Windows.Input;
using Microsoft.Win32;
using QuestPDF.Fluent;
using ReGen.Extensions;
using static ReGen.Generators.PdfGenerator;

namespace ReGen.ViewModels;

public class MainViewModel : BaseViewModel
{
    private string _userName = string.Empty;
    private string _csvFilePath = string.Empty;

    public string UserName
    {
        get => _userName;
        set { _userName = value; OnPropertyChanged(); }
    }

    public string CsvFilePath
    {
        get => _csvFilePath;
        set { _csvFilePath = value;
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
        ExitAppCommand = new RelayCommand(_ => Application.Current.Shutdown());
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

        if (string.IsNullOrWhiteSpace(UserName))
        {
            MessageBox.Show("Please enter your name.", "Missing Name", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        PreviewReport(null!);
    }
    
    private void PreviewReport(object _)
    {
        var doc = GeneratePdfReport(UserName, CsvFilePath);
        doc.GeneratePdfAndShow();
    }
}