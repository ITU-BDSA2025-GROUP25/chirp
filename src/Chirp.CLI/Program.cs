using DocoptNet;

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

    private static readonly HttpClient client = new HttpClient
    {
        BaseAddress = new Uri("http://localhost:5143")
    };

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
        try
        {
            var response = client.GetAsync($"cheeps?limit={limit}")
                                 .GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            var cheeps = response.Content.ReadFromJsonAsync<List<CheepDTO>>()
                                         .GetAwaiter().GetResult();

            foreach (var record in cheeps)
                Console.WriteLine($"{record.Author} @ {DateFormatting(record.Timestamp)}: {record.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching cheeps: {ex.Message}");
        }
    }

    public static void WriteCheep(string cheep)
    {
        var record = new CheepDTO
        {
            Author = Environment.UserName,
            Message = cheep,
            Timestamp = GetTimestamp()
        };

        try
        {
            var response = client.PostAsJsonAsync("cheep", record)
                                 .GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            Console.WriteLine($"{record.Author} @ {DateFormatting(record.Timestamp)}: {record.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error posting cheep: {ex.Message}");
        }
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

    // DTO for HTTP communication
    public class CheepDTO
    {
        public string Author { get; set; }
        public string Message { get; set; }
        public long Timestamp { get; set; }
    }
}
