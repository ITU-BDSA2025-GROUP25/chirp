using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace SimpleDB;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    private readonly string _filePath;

    public CSVDatabase(string filePath)
    {
        _filePath = filePath;
    }

    public IEnumerable<T> Read(int? limit = null)
    {
        using var reader = new StreamReader(_filePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records = csv.GetRecords<T>();
        return limit.HasValue ? records.Take(limit.Value).ToList() : records.ToList();
    }

    public void Store(T record)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = !File.Exists(_filePath) || new FileInfo(_filePath).Length == 0
        };

        using var stream = File.Open(_filePath, FileMode.Append);
        using var writer = new StreamWriter(stream);
        using var csv = new CsvWriter(writer, config);
        csv.WriteRecord(record);
        csv.NextRecord();
    }
}