using System.Runtime.CompilerServices;
using System.Text;


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
        string cheep;
        try
        {
            using StreamReader reader = new("chirp_cli_db.csv");
            while ((cheep = reader.ReadLine()) != null)
            {
                Console.WriteLine(cheep);
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public static string DateFormatting(long seconds)
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(seconds);
        return dateTimeOffset.ToString("dddd, dd MMMM yyyy HH:mm:ss");
    }

    public static long GetTimestamp()
    {
        return DateTimeOffset.Now.ToUnixTimeSeconds();;
        
    }

    public static void WriteCheep(string cheep)
    {
        
        using(StreamWriter writer = File.AppendText("chirp_cli_db.csv"))
            writer.WriteLine(Environment.UserName + "," + cheep + "," +  GetTimestamp());
        
        Console.WriteLine(Environment.UserName + "," + cheep + "," +  GetTimestamp());
    }
}