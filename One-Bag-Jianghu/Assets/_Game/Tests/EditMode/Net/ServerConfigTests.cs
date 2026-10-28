using System.IO;
using NUnit.Framework;
using OneBagJianghu.Net;

namespace OneBagJianghu.Tests.Net
{
    public sealed class ServerConfigTests
    {
        private string serverFile;

        [SetUp]
        public void SetUp()
        {
            serverFile = Path.GetTempFileName();
            File.WriteAllText(serverFile, "wss://file.example/ws\n");
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(serverFile);
        }

        [Test]
        public void CommandLineOverridesAllOtherSources()
        {
            // Arrange
            string[] arguments = { "game", "-server", "wss://argument.example/ws" };

            // Act
            string result = ServerConfig.ResolveUrl(arguments, "wss://prefs.example/ws", serverFile);

            // Assert
            Assert.That(result, Is.EqualTo("wss://argument.example/ws"));
        }

        [Test]
        public void PlayerPrefsOverridesStreamingAssets()
        {
            // Arrange
            string[] arguments = { "game" };

            // Act
            string result = ServerConfig.ResolveUrl(arguments, " wss://prefs.example/ws ", serverFile);

            // Assert
            Assert.That(result, Is.EqualTo("wss://prefs.example/ws"));
        }

        [Test]
        public void StreamingAssetsOverridesDefault()
        {
            // Arrange
            string[] arguments = { "game" };

            // Act
            string result = ServerConfig.ResolveUrl(arguments, null, serverFile);

            // Assert
            Assert.That(result, Is.EqualTo("wss://file.example/ws"));
        }

        [Test]
        public void DefaultIsUsedWhenOtherSourcesAreEmpty()
        {
            // Arrange
            string missingFile = serverFile + ".missing";

            // Act
            string result = ServerConfig.ResolveUrl(new[] { "game" }, " ", missingFile);

            // Assert
            Assert.That(result, Is.EqualTo(ServerConfig.DefaultUrl));
        }
    }
}
