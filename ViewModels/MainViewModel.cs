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
    
    
    private int? MaintenanceCount
    {
        get
        {
            if (int.TryParse(_maintenanceCountDisplay, out var result))
            {
                return result;
            }

            return null;
        }
    }

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

    private string _technicianStampDisplay = Properties.Settings_Designer.Default.TechnicianStamp.ToString();
    private string _lockButtonContent = LockContent.Lock;
    private bool _isRestrictedFieldEditEnabled = false;
    private string _cmm = Properties.Settings_Designer.Default.CMM;
    private string _serialNumber = Properties.Settings_Designer.Default.SerialNumber;
    private string _acsn = Properties.Settings_Designer.Default.ACSN;
    private string _workOrder = Properties.Settings_Designer.Default.WorkOrder;
    private DateTime _lastMaintenance = Properties.Settings_Designer.Default.LastMaintenance;
    private string _maintenanceCountDisplay = Properties.Settings_Designer.Default.MaintenanceCount.ToString();

    public string TechnicianStampDisplay
    {
        get => _technicianStampDisplay;
        set
        {
            if (_technicianStampDisplay == value) return;
            _technicianStampDisplay = value;
            OnPropertyChanged();
            if (!int.TryParse(value, out var result)) return;
            Properties.Settings_Designer.Default.TechnicianStamp = result;
            Properties.Settings_Designer.Default.Save();
        }
    }



    public string TechnicianName
    {
        get => _technicianName;
        set
        {
            _technicianName = value; OnPropertyChanged();
            Properties.Settings_Designer.Default.TechnicianName = value;
            Properties.Settings_Designer.Default.Save();
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
            const int maxLength = 100;
            if (string.IsNullOrWhiteSpace(_csvFilePath)) return string.Empty;

            return _csvFilePath.Length > maxLength
                ? "..." + _csvFilePath[^maxLength..]  // Show only the end
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
            Properties.Settings_Designer.Default.CMM = value;
            Properties.Settings_Designer.Default.Save();
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
            Properties.Settings_Designer.Default.SerialNumber = value;
            Properties.Settings_Designer.Default.Save();
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
            Properties.Settings_Designer.Default.ACSN = value;
            Properties.Settings_Designer.Default.Save();
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
            Properties.Settings_Designer.Default.LastMaintenance = value;
            Properties.Settings_Designer.Default.Save();
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
            Properties.Settings_Designer.Default.MaintenanceCount = result;
            Properties.Settings_Designer.Default.Save();
            
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
            Properties.Settings_Designer.Default.WorkOrder = value;
            Properties.Settings_Designer.Default.Save();
        }
    }

    public MainViewModel()
    {
        BrowseCsvCommand = new RelayCommand(BrowseCsvFile);
        GenerateReportCommand = new RelayCommand(GenerateReport);
        ExitAppCommand = new RelayCommand(ExitApp);
        OnLockClickCommand = new RelayCommand(LockClickHandler);
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

    private void ExitApp(object? _)
    {
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