namespace ReGen.Model;

public class RawCsvRecord
{
    public string StepTime { get; set; } = null!; // hh:mm:ss
    public string V { get; set; } = null!; // e.g. "+5.9752"
    public string I { get; set; } = null!;
    public string W { get; set; } = null!;

    public string Ah { get; set; } = null!;
    public string Wh { get; set; } = null!;
    public string TotalTime { get; set; } = null!;
    public string FunctionName { get; set; } = null!;
    public string Result { get; set; } = null!;
}