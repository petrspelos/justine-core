using System.Collections.Generic;
using Discord;
using JustineCore.Discord;
using Xunit;

namespace JustineCore.Tests
{
    public class UtilitiesTests
    {
        [Theory]
        [InlineData("55", 55)]
        [InlineData("1969", 1969)]
        [InlineData("37", 37)]
        [InlineData("-99", -99)]
        [InlineData("Not parsable...", 0)]
        public void StringDictionaryToIntTest(string value, int expected)
        {
            const string propertyName = "MyTestProperty";

            var dictionary = new Dictionary<string, string>
            {
                { propertyName, value }
            };

            var actual = dictionary.TryParseIntByKey(propertyName);
            
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("True", true)]
        [InlineData("true", true)]
        [InlineData("False", false)]
        [InlineData("false", false)]
        [InlineData("Not a Bool", false)]
        public void StringDictionaryToBoolTest(string value, bool expected)
        {
            const string propertyName = "BoolTestProperty";

            var dictionary = new Dictionary<string, string>
            {
                { propertyName, value }
            };

            var actual = dictionary.TryParseBoolByKey(propertyName);
            
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("Verbose", LogSeverity.Verbose)]
        [InlineData("Debug", LogSeverity.Debug)]
        [InlineData("Error", LogSeverity.Error)]
        [InlineData("Info", LogSeverity.Info)]
        [InlineData("Warning", LogSeverity.Warning)]
        [InlineData("Critical", LogSeverity.Critical)]
        [InlineData("Not Correct enumeration", LogSeverity.Info)]
        [InlineData("9841", LogSeverity.Info)]
        public void StringDictionaryToLogSeverityTest(string value, LogSeverity expected)
        {
            const string propertyName = "LogLvlProperty";

            var dictionary = new Dictionary<string, string>
            {
                { propertyName, value }
            };

            var actual = dictionary.TryParseLogSeverityByKey(propertyName);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ArgumentsParser_Test()
        {
            const string expectedToken = "Some Token Here";
            var arguments = new [] {$"-t:{expectedToken}", "RandomArgument"};
            
            var result = Utilities.ParseArgumentArray(arguments);

            Assert.Equal(expectedToken, result.Token);

            Assert.False(result.HelpMode);
        }

        [Fact]
        public void ArgumentsParser_EmptyTokenTest()
        {
            const string expectedToken = "";
            var arguments = new [] {"-t:", "-help"};
            
            var result = Utilities.ParseArgumentArray(arguments);

            Assert.Equal(expectedToken, result.Token);
            Assert.True(result.HelpMode);
        }

        [Fact]
        public void ArgumentsParser_EmptyArgsTest()
        {
            const string expectedToken = null;
            var arguments = new string[] {};
            
            var result = Utilities.ParseArgumentArray(arguments);

            Assert.Equal(expectedToken, result.Token);
            Assert.False(result.HelpMode);
        }
    }
}