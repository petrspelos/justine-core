using System;
using JustineCore.Entities;

namespace JustineCore.Discord.Features
{
    public class DataCollectionConsent
    {
        public bool GetUserConsented(GlobalUserData userData)
        {
            return userData.CollectionConsent != null;
        }

        public void GiveConsent(GlobalUserData userData)
        {
            userData.CollectionConsent = new DataConsent
            {
                Date = DateTime.UtcNow
            };
        }
    }
}