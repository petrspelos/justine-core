using System;
using System.Collections.Generic;
using System.Linq;
using JustineCore.Entities;
using JustineCore.Storage;
using JustineCore.Discord.Features.RPG.Actions;
using System.Collections.Concurrent;
using System.Text;

namespace JustineCore.Discord.Providers.TutorialBots
{
    public class VerificationProvider
    {
        private const string VerificationListKey = "BotVerificationList";

        private readonly IDataStorage _dataStorage;
        private List<BotVerification> _verificationList;

        public VerificationProvider(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
            LoadVerificationList();
        }

        public bool CheckVerification(ulong userId, ulong botId, string message)
        {
            if(IsVerified(botId)) return false;

            if(!BotVerificationExists(botId, userId)) return false;

            var verification = _verificationList.FirstOrDefault(d => d.BotId == botId && d.OwnerId == userId && d.Verified == false);

            if(verification == null) return false;

            if(!message.Contains(verification.VerificationString)) return false;

            ValidateVerification(botId, userId);

            return true;
        }

        private string GenerateVerificationString()
        {
            var pool = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var result = new StringBuilder();

            int seed;
            for(int i = 0; i < 10; i++)
            {
                seed = JustineCore.Utilities.Random.Next(0, pool.Length);
                result.Append(pool[seed]);
            }

            return result.ToString();
        }

        //<summary>Creates a new verification and returns the verification string. Returns null if already verified.</summary>
        public string CreateNewVerification(ulong botId, ulong userId)
        {
            if(IsVerified(botId)) return null;

            if(BotVerificationExists(botId, userId)) return null;

            var verificationCode = GenerateVerificationString();

            _verificationList.Add(new BotVerification
            {
                OwnerId = userId,
                BotId = botId,
                Verified = false,
                VerificationString = verificationCode
            });

            Save();

            return verificationCode;
        }

        public bool IsVerified(ulong botId)
        {
            return _verificationList.Any(b => b.BotId == botId && b.Verified);
        }

        public void ValidateVerification(ulong botId, ulong userId)
        {
            var correct = _verificationList.FirstOrDefault(b => b.BotId == botId && b.OwnerId == userId);

            if(correct == null) return;

            correct.Verified = true;

            _verificationList = _verificationList.Where(b => b.BotId != botId || b.OwnerId == userId).ToList();

            Save();
        }

        public void ClearValidation(ulong botId)
        {
            _verificationList = _verificationList.Where(d => d.BotId != botId).ToList();

            Save();
        }

        public bool BotVerificationExists(ulong botId, ulong userId)
        {
            return _verificationList.Any(d => d.BotId == botId && d.OwnerId == userId);
        }

        public IEnumerable<BotVerification> SearchByPredicate(Func<BotVerification, bool> predicate)
        {
            return _verificationList.Where(predicate);
        }

        public void Save()
        {
            _dataStorage.Store(_verificationList, VerificationListKey);
        }

        private void LoadVerificationList()
        {
            try
            {
                _verificationList = _dataStorage.Get<List<BotVerification>>(VerificationListKey);
            }
            catch(DataStorageKeyDoesNotExistException)
            {
                _verificationList = new List<BotVerification>();
            }
        }
    }
}