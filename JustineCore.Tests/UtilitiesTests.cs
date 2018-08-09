using System.Collections.Generic;
using Discord;
using JustineCore.Discord;
using NUnit.Framework;

namespace JustineCore.Tests
{
    [TestFixture]
    public class UtilitiesTests
    {
        [Test]
        [TestCase("55", ExpectedResult = 55)]
        [TestCase("1969", ExpectedResult = 1969)]
        [TestCase("37", ExpectedResult = 37)]
        [TestCase("-99", ExpectedResult = -99)]
        [TestCase("Not parsable...", ExpectedResult = 0)]
        public int StringDictionaryToIntTest(string value)
        {
            const string propertyName = "MyTestProperty";

            var dictionary = new Dictionary<string, string>
            {
                { propertyName, value }
            };

            return dictionary.TryParseIntByKey(propertyName);
        }

        [Test]
        [TestCase("True", ExpectedResult = true)]
        [TestCase("true", ExpectedResult = true)]
        [TestCase("False", ExpectedResult = false)]
        [TestCase("false", ExpectedResult = false)]
        [TestCase("Not a Bool", ExpectedResult = false)]
        public bool StringDictionaryToBoolTest(string value)
        {
            const string propertyName = "BoolTestProperty";

            var dictionary = new Dictionary<string, string>
            {
                { propertyName, value }
            };

            return dictionary.TryParseBoolByKey(propertyName);
        }

        [Test]
        [TestCase("Verbose", ExpectedResult = LogSeverity.Verbose)]
        [TestCase("Debug", ExpectedResult = LogSeverity.Debug)]
        [TestCase("Error", ExpectedResult = LogSeverity.Error)]
        [TestCase("Info", ExpectedResult = LogSeverity.Info)]
        [TestCase("Warning", ExpectedResult = LogSeverity.Warning)]
        [TestCase("Critical", ExpectedResult = LogSeverity.Critical)]
        [TestCase("Not Correct enumeration", ExpectedResult = LogSeverity.Info)]
        [TestCase("9841", ExpectedResult = LogSeverity.Info)]
        public LogSeverity StringDictionaryToLogSeverityTest(string value)
        {
            const string propertyName = "LogLvlProperty";

            var dictionary = new Dictionary<string, string>
            {
                { propertyName, value }
            };

            return dictionary.TryParseLogSeverityByKey(propertyName);
        }
    }
}