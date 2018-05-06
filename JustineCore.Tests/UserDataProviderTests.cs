using JustineCore.Discord.Providers.UserData;
using NUnit.Framework;

namespace JustineCore.Tests
{
    public class UserDataProviderTests
    {
        [Test]
        public void GlobalUserDataProviderTest_DataDoesNotExist()
        {
            const ulong userId = 1;
            var udp = TestUnity.Resolve<GlobalUserDataProvider>();

            var actual = udp.GlobalDataExists(userId);

            Assert.IsFalse(actual);
        }
    }
}