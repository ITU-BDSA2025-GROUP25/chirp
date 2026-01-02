How to make Chirp! work locally

Prerequisites:
•.NET SDK 9.x (the web project targets net9.0)
•Only needed for troubleshooting migrations) EF Core CLI:
If dotnet ef is not recognized, install it: dotnet tool install --global dotnet-ef

Clone the repository
git clone https://github.com/ITU-BDSA2025-GROUP25/chirp
cd chirp

Run the web app
Navigate into the web project and start it:
cd src
cd Chirp.Web
dotnet run
click on localhost: link in terminal

Troubleshooting: migration / database issues
If you get database/migration errors (e.g., the DB schema is out of sync), reset the local SQLite database files and re-apply migrations.
Delete Cheep.db in (Chirp.Web) (possible in Chirp.Infrastructure if typed wrong commands)
Recreate database from migrations (from Chirp.Infrastructure)
cd src
cd Chirp.Infrastructure
dotnet ef database update --startup-project "../Chirp.Web/Chirp.Web.csproj"
then run dotnet in Chirp.Web again after
