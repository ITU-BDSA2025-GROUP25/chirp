using System;
using System.IO;
using Chirp.CLI.Client;
using Xunit;

namespace Chirp.CLI.Tests
{
    public class UserInterfaceTests
    {
        [Fact]
        public void PrintCheep_ShouldFormatTimestampCorrectly()
        {
            // Arrange
            var user = "Hedin";
            var message = "Hello Chirp!";
            long unixTime = 1735689600; // 2025-01-01 00:00:00 UTC

            // Generate expected string using the same format as production code
            var expectedDate = DateTimeOffset
                .FromUnixTimeSeconds(unixTime)
                .ToString("dddd, dd MMMM yyyy HH:mm:ss");

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            UserInterface.PrintCheep(user, message, unixTime);

            // Assert
            string output = sw.ToString().Trim();
            Assert.Contains(user, output);
            Assert.Contains(message, output);
            Assert.Contains(expectedDate, output);
        }
    }
}