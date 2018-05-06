using System;
using JustineCore.Discord.Providers.UserData;
using JustineCore.Entities;
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

        [Test]
        public void GlobalUserDataProviderTest_DataExists()
        {
            const ulong userId = 2;
            var consent = new DataConsent
            {
                Date = DateTime.UtcNow
            };

            var udp = TestUnity.Resolve<GlobalUserDataProvider>();

            Assert.IsFalse(udp.GlobalDataExists(userId));

            udp.AddNewGlobalData(userId, consent);

            Assert.IsTrue(udp.GlobalDataExists(userId));
        }

        [Test]
        public void GlobalUserDataProviderTest_DataDeleting()
        {
            const ulong userId = 3;
            var consent = new DataConsent
            {
                Date = DateTime.UtcNow
            };

            var udp = TestUnity.Resolve<GlobalUserDataProvider>();

            Assert.IsFalse(udp.GlobalDataExists(userId));

            udp.AddNewGlobalData(userId, consent);

            Assert.IsTrue(udp.GlobalDataExists(userId));

            udp.DeleteUserGlobalData(userId);

            Assert.IsFalse(udp.GlobalDataExists(userId));
        }
    }
}