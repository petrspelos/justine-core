using System;
using JustineCore.Discord.Features;
using JustineCore.Entities;
using NUnit.Framework;

namespace JustineCore.Tests
{
    public class UserConsentTests
    {
        [Test]
        public void UserNotConsentedTest()
        {
            var userData = new GlobalUserData { DiscordId = 1 };
            const bool expected = false;

            var dcc = new DataCollectionConsent();
            var actual = dcc.GetUserConsented(userData);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void UserConsentedTest()
        {
            var userData = GetConsentedUser();
            const bool expected = true;

            var dcc = new DataCollectionConsent();
            var actual = dcc.GetUserConsented(userData);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void UserGiveConsentTest()
        {
            var userData = new GlobalUserData { DiscordId = 1 };
            var dcc = new DataCollectionConsent();

            Assert.IsFalse(dcc.GetUserConsented(userData));

            dcc.GiveConsent(userData);

            Assert.IsNotNull(userData.CollectionConsent);
            Assert.IsTrue(dcc.GetUserConsented(userData));
        }

        [Test]
        public void UserRemoveConsentTest()
        {
            var userData = GetConsentedUser();
            var dcc = new DataCollectionConsent();

            Assert.IsTrue(dcc.GetUserConsented(userData));

            dcc.RemoveConsent(userData);

            Assert.IsFalse(dcc.GetUserConsented(userData));
            Assert.IsNull(userData.CollectionConsent);
        }

        private static GlobalUserData GetConsentedUser()
        {
            return new GlobalUserData
            {
                DiscordId = 1,
                CollectionConsent = new DataConsent
                {
                    Date = DateTime.UtcNow,
                    MessageId = 2
                }
            };
        }
    }
}
