namespace Chirp.CLI;
public static class UserInterface
{
    public static void PrintCheep(string username, string message, long unixSeconds)
    {
        Console.WriteLine($"{username}: {Format(unixSeconds)} - {message}");
    }
    
    public static void PrintError(string message)
    {
        Console.WriteLine(message);
    }

    private static string Format(long seconds)
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(seconds);
        return dateTimeOffset.ToString("dddd, dd MMMM yyyy HH:mm:ss");
    }
}