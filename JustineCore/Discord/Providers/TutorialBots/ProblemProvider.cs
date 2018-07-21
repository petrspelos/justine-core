using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JustineCore.Entities;
using JustineCore.Storage;

namespace JustineCore.Discord.Providers.TutorialBots
{
    public class ProblemProvider
    {
        private const string GroupKey = "ProblemAccounts";
        private const string KeyFormat = "pa{0}";

        private readonly IDataStorage _storage;
        
        private List<UserProblemAccount> _accounts;
        private ConcurrentDictionary<ulong, UserProblemAccount> _cAccounts;

        public ProblemProvider(IDataStorage storage)
        {
            _storage = storage;

            _cAccounts = new ConcurrentDictionary<ulong, UserProblemAccount>();

            LoadAccounts();
        }

        private void LoadAccounts()
        {
            var accounts = _storage.GetGroup<UserProblemAccount>(GroupKey).ToList();
            foreach(var account in accounts)
            {
                _cAccounts.TryAdd(account.Id, account);
            }
        }

        public UserProblemAccount GetAccount(ulong userId)
        {
            return _cAccounts.GetOrAdd(userId, (key) => {
                var newAccount = new UserProblemAccount{Id = userId};
                _storage.Store(newAccount, GroupKey, GetKeyFor(newAccount));
                return newAccount;
            });
        }

        public void SaveAccount(UserProblemAccount account)
        {
            _storage.Store(account, GroupKey, GetKeyFor(account));
        }

        private string GetKeyFor(UserProblemAccount account)
        {
            return string.Format(KeyFormat, account.Id);
        }

        private bool AccountExists(ulong userId)
        {
            return _accounts.Any(a => a.Id == userId);
        }
    }
}