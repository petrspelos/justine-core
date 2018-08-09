using System;
using JustineCore.Discord.Providers.UserData;
using JustineCore.Entities;
using Xunit;

namespace JustineCore.Tests
{
    public class UserDataProviderTests
    {
        [Fact]
        public void GlobalUserDataProviderTest_DataDoesNotExist()
        {
            const ulong userId = 1;
            var udp = TestUnity.Resolve<GlobalUserDataProvider>();

            var actual = udp.GlobalDataExists(userId);

            Assert.False(actual);
        }

        [Fact]
        public void GlobalUserDataProviderTest_DataExists()
        {
            const ulong userId = 2;
            var consent = new DataConsent
            {
                Date = DateTime.UtcNow
            };

            var udp = TestUnity.Resolve<GlobalUserDataProvider>();

            Assert.False(udp.GlobalDataExists(userId));

            udp.AddNewGlobalData(userId, consent);

            Assert.True(udp.GlobalDataExists(userId));
        }

        [Fact]
        public void GlobalUserDataProviderTest_DataDeleting()
        {
            const ulong userId = 3;
            var consent = new DataConsent
            {
                Date = DateTime.UtcNow
            };

            var udp = TestUnity.Resolve<GlobalUserDataProvider>();

            Assert.False(udp.GlobalDataExists(userId));

            udp.AddNewGlobalData(userId, consent);

            Assert.True(udp.GlobalDataExists(userId));

            udp.DeleteUserGlobalData(userId);

            Assert.False(udp.GlobalDataExists(userId));
        }
    }
}