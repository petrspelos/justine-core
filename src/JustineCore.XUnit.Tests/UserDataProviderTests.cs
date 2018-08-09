using System;
using System.Linq;
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
        public void GlobalUserDataProviderTest_DataAlreadyExists()
        {
            const ulong userId = 4;
            var consent = new DataConsent
            {
                Date = DateTime.UtcNow
            };

            var udp = TestUnity.Resolve<GlobalUserDataProvider>();

            Assert.False(udp.GlobalDataExists(userId));

            udp.AddNewGlobalData(userId, consent);

            Assert.True(udp.GlobalDataExists(userId));

            // should simply return...
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

            // double delete should do nothing.
            udp.DeleteUserGlobalData(userId);
        }

        [Fact]
        public void GlobalUserDataProviderTest_DeletingFakeId()
        {
            const ulong fakeId = 123456789;

            var udp = TestUnity.Resolve<GlobalUserDataProvider>();
            udp.DeleteUserGlobalData(fakeId);
        }

        [Fact]
        public void GlobalUserDataProviderTest_PredicateSearch()
        {
            var udp = TestUnity.Resolve<GlobalUserDataProvider>();

            const int expectedCount = 4;
            var expectedDate = DateTime.Now;

            udp.AddNewGlobalData(1, new DataConsent { Date = expectedDate });
            udp.AddNewGlobalData(2, new DataConsent { Date = expectedDate });
            udp.AddNewGlobalData(3, new DataConsent { Date = expectedDate });
            udp.AddNewGlobalData(4, new DataConsent { Date = DateTime.UtcNow.AddDays(5) });
            udp.AddNewGlobalData(5, new DataConsent { Date = expectedDate });

            var actualCount = udp.SearchByPredicate(u => u.CollectionConsent.Date == expectedDate).Count();

            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public void GlobalUserDataProviderTest_Saving()
        {
            var udp = TestUnity.Resolve<GlobalUserDataProvider>();

            var baseDate = DateTime.Now;
            var expectedDate = baseDate.AddDays(20);

            udp.AddNewGlobalData(1, new DataConsent { Date = expectedDate });
            
            var data = udp.GetGlobalUserData(1);

            data.CollectionConsent.Date = expectedDate;

            udp.SaveGlobalUserData(data);

            var actualData = udp.GetGlobalUserData(1);

            Assert.Equal(expectedDate, actualData.CollectionConsent.Date);
        }
    }
}