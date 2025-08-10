namespace ReGen.Model;

public class PdfData
{
    public string SerialNumber { get; init; } = string.Empty;
    public string TechnicianName  { get; init; } = string.Empty;
    public string Acsn { get; init; } = string.Empty;
    public string WorkOrder { get; init; } = string.Empty;
    public int TechnicianStamp { get; init; } = 0;
    public DateTime LastMaintenance { get; init; } = DateTime.MinValue;
    public string Cmm { get; init; } = string.Empty;
    public int MaintenanceCount { get; init; } = 0;
    public TimeSpan TestDuration { get; init; } = TimeSpan.Zero;
    public double StartVoltage { get; init; } = 0;
    public double EndVoltage { get; init; } = 0;
    public double StartAmp  { get; init; } = 0;
    public double EndAmp { get; init; } = 0;
    public double Ah { get; init; } = 0;
    public double Cn { get; init; } = 0;
}