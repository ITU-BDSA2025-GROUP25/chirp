using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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
        string? cheep = null;
        try
        {
            using StreamReader reader = new("chirp_cli_db.csv");
            // Read and skip the header line
            reader.ReadLine();
            
            while ((cheep = reader.ReadLine()) != null)
            {
                // Use regex to parse CSV line properly (handles commas in quotes)
                string[] parts = ParseCsvLineWithRegex(cheep);
                
                if (parts.Length >= 3)
                {
                    string username = parts[0];
                    string message = parts[1];
                    string timestamp = parts[2];
                    Console.WriteLine($"{username}: {DateFormatting(timestamp)} - {message}");
                    // Remove the duplicate Console.WriteLine(cheep) if you don't need both outputs
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
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

    public static string DateFormatting(string seconds)
    {
        // Convert the string to long before passing to FromUnixTimeSeconds
        long timestamp = long.Parse(seconds);
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timestamp);
        return dateTimeOffset.ToString("dddd, dd MMMM yyyy HH:mm:ss");
    }

    public static long GetTimestamp()
    {
        return DateTimeOffset.Now.ToUnixTimeSeconds();
    }

    public static void WriteCheep(string cheep)
    {
        
        using(StreamWriter writer = File.AppendText("chirp_cli_db.csv"))
            writer.WriteLine(Environment.UserName + "," + "\"" + cheep + "\"" + "," +  GetTimestamp());
        
        Console.WriteLine(Environment.UserName + "," + "\"" + cheep + "\"" + "," +  GetTimestamp());
    }
}