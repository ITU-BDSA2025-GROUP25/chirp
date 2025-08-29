using System.Runtime.CompilerServices;
using System.Text;


class Program
{
    static void Main(string[] args)
    {
        StringBuilder builder = new();
        string space = "";

        foreach (var arg in args)
        {
            builder.Append(space + arg);
            space = " ";
        }
        
        if (builder.ToString().Equals("read"))
        {
            ReadCheeps();
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

    public static string dateFormatting(long seconds)
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(seconds);
        return dateTimeOffset.ToString("dddd, dd MMMM yyyy HH:mm:ss");
    }
}