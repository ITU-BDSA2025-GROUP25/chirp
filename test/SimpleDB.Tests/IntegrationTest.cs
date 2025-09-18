using System.IO;
using SimpleDB;
using Xunit;

namespace SimpleDB.Tests
{
    public class CSVDatabaseTests
    {
        [Fact]
        public void StoreAndRead_ReturnsStoredRecord()
        {
            // Arrange: use a temporary file
            string tempFile = Path.GetTempFileName();
            var db = CSVDatabase<Cheeps>.Instance(tempFile);

            var cheep = new Cheeps("Hedin", "Test message", 1735689600);

            // Act: store and then read back
            db.Store(cheep);
            var results = db.Read();

            // Assert: verify the record exists
            Assert.Contains(results, c =>
                c.Author == "Hedin" &&
                c.Message == "Test message" &&
                c.Timestamp == 1735689600);

            // Cleanup
            File.Delete(tempFile);
        }
    }
}