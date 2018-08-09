using System;
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

        public bool UserHasProblemWithId(ulong userId, int problemId)
        {
            var user = GetAccount(userId);
            if(user is null) return false;
            if(!user.Problems.Any()) return false;
            
            return problemId >= 0 && problemId < user.Problems.Count;
        }

        public IEnumerable<UserProblemView> GetProblemsByPredicate(Func<UserProblem, bool> predicate)
        {
            var result = new List<UserProblemView>();
            var currentDate = DateTime.Now;

            foreach(var pAcc in _cAccounts)
            {
                foreach(var problem in pAcc.Value.Problems)
                {
                    if(predicate.Invoke(problem))
                    {
                        result.Add(new UserProblemView 
                        { 
                            CreatedAt = problem.CreatedAt,
                            MessageId = problem.MessageId,
                            UserId = pAcc.Key
                        });
                    }
                }
            }

            return result;
        }

        public UserProblemAccount GetUserAccountByPredicate(Func<UserProblemAccount, bool> predicate)
        {
            return _cAccounts.Values.Where(predicate).FirstOrDefault();
        }

        public IEnumerable<UserProblemView> GetExpiredProblems()
        {
            return GetProblemsByPredicate(p => GetDateTimeHoursDiff(p.CreatedAt) <= -24);
        }

        public IEnumerable<UserProblemView> GetSoonToBeExpiredProblems()
        {
            return GetProblemsByPredicate(p => GetDateTimeHoursDiff(p.CreatedAt) < -20 && GetDateTimeHoursDiff(p.CreatedAt) > -24);
        }

        private double GetDateTimeHoursDiff(DateTime date)
        {
            var diff = date - DateTime.Now;
            return diff.TotalHours;
        }

        private string GetKeyFor(UserProblemAccount account)
        {
            return string.Format(KeyFormat, account.Id);
        }

        private bool AccountExists(ulong userId)
        {
            return _cAccounts.Values.Any(a => a.Id == userId);
        }

    }
}