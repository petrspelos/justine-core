using JustineCore.Discord.Providers.UserData;
using JustineCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using JustineCore.Discord.Features.RPG.Actions;
using System.Threading.Tasks;

namespace JustineCore.Discord.Features.RPG.GoldDigging
{
    public class DiggingJobProvider
    {
        private const string DataStorageKey = "DiggingJobs";

        private List<DiggingJob> _activeJobs;
        private IDataStorage _ds;
        private GlobalUserDataProvider _gudp;
        private Discord.Connection _dcon;

        public DiggingJobProvider(IDataStorage dataStorage, GlobalUserDataProvider userProvider, Discord.Connection connection)
        {
            _ds = dataStorage;
            _gudp = userProvider;
            _dcon = connection;

            try
            {
                _activeJobs = dataStorage.Get<List<DiggingJob>>(DataStorageKey);
            }
            catch (DataStorageKeyDoesNotExistException)
            {
                _activeJobs = new List<DiggingJob>();
                dataStorage.Store(_activeJobs, DataStorageKey);
            }
        }

        public IEnumerable<DiggingJob> Get(Func<DiggingJob, bool> predicate)
        {
            return _activeJobs.Where(predicate);
        }

        public bool IsDigging(ulong userId)
        {
            return _activeJobs.Any(j => j.UserId == userId);
        }

        public async void AddJob(DiggingJob job)
        {
            if(_activeJobs.Any(j => j.UserId == job.UserId)) return;

            _activeJobs.Add(job);
            SaveJobs();
            
            await RegisterJob(job);
        }

        public void RemoveByUserId(ulong userId)
        {
            var target = _activeJobs.FirstOrDefault(j => j.UserId == userId);
            if(target is null) return;
            _activeJobs.Remove(target);
            SaveJobs();
        }

        public void SaveJobs()
        {
            _ds.Store(_activeJobs, DataStorageKey);
        }

        public async void RegisterAllJobs()
        {
            for(int i = _activeJobs.Count - 1; i >= 0; i--)
            {
                var job = _activeJobs[i];
                await RegisterJob(job);
            }
        }

        public async Task RegisterJob(DiggingJob job)
        {
            // Do not continue if consent was lost.
            if(!_gudp.GlobalDataExists(job.UserId)) return;

#if DEBUG
            // In DEBUG, we need to wait for SECONDS instead of HOURS
            // to be able to test the feature without actually waiting.
            var finishDateTime = job.StartDateTime.AddSeconds(job.DiggingLengthInHours);
            //var finishDateTime = job.StartDateTime.AddSeconds(20);
#else
            var finishDateTime = job.StartDateTime.AddHours(job.DiggingLengthInHours);
#endif

            // Finish if past due-date.
            if(finishDateTime < DateTime.Now)
            {
                await FinishDigging(job);
                return;
            }

            var user = _gudp.GetGlobalUserData(job.UserId);

            Utility.ExecuteAt(async () => {
                await FinishDigging(job);
            }, finishDateTime);

            Logger.Log($"[RegisterAllDiggingJobs] Scheduled a stored job.");
        }

        public async Task FinishDigging(DiggingJob job)
        {
            if(job is null) return;
            if(!_gudp.GlobalDataExists(job.UserId)) return;

            var reward = (uint)(Constants.DiggingGoldPerHour * job.DiggingLengthInHours);

            var user = _gudp.GetGlobalUserData(job.UserId);

            user.RpgAccount.AddGold(reward);

            try
            {
                var discordUser = _dcon.client.GetUser(job.UserId);

                var g = _dcon.client.GetGuild(job.GuildId);
                var ch = g.GetTextChannel(job.TextChannelId);

                // DIGGING UP A DEAD GUY CHANCE
                var deadPlayers = _gudp.SearchByPredicate(u => u.RpgAccount.Health <= 0);
                if(deadPlayers.Count() > 0 && Utility.Random.Next(0, 101) > 10)
                {
                    var luckyGuy = Utility.GetRandomElement(deadPlayers.ToList());
                    var luckyUser = g.GetUser(luckyGuy.DiscordId);
                    var luckyMention = (luckyUser == null) ? "someone from a different server" : luckyUser.Mention;

                    await ch.SendMessageAsync($":coffin: Oh damn! {discordUser.Mention} you dug up {luckyMention}.\n\nThanks to you, they get +1 health, which makes them alive again. :thumbsup:");

                    luckyGuy.RpgAccount.Health = 1;
                    _gudp.SaveGlobalUserData(luckyGuy);
                }
                // ----------------------------

                await ch.SendMessageAsync($"{discordUser.Mention}, you finished your digging and got {reward} gold!");
            }
            catch
            {
                Logger.Log("[DiggingJobProvider] Couldn't send a completion message.", ConsoleColor.Red);
            }

            _gudp.SaveGlobalUserData(user);

            RemoveByUserId(job.UserId);
        }
    }
}
