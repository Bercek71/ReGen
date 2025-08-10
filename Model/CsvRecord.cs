using System.Globalization;

namespace ReGen.Model;

public class CsvRecord(RawCsvRecord rawRecord)
{
    public TimeSpan StepTime { get; } =
        TimeSpan.ParseExact(rawRecord.StepTime, @"hh\:mm\:ss", CultureInfo.InvariantCulture);

    public double V { get; set; } = double.Parse(rawRecord.V.Replace("+", ""), CultureInfo.InvariantCulture);
    public double I { get; set; } = double.Parse(rawRecord.I.Replace("+", ""), CultureInfo.InvariantCulture);
    public double W { get; set; } = double.Parse(rawRecord.W.Replace("+", ""), CultureInfo.InvariantCulture);
    public double Ah { get; set; } = double.Parse(rawRecord.Ah.Replace("+", ""), CultureInfo.InvariantCulture);
    public double Wh { get; set; } = double.Parse(rawRecord.Wh.Replace("+", ""), CultureInfo.InvariantCulture);
    public DateTime TotalTime { get; set; } = DateTime.Parse(rawRecord.TotalTime, CultureInfo.InvariantCulture);
    public string FunctionName { get; set; } = rawRecord.FunctionName;
    public string Result { get; set; } = rawRecord.Result;
}