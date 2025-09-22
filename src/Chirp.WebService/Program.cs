using Microsoft.AspNetCore.Mvc;
using SimpleDB;

var builder = WebApplication.CreateBuilder(args);

// Path to CSV file
const string CsvPath = "../Chirp.CLI/chirp_cli_db.csv";

// Register the database for our endpoints
var cheepsDb = CSVDatabase<Cheeps>.Instance(CsvPath);

builder.Services.AddSingleton<IDatabaseRepository<Cheeps>>(cheepsDb);

var app = builder.Build();

// Root route
app.MapGet("/", () => "Chirp Web Service is running. Use /cheeps (GET) and /cheep (POST) endpoints.");

// Get all cheeps
app.MapGet("/cheeps", GetCheeps);

// Post a new cheep
app.MapPost("/cheep", PostCheep);

//Run
app.Run();


static IResult GetCheeps([FromServices] IDatabaseRepository<Cheeps> db,
    [FromQuery] int? limit)
{
    var records = db.Read(limit).ToList();
    return Results.Ok(records);
}

static IResult PostCheep([FromBody] Cheeps cheep,
    [FromServices] IDatabaseRepository<Cheeps> db)
{
    // very basic validation
    if (string.IsNullOrWhiteSpace(cheep.Author) ||
        string.IsNullOrWhiteSpace(cheep.Message) ||
        cheep.Timestamp <= 0)
    {
        return Results.BadRequest("Author, Message and Timestamp are required");
    }

    db.Store(cheep);
    return Results.Created("/cheeps", cheep);
}
