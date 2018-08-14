using System;
using System.Reflection;
using System.Text;
using Discord;
using Discord.WebSocket;
using JustineCore.Discord;
using Newtonsoft.Json;
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

            var actual = DiscordSocketConfigFactory.FromJson(json);

            Assert.Equal(expectedMsgCacheSize, actual.MessageCacheSize);
            Assert.Equal(expectedAlwaysDownloadUsers, actual.AlwaysDownloadUsers);
            Assert.Equal(expectedLogSev, actual.LogLevel);
        }

        [Fact]
        public void ConfigFromJson_InvalidJsonDoesNotThrowTest()
        {
            const string json = "Not a proper json string.";

            DiscordSocketConfigFactory.FromJson(json);
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

            var actual = DiscordSocketConfigFactory.FromJson(json);

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

            var actual = DiscordSocketConfigFactory.FromJson(json);

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

            var actual = DiscordSocketConfigFactory.FromJson(json);

            Assert.Equal(expected, actual.MessageCacheSize);
        }

        [Theory]
        [InlineData("4", LogSeverity.Verbose)]
        [InlineData("Verbose", LogSeverity.Verbose)]
        [InlineData("Debug", LogSeverity.Debug)]
        [InlineData("Critical", LogSeverity.Critical)]
        [InlineData("Error", LogSeverity.Error)]
        [InlineData("Warning", LogSeverity.Warning)]
        public void DiscordSocketConfigFactory_LogSeverityParsingTest(string input, LogSeverity expected)
        {
            var json = $@"{{ ""LogLevel"" : ""{input}"" }}";
            var actual = DiscordSocketConfigFactory.FromJson(json).LogLevel;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void DeserializeJsonTest()
        {
            var expected = new DiscordSocketConfig(){GatewayHost = "ABC"};
            var json = @"{
                    ""GatewayHost"" : ""ABC""
                }";

            var actual = JsonConvert.DeserializeObject<DiscordSocketConfig>(json);

            Assert.Equal(expected.GatewayHost, actual.GatewayHost);
            Assert.Equal(expected.WebSocketProvider, actual.WebSocketProvider);
        }
    }
}
