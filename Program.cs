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
            Console.WriteLine(builder.ToString());
        }
        
    }
}