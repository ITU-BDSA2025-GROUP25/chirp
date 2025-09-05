using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using CsvHelper.Configuration;

namespace Chirp.CLI.Client;

using CsvHelper;

class Program
{
    static void Main(string[] args)
    {
        StringBuilder key = new();
        StringBuilder userText = new();

        string space = "";
        
        if (args.Length > 0)
        {
            key.Append(args[0]);
        }
        

        for (int i = 1; i < args.Length; i++)
        {
            if (i == args.Length - 1)
            {
                space = "";
            }
            userText.Append(args[i] + space);
            space = " ";
        }
        
        
        if (key.ToString().Equals("read"))
        {
           ReadCheeps();
        } else if (key.ToString().Equals("cheep"))
        {
           WriteCheep(userText.ToString());
        }
    }

    public static void ReadCheeps()
    {
        using (var reader = new StreamReader("chirp_cli_db.csv"))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = new List<Cheeps>();
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                var record = new Cheeps
                {
                    Author = csv.GetField<string>("Author"),
                    Message = csv.GetField<string>("Message"),
                    Timestamp = csv.GetField<long>("Timestamp"),
                };
                records.Add(record);
            }
            foreach (var record in records)
                Console.WriteLine($"{record.Author} @ {DateFormatting(record.Timestamp)}: {record.Message}");
        }
    }

    private static string[] ParseCsvLineWithRegex(string line)
    {
        // Regex pattern: split on commas that are NOT inside quotes
        string pattern = @",(?=(?:[^""]*""[^""]*"")*[^""]*$)";
        
        return Regex.Split(line, pattern)
            .Select(field => field.Trim('"')) // Remove surrounding quotes
            .ToArray();
    }

    public static string DateFormatting(long timestamp)
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timestamp);
        return dateTimeOffset.LocalDateTime.ToString("dd/MM/yy HH:mm:ss");
    }

    public static long GetTimestamp()
    {
        return DateTimeOffset.Now.ToUnixTimeSeconds();
    }

    public static void WriteCheep(string cheep)
    {
        var records = new List<Cheeps>
        {
            new Cheeps {Author = Environment.UserName, Message = cheep, Timestamp = GetTimestamp()}
        };

        
        // Append to the file.
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            // Don't write the header again.
            HasHeaderRecord = false,
        };
        using (var stream = File.Open("chirp_cli_db.csv", FileMode.Append))
        using (var writer = new StreamWriter(stream))
        using (var csv = new CsvWriter(writer, config))
        {
            csv.WriteRecords(records);
        }

        UserInterface.PrintCheep(Environment.UserName, cheep, GetTimestamp());
    }
}

class Cheeps
{
    public required string Author { get; set; }
    public required String Message { get; set; }
    public long Timestamp { get; set; }
    
    public record Cheep(string Author, string Message, long Timestamp)
    {
        
        
    }
}