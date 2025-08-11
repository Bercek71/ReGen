namespace ReGen.Model;

public class PdfData
{
    public PdfData()
    {
    }

    public PdfData(List<CsvRecord> records)
    {
        var minTimeStep = records.Min(record => record.TotalTime);
        var maxTimeStep = records.Max(record => record.TotalTime);
        StartVoltage = records.FirstOrDefault(rec => rec.TotalTime == minTimeStep)?.V ?? 0;
        EndVoltage = records.FirstOrDefault(rec => rec.TotalTime == maxTimeStep)?.V ?? 0;

        StartAmp = records.FirstOrDefault(rec => rec.TotalTime == minTimeStep)?.I ?? 0;
        EndAmp = records.FirstOrDefault(rec => rec.TotalTime == maxTimeStep)?.I ?? 0;
        TestDuration = maxTimeStep - minTimeStep;

        Ah = records.FirstOrDefault(rec => rec.TotalTime == maxTimeStep)?.Ah ?? 0;

        Cn = Ah * 100 / 5;

    }

    public string SerialNumber { get; init; } = string.Empty;
    public string TechnicianName { get; init; } = string.Empty;
    public string Acsn { get; init; } = string.Empty;
    public string WorkOrder { get; init; } = string.Empty;
    public int TechnicianStamp { get; init; } = 0;
    public DateTime LastMaintenance { get; init; } = DateTime.MinValue;
    public string Cmm { get; init; } = string.Empty;
    public int MaintenanceCount { get; init; } = 0;
    public TimeSpan TestDuration { get; init; } = TimeSpan.Zero;
    public double StartVoltage { get; init; }
    public double EndVoltage { get; init; }
    public double StartAmp { get; init; }
    public double EndAmp { get; init; }
    public double Ah { get; init; } = 0;
    public double Cn { get; init; } = 0;

    public string BatteryPn { get; init; } = string.Empty;
    public string Amdt { get; init; } = string.Empty;
}