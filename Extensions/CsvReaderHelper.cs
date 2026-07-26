using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using ReGen.Dtos;

namespace ReGen.Extensions;

public sealed record CsvReadResult(IReadOnlyList<CsvRecord> Records, int SkippedRows);

public static class CsvReaderHelper
{
    public static CsvReadResult ReadCsvFile(string csvFilePath)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = "\t", // tab-delimited
            IgnoreBlankLines = true,
            TrimOptions = TrimOptions.Trim
        };

        List<RawCsvRecord> records;
        using (var reader = new StreamReader(csvFilePath))
        using (var csv = new CsvReader(reader, config))
        {
            records = csv.GetRecords<RawCsvRecord>().ToList();
        }

        var parsedRecords = new List<CsvRecord>();
        var skippedRows = 0;

        foreach (var record in records)
            if (CsvRecord.TryCreate(record, out var csvRecord))
                parsedRecords.Add(csvRecord);
            else
                skippedRows++;

        return new CsvReadResult(parsedRecords, skippedRows);
    }
}