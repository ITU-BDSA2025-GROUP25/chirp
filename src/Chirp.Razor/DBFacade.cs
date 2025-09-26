using System.Data.SQLite;

public class DBFacade {

    public static List<CheepViewModel> GetDbCheeps()
    {
        using var connection = new SQLiteConnection("tmp/chirp.db");
        connection.Open();

        var cheeps = new List<CheepViewModel>();
        using (var command = new SQLiteCommand("SELECT author_id, text, pub_date FROM Message", connection))
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                var cheep = new CheepViewModel(
                    reader.GetString(0),
                    reader.GetString(1),
                    reader.GetString(2)
                );
                cheeps.Add(cheep);
            }
        }
        
        foreach (var cheep in cheeps)
        {
            Console.WriteLine($"{cheep.Author}: {cheep.Message} ({cheep.Timestamp})");
        }
        return cheeps;

    }
}