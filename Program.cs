using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
namespace Chirp.CLI.Client;

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
                    UserInterface.PrintCheep(username, message, long.Parse(timestamp));
                }
            }
        }
        catch (Exception e)
        {
            UserInterface.PrintError(e.Message);
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


    public static long GetTimestamp()
    {
        return DateTimeOffset.Now.ToUnixTimeSeconds();
    }

    public static void WriteCheep(string cheep)
    {
        long ts = GetTimestamp();
        string line = Environment.UserName + "," + "\"" + cheep + "\"" + "," + ts;
        using (StreamWriter writer = File.AppendText("chirp_cli_db.csv"))
        {
            writer.WriteLine(line);
        }
        UserInterface.PrintCheep(Environment.UserName, cheep, ts);
    }
}