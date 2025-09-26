using Microsoft.Data.Sqlite;

public class DBFacade
{
	    private static readonly string DbPath =
        Environment.GetEnvironmentVariable("CHIRPDBPATH")
        ?? Path.Combine("tmp", "chirp.db");

    	private static readonly string ConnectionString = $"Data Source={DbPath}";

    public static List<CheepViewModel> Cheeps(int limit, int offset)
    {
        var cheeps = new List<CheepViewModel>();

        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        // Join messages with authors
        string sql = @"
            SELECT m.message_id, m.text, m.pub_date, u.username
            FROM message m
            JOIN user u ON m.author_id = u.user_id
            ORDER BY m.pub_date DESC
            Limit @Limit OFFSET @Offset;
        ";

        using var command = new SqliteCommand(sql, connection);
		command.Parameters.AddWithValue("@Limit", limit);
        command.Parameters.AddWithValue("@Offset", offset);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            string author = reader.GetString(3);
            string message = reader.GetString(1);

            // pub_date is stored as Unix timestamp (int/long)
            long pubDateUnix = reader.GetInt64(2);
            string timestamp = UnixTimeStampToDateTimeString(pubDateUnix);

            cheeps.Add(new CheepViewModel(author, message, timestamp));
        }

        return cheeps;
    }

    public static List<CheepViewModel> CheepsByAuthor(string authorName, int limit, int offset)
    {
        var cheeps = new List<CheepViewModel>();

        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        string sql = @"
            SELECT m.message_id, m.text, m.pub_date, u.username
            FROM message m
            JOIN user u ON m.author_id = u.user_id
            WHERE u.username =  @Author
            ORDER BY m.pub_date DESC
            LIMIT @Limit OFFSET @Offset;
        ";

        using var command = new SqliteCommand(sql, connection);
        command.Parameters.AddWithValue("@Author", authorName);
		command.Parameters.AddWithValue("@Limit", limit);
        command.Parameters.AddWithValue("@Offset", offset);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            string author = reader.GetString(3);
            string message = reader.GetString(1);
            long pubDateUnix = reader.GetInt64(2);
            string timestamp = UnixTimeStampToDateTimeString(pubDateUnix);

            cheeps.Add(new CheepViewModel(author, message, timestamp));
        }

        return cheeps;
    }

    private static string UnixTimeStampToDateTimeString(long unixTimeStamp)
    {
        DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp).UtcDateTime;
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }
}
