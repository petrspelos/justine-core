using System;
using System.Reflection;
using System.Text;
using Discord;
using Discord.WebSocket;
using JustineCore.Discord;
using NUnit.Framework;

namespace JustineCore.Tests
{
    [TestFixture]
    public class DiscordSocketConfigFactoryTests
    {
        [Test]
        public void ConfigDefaultTest()
        {
            const int expectedMsgChacheSize = 0;
            const bool expectedAlwaysDownloadUsers = false;
            
            var actual = DiscordSocketConfigFactory.GetDefault();

            Assert.AreEqual(expectedMsgChacheSize, actual.MessageCacheSize);
            Assert.AreEqual(expectedAlwaysDownloadUsers, actual.AlwaysDownloadUsers);
        }

        [Test]
        public void ConfigFromJsonTest()
        {
            const int expectedMsgCacheSize = 10;
            const bool expectedAlwaysDownloadUsers = true;
            const LogSeverity expectedLogSev = LogSeverity.Debug;

            var json = $@"{{ 
                ""MessageCacheSize"" : ""{expectedMsgCacheSize}"",
                ""AlwaysDownloadUsers"" : ""{expectedAlwaysDownloadUsers}"",
                ""LogLevel"" : ""Debug""
            }}";

            var actual = DiscordSocketConfigFactory.FromJsonDictionary(json);

            Assert.AreEqual(expectedMsgCacheSize, actual.MessageCacheSize);
            Assert.AreEqual(expectedAlwaysDownloadUsers, actual.AlwaysDownloadUsers);
            Assert.AreEqual(expectedLogSev, actual.LogLevel);
        }

        [Test]
        public void ConfigFromJson_InvalidJsonTest()
        {
            const string json = "Not a proper json string.";

            Assert.Throws<ArgumentException>( () => DiscordSocketConfigFactory.FromJsonDictionary(json) );
        }

        [Test]
        public void ConfigFromJson_InvalidJsonValuesTest()
        {
            const int expectedMsgCacheSize = 0;
            const bool expectedAlwaysDownloadUsers = false;

            var json = $@"{{ 
                ""MessageCacheSize"" : ""Hello, World!"",
                ""AlwaysDownloadUsers"" : ""Invalid Boolean""
            }}";

            var actual = DiscordSocketConfigFactory.FromJsonDictionary(json);

            Assert.AreEqual(expectedMsgCacheSize, actual.MessageCacheSize);
            Assert.AreEqual(expectedAlwaysDownloadUsers, actual.AlwaysDownloadUsers);
        }

        [Test]
        public void ConfigFromJson_IgnoreExtraValuesTest()
        {
            const int expectedMsgCacheSize = 10;
            const bool expectedAlwaysDownloadUsers = true;

            var json = $@"{{ 
                ""ExtraProperty1"" : ""Value 1"",
                ""MessageCacheSize"" : ""{expectedMsgCacheSize}"",
                ""AlwaysDownloadUsers"" : ""{expectedAlwaysDownloadUsers}"",
                ""ExtraProperty2"" : ""Value 2""
            }}";

            var actual = DiscordSocketConfigFactory.FromJsonDictionary(json);

            Assert.AreEqual(expectedMsgCacheSize, actual.MessageCacheSize);
            Assert.AreEqual(expectedAlwaysDownloadUsers, actual.AlwaysDownloadUsers);
        }

        [Test]
        public void ConfigFromJson_DuplicateKeyTest()
        {
            const int expected = 100;

            var json = $@"{{ 
                ""MessageCacheSize"" : ""75"",
                ""AlwaysDownloadUsers"" : ""true"",
                ""MessageCacheSize"" : ""{expected}"",
            }}";

            var actual = DiscordSocketConfigFactory.FromJsonDictionary(json);

            Assert.AreEqual(expected, actual.MessageCacheSize);
        }

        [Test]
        [TestCase("Not a valid string.", ExpectedResult=LogSeverity.Info)]
        [TestCase("999", ExpectedResult=LogSeverity.Info)]
        [TestCase("4", ExpectedResult=LogSeverity.Verbose)]
        [TestCase("Verbose", ExpectedResult=LogSeverity.Verbose)]
        [TestCase("Debug", ExpectedResult=LogSeverity.Debug)]
        [TestCase("Critical", ExpectedResult=LogSeverity.Critical)]
        [TestCase("Error", ExpectedResult=LogSeverity.Error)]
        [TestCase("Warning", ExpectedResult=LogSeverity.Warning)]
        public LogSeverity LogLevelParsingTest_InvalidValue(string input)
        {
            return DiscordSocketConfigFactory.StringToLogSeverity(input);
        }
    }
}