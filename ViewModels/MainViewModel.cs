using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using ReGen.Extensions;
using ReGen.Generators;
using ReGen.Model;
using ReGen.Properties;
using Velopack;

namespace ReGen.ViewModels;

public class MainViewModel : BaseViewModel
{
    private string _acsn = Settings_Designer.Default.ACSN;
    private string _cmm = Settings_Designer.Default.CMM;
    private string _csvFilePath = string.Empty;
    private bool _isGenerateButtonEnabled = true;
    private bool _isRestrictedFieldEditEnabled;
    private bool _isUpdating;
    private DateTime _lastMaintenance = Settings_Designer.Default.LastMaintenance;
    private string _lockButtonContent = LockContent.Lock;
    private string _maintenanceCountDisplay = Settings_Designer.Default.MaintenanceCount.ToString();
    private string _serialNumber = Settings_Designer.Default.SerialNumber;
    private string _technicianName = Settings_Designer.Default.TechnicianName;

    private string _technicianStampDisplay = Settings_Designer.Default.TechnicianStamp.ToString();
    private Visibility _updateButtonVisibility = Visibility.Hidden;
    private double _updateProgress;
    private string _workOrder = Settings_Designer.Default.WorkOrder;

    public MainViewModel()
    {
        BrowseCsvCommand = new RelayCommand(BrowseCsvFile);
        GenerateReportCommand = new RelayCommand(GenerateReport);
        ExitAppCommand = new RelayCommand(_ => Application.Current.Shutdown());
        OnLockClickCommand = new RelayCommand(LockClickHandler);
        UpdateCommand = new RelayCommand(UpdateCommandHandler);

        const string repoUrl = "https://github.com/bercek71/ReGen/releases/latest/download";
        UpdateManager = new UpdateManager(repoUrl);

        CheckForUpdate();
    }

    private UpdateManager UpdateManager { get; }

    private int TechnicianStamp => int.TryParse(_technicianStampDisplay, out var result) ? result : int.MinValue;


    private int MaintenanceCount => int.TryParse(_maintenanceCountDisplay, out var result) ? result : int.MinValue;

    public bool IsRestrictedFieldEditEnabled
    {
        get => _isRestrictedFieldEditEnabled;
        set
        {
            if (value == _isRestrictedFieldEditEnabled) return;
            _isRestrictedFieldEditEnabled = value;
            OnPropertyChanged();
        }
    }

    public string TechnicianStampDisplay
    {
        get => _technicianStampDisplay;
        set
        {
            if (_technicianStampDisplay == value) return;
            _technicianStampDisplay = value;
            OnPropertyChanged();
            if (!int.TryParse(value, out var result)) return;
            Settings_Designer.Default.TechnicianStamp = result;
            Settings_Designer.Default.Save();
        }
    }


    public string TechnicianName
    {
        get => _technicianName;
        set
        {
            _technicianName = value;
            OnPropertyChanged();
            Settings_Designer.Default.TechnicianName = value;
            Settings_Designer.Default.Save();
        }
    }

    public string CsvFilePath
    {
        get => _csvFilePath;
        private set
        {
            _csvFilePath = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CsvFilePathDisplay));
        }
    }

    public string CsvFilePathDisplay
    {
        get
        {
            const int maxLength = 100;
            if (string.IsNullOrWhiteSpace(_csvFilePath)) return string.Empty;

            return _csvFilePath.Length > maxLength
                ? "..." + _csvFilePath[^maxLength..] // Show only the end
                : _csvFilePath;
        }
    }

    public ICommand BrowseCsvCommand { get; }
    public ICommand GenerateReportCommand { get; }
    public ICommand ExitAppCommand { get; }

    public string LockButtonContent
    {
        get => _lockButtonContent;
        set
        {
            if (value == _lockButtonContent) return;
            _lockButtonContent = value;
            OnPropertyChanged();
        }
    }

    public ICommand OnLockClickCommand { get; }

    public string Cmm
    {
        get => _cmm;
        set
        {
            if (value == _cmm) return;
            _cmm = value;
            OnPropertyChanged();
            Settings_Designer.Default.CMM = value;
            Settings_Designer.Default.Save();
        }
    }

    public string SerialNumber
    {
        get => _serialNumber;
        set
        {
            if (value == _serialNumber) return;
            _serialNumber = value;
            OnPropertyChanged();
            Settings_Designer.Default.SerialNumber = value;
            Settings_Designer.Default.Save();
        }
    }

    public string Acsn
    {
        get => _acsn;
        set
        {
            if (value == _acsn) return;
            _acsn = value;
            OnPropertyChanged();
            Settings_Designer.Default.ACSN = value;
            Settings_Designer.Default.Save();
        }
    }

    public DateTime LastMaintenance
    {
        get => _lastMaintenance;
        set
        {
            if (value.Equals(_lastMaintenance)) return;
            _lastMaintenance = value;
            OnPropertyChanged();
            Settings_Designer.Default.LastMaintenance = value;
            Settings_Designer.Default.Save();
        }
    }

    public string MaintenanceCountDisplay


    {
        get => _maintenanceCountDisplay;
        set
        {
            if (value == _maintenanceCountDisplay) return;
            _maintenanceCountDisplay = value;
            OnPropertyChanged();
            if (!int.TryParse(value, out var result)) return;
            Settings_Designer.Default.MaintenanceCount = result;
            Settings_Designer.Default.Save();
        }
    }

    public string WorkOrder
    {
        get => _workOrder;
        set
        {
            if (value == _workOrder) return;
            _workOrder = value;
            OnPropertyChanged();
            Settings_Designer.Default.WorkOrder = value;
            Settings_Designer.Default.Save();
        }
    }

    public bool IsGenerateButtonEnabled
    {
        get => _isGenerateButtonEnabled;
        set
        {
            if (value == _isGenerateButtonEnabled) return;
            _isGenerateButtonEnabled = value;
            OnPropertyChanged();
        }
    }

    public Visibility UpdateButtonVisibility
    {
        get => _updateButtonVisibility;
        set
        {
            if (value == _updateButtonVisibility) return;
            _updateButtonVisibility = value;
            OnPropertyChanged();
        }
    }

    public ICommand UpdateCommand { get; }

    public bool IsUpdating
    {
        get => _isUpdating;
        set
        {
            if (value == _isUpdating) return;
            _isUpdating = value;
            OnPropertyChanged();
        }
    }

    public double UpdateProgress
    {
        get => _updateProgress;
        set
        {
            if (value.Equals(_updateProgress)) return;
            _updateProgress = value;
            OnPropertyChanged();
        }
    }

    private async void CheckForUpdate()
    {
        try
        {
            var updateInfo = await UpdateManager.CheckForUpdatesAsync();
            if (updateInfo != null) UpdateButtonVisibility = Visibility.Visible;
        }
        catch (Exception)
        {
            // ignored
        }
    }

    private async void UpdateCommandHandler(object? _)
    {
        var updateInfo = await UpdateManager.CheckForUpdatesAsync();
        if (updateInfo == null) return;
        IsUpdating = true;
        //Show window with progress bar
        await UpdateManager.DownloadUpdatesAsync(updateInfo, i => { UpdateProgress = i; });
        var asset = UpdateManager.UpdatePendingRestart;
        IsUpdating = false;
        UpdateManager.ApplyUpdatesAndRestart(asset);
    }

    private void LockClickHandler(object? _)
    {
        if (LockButtonContent == LockContent.Lock)
        {
            LockButtonContent = LockContent.Unlock;
            IsRestrictedFieldEditEnabled = true;
        }
        else
        {
            LockButtonContent = LockContent.Lock;
            IsRestrictedFieldEditEnabled = false;
        }
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

        if (string.IsNullOrWhiteSpace(Acsn))
        {
            MessageBox.Show("Please enter acs number", "Missing Acs Number", MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(WorkOrder))
        {
            MessageBox.Show("Please enter work order", "Missing Work Order", MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(Cmm))
        {
            MessageBox.Show("Please enter CMN", "Missing CMN", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(SerialNumber))
        {
            MessageBox.Show("Please enter SerialNumber", "Missing Serial number", MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        IsGenerateButtonEnabled = false;


        PreviewReport(null!);

        Task.Run(async () =>
        {
            await Task.Delay(500);
            IsGenerateButtonEnabled = true;
        });
    }


    private void PreviewReport(object _)
    {
        if (!File.Exists(CsvFilePath))
            MessageBox.Show("CSV file does not exist.", "Missing File", MessageBoxButton.OK, MessageBoxImage.Warning);

        try
        {
            var records = CsvReaderHelper
                .ReadCsvFile(CsvFilePath)
                .ToList();

            var pdfData = new PdfData(records)
            {
                Cmm = Cmm,
                Ah = 1.51651,

                LastMaintenance = LastMaintenance,
                SerialNumber = SerialNumber,
                TechnicianName = TechnicianName,
                Acsn = Acsn,
                WorkOrder = WorkOrder,
                TechnicianStamp = TechnicianStamp == int.MinValue ? 0 : TechnicianStamp,
                MaintenanceCount = MaintenanceCount == int.MinValue ? 0 : MaintenanceCount,

                Cn = 31.21
            };

            var testNumber = Settings_Designer.Default.TestNumber;

            var chartPath = Path.Combine(TmpHelper.ChartsDirectoryPath,
                $"{testNumber}.png");

            PlotGenerator.GeneratePlot(records, chartPath);
            var reportDocument = new ReportDocument(chartPath, testNumber, pdfData);
            reportDocument.SaveFileAndShowDefault($"Test {testNumber}");

            Settings_Designer.Default.TestNumber++;
            Settings_Designer.Default.Save();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Csv file format error: " + ex.Message, "Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}