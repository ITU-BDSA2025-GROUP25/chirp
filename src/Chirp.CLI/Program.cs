using DocoptNet;
using SimpleDB;

namespace Chirp.CLI.Client;

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
                int limit = int.Parse(arguments["<limit>"].ToString());
                ReadCheeps(limit);
            }
            else if (arguments["cheep"].IsTrue)
            {
                var message = arguments["<message>"].ToString();
                WriteCheep(message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Environment.Exit(1);
        }
    }

    public static void ReadCheeps(int limit)
    {
        IDatabaseRepository<Cheeps> db = CSVDatabase<Cheeps>.Instance("chirp_cli_db.csv");
        var records = db.Read(limit);

        foreach (var record in records)
            Console.WriteLine($"{record.Author} @ {DateFormatting(record.Timestamp)}: {record.Message}");
    }

    public static string DateFormatting(long timestamp)
    {
        var dto = DateTimeOffset.FromUnixTimeSeconds(timestamp);
        return dto.LocalDateTime.ToString("dd/MM/yy HH:mm:ss");
    }

    public static long GetTimestamp()
    {
        return DateTimeOffset.Now.ToUnixTimeSeconds();
    }

    public static void WriteCheep(string cheep)
    {
        var record = new Cheeps(Environment.UserName, cheep, GetTimestamp());

        IDatabaseRepository<Cheeps> db = CSVDatabase<Cheeps>.Instance("chirp_cli_db.csv");
        db.Store(record);

        UserInterface.PrintCheep(Environment.UserName, cheep, record.Timestamp);
    }
}