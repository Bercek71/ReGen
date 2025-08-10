using System.Globalization;
using System.IO;
using CsvHelper;
using ReGen.Model;

namespace ReGen.Extensions;

public static class CsvReaderHelper
{
    public static IEnumerable<CsvRecord> ReadCsvFile(string csvFilePath)
    {
        var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = "\t",  // tab-delimited
            IgnoreBlankLines = true,
            TrimOptions = CsvHelper.Configuration.TrimOptions.Trim,
        };
        // Read CSV
        List<RawCsvRecord> records;
        using (var reader = new StreamReader(csvFilePath))
        using (var csv = new CsvReader(reader, config))
        {
            records = csv.GetRecords<RawCsvRecord>().ToList();
        }

        return records.Select(record => new CsvRecord(record));
    }
    
}