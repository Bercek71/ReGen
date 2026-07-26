using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace ReGen.Dtos;

public class CsvRecord
{
    public TimeSpan StepTime { get; private init; }
    public double V { get; private init; }
    public double I { get; private init; }
    public double W { get; private init; }
    public double Ah { get; private init; }
    public double Wh { get; private init; }
    public DateTime TotalTime { get; private init; }
    public string FunctionName { get; private init; } = string.Empty;
    public string Result { get; private init; } = string.Empty;

    public static bool TryCreate(RawCsvRecord rawRecord, [NotNullWhen(true)] out CsvRecord? record)
    {
        record = null;

        if (!TimeSpan.TryParseExact(rawRecord.StepTime, @"hh\:mm\:ss", CultureInfo.InvariantCulture, out var stepTime))
            return false;

        if (!TryParseDouble(rawRecord.V, out var v) ||
            !TryParseDouble(rawRecord.I, out var i) ||
            !TryParseDouble(rawRecord.W, out var w) ||
            !TryParseDouble(rawRecord.Ah, out var ah) ||
            !TryParseDouble(rawRecord.Wh, out var wh))
            return false;

        if (!DateTime.TryParse(rawRecord.TotalTime, CultureInfo.InvariantCulture, DateTimeStyles.None,
                out var totalTime))
            return false;

        record = new CsvRecord
        {
            StepTime = stepTime,
            V = v,
            I = i,
            W = w,
            Ah = ah,
            Wh = wh,
            TotalTime = totalTime,
            FunctionName = rawRecord.FunctionName ?? string.Empty,
            Result = rawRecord.Result ?? string.Empty
        };

        return true;
    }

    private static bool TryParseDouble(string? value, out double result)
    {
        result = 0;
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return double.TryParse(value.Replace("+", ""), CultureInfo.InvariantCulture, out result);
    }
}