using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using CsvHelper.Configuration;
using DocoptNet;

namespace Chirp.CLI.Client;

using CsvHelper;

class Program
{
    const string usage = @"Chirp CLI version.

    Usage:
      chirp read <limit>
      chirp cheep <message>
      chirp (-h | --help)
      chirp --version

    Options:
      -h --help     Show this screen.
      --version     Show version.
    ";
    static void Main(string[] args)
    {
        try
        {
            var arguments = new Docopt().Apply(usage, args, version: "1.0", exit: true);

            if (arguments["read"].IsTrue)
            {
                ReadCheeps();
            }
            else if (arguments["cheep"].IsTrue)
            {
                var message = arguments["message"].ToString();
                WriteCheep(message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message); //general error exception
            Environment.Exit(1);
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