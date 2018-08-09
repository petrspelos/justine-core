using System;
using System.Reflection;
using System.Text;
using Discord;
using Discord.WebSocket;
using JustineCore.Discord;
using Xunit;

namespace JustineCore.Tests
{
    public class DiscordSocketConfigFactoryTests
    {
        [Fact]
        public void ConfigDefaultTest()
        {
            const int expectedMsgChacheSize = 0;
            const bool expectedAlwaysDownloadUsers = false;
            
            var actual = DiscordSocketConfigFactory.GetDefault();

            Assert.Equal(expectedMsgChacheSize, actual.MessageCacheSize);
            Assert.Equal(expectedAlwaysDownloadUsers, actual.AlwaysDownloadUsers);
        }

        [Fact]
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

            Assert.Equal(expectedMsgCacheSize, actual.MessageCacheSize);
            Assert.Equal(expectedAlwaysDownloadUsers, actual.AlwaysDownloadUsers);
            Assert.Equal(expectedLogSev, actual.LogLevel);
        }

        [Fact]
        public void ConfigFromJson_InvalidJsonTest()
        {
            const string json = "Not a proper json string.";

            Assert.Throws<ArgumentException>( () => DiscordSocketConfigFactory.FromJsonDictionary(json) );
        }

        [Fact]
        public void ConfigFromJson_InvalidJsonValuesTest()
        {
            const int expectedMsgCacheSize = 0;
            const bool expectedAlwaysDownloadUsers = false;

            var json = $@"{{ 
                ""MessageCacheSize"" : ""Hello, World!"",
                ""AlwaysDownloadUsers"" : ""Invalid Boolean""
            }}";

            var actual = DiscordSocketConfigFactory.FromJsonDictionary(json);

            Assert.Equal(expectedMsgCacheSize, actual.MessageCacheSize);
            Assert.Equal(expectedAlwaysDownloadUsers, actual.AlwaysDownloadUsers);
        }

        [Fact]
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

            Assert.Equal(expectedMsgCacheSize, actual.MessageCacheSize);
            Assert.Equal(expectedAlwaysDownloadUsers, actual.AlwaysDownloadUsers);
        }

        [Fact]
        public void ConfigFromJson_DuplicateKeyTest()
        {
            const int expected = 100;

            var json = $@"{{ 
                ""MessageCacheSize"" : ""75"",
                ""AlwaysDownloadUsers"" : ""true"",
                ""MessageCacheSize"" : ""{expected}"",
            }}";

            var actual = DiscordSocketConfigFactory.FromJsonDictionary(json);

            Assert.Equal(expected, actual.MessageCacheSize);
        }

        [Theory]
        [InlineData("Not a valid string.", LogSeverity.Info)]
        [InlineData("999", LogSeverity.Info)]
        [InlineData("4", LogSeverity.Verbose)]
        [InlineData("Verbose", LogSeverity.Verbose)]
        [InlineData("Debug", LogSeverity.Debug)]
        [InlineData("Critical", LogSeverity.Critical)]
        [InlineData("Error", LogSeverity.Error)]
        [InlineData("Warning", LogSeverity.Warning)]
        public void LogLevelParsingTest_InvalidValue(string input, LogSeverity expected)
        {
            var actual = DiscordSocketConfigFactory.StringToLogSeverity(input);
            Assert.Equal(expected, actual);
        }
    }
}