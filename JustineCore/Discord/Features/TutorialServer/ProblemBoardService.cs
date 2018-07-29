using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using JustineCore.Discord.Providers.TutorialBots;
using JustineCore.Entities;

namespace JustineCore.Discord.Features.TutorialServer
{
    public class ProblemBoardService
    {
        private readonly DiscordSocketClient _client;
        private readonly ProblemProvider _problemProvider;

        private SocketGuild _tutorialServer;
        private SocketTextChannel _problemBoardChannel;
        private SocketTextChannel _generalChannel;
        private bool performingCleanup = false;

        public ProblemBoardService(DiscordSocketClient client, ProblemProvider problemProvider)
        {
            _client = client;
            _problemProvider = problemProvider;
        }

        internal async Task MessageReceived(SocketMessage socketMessage)
        {
            if (!(socketMessage is SocketUserMessage msg)) return;
            if (msg.Channel is SocketDMChannel) return;
            var context = new SocketCommandContext(_client, msg);
            if(context.Guild.Id != Constants.TutorialServerId) return;

            if(context.Channel.Id != Constants.TutorialGeneralId) return;
            if(context.User.IsBot) return;

            if(context.Message.Content.Trim().StartsWith(Constants.TutorialProblemMarker))
            {
                await CreateProblemForUser(context.Message.Content, context.User.Id);
            }
            else if(context.Message.Content.Trim().StartsWith(Constants.TutorialSolvedMarker))
            {
                var problemIdString = context.Message.Content.Replace(Constants.TutorialSolvedMarker, "");
                var success = int.TryParse(problemIdString.Trim(), out var problemId);
                if(!success)
                {
                    if(_problemProvider.UserHasProblemWithId(context.User.Id, 0))
                    {
                        await SolveProblemForUser(0, context.User.Id);
                    }
                    else
                    {
                        await _generalChannel.SendMessageAsync($"{context.User.Mention}, it looks like you're trying to mark a problem as solved.\n\nHowever, I can't seem to parse the id '{problemIdString.Trim()}'. Make sure you send just the emoji with a number.\n\nThe ID of your problem should be defined in Problem Board.");
                    }
                    return;
                }
                try
                {
                    await SolveProblemForUser(problemId, context.User.Id);
                }
                catch(Exception e)
                {
                    Logger.Log($"[ProblemBoardService][Exception]{e.Message}");
                }
            }
            else if(context.Message.Content.Trim().StartsWith(Constants.TutorialHelpMarker))
            {
                await _generalChannel.SendMessageAsync($@"**HOW TO POST A PROBLEM**
_Step 1) Post the description of your problem in a single message beginning with the {Constants.TutorialProblemMarker} emoji._

Example:
{Constants.TutorialProblemMarker} I cannot seem to access the SendMessageAsync method of a channel:
```cs
var channel = await _client.GetChannelAsync(someChannelId);
// channel.SendMessageAsync doesn't exist
```

_Step 2) Wait for someone to respond. :blush: Take break, watch some YouTube videos._

_Step 3) After your problem is solved, mark it as such with the {Constants.TutorialSolvedMarker} emoji._

Example:
{Constants.TutorialSolvedMarker}

If you have more than one problem, add the problem's ID after the marker:

Example:
{Constants.TutorialSolvedMarker} 1

_You can find the ID of a problem in problem-board._");
            }
        }

        public async Task CreateProblemForUser(string message, ulong userId)
        {
            if(message.Length >= 1800) return;

            if(_problemBoardChannel is null) InitializeResources();

            var account = _problemProvider.GetAccount(userId);

            if(performingCleanup)
            {
                await _generalChannel.SendMessageAsync("I am currently performing a cleanup. Please try submitting your problem again in about five minutes.");
                return;
            }

            var author = _tutorialServer.GetUser(userId);
            if(author is null)
            {
                Logger.Log($"Could not create a Problem in ProblemBoard for user with id {userId}. User not found.");
                return;
            }

            var msg = await _problemBoardChannel.SendMessageAsync("_A problem new is being created..._");

            var problemObj = new UserProblem
            {
                CreatedAt = DateTime.Now,
                MessageId = msg.Id
            };

            account.Problems.Add(problemObj);
            _problemProvider.SaveAccount(account);

            var problemId = account.Problems.Count - 1;

            var problemMessage = $"[Problem ID: {problemId}] [By: {author.Mention}]\n{message}";

            await msg.ModifyAsync(m => m.Content = problemMessage);

            await _generalChannel.SendMessageAsync($":shield:  {author.Mention}, your problem with ID ({problemId}) has been created.");
        }

        public async Task SolveProblemForUser(int problemId, ulong userId, bool silent = false)
        {
            if(_problemBoardChannel is null) InitializeResources();

            var account = _problemProvider.GetAccount(userId);

            if(performingCleanup && !silent)
            {
                await _generalChannel.SendMessageAsync("I am currently performing a cleanup. Please try submitting your problem again in about five minutes.");
                return;
            }

            var author = _tutorialServer.GetUser(userId);
            if(author is null)
            {
                Logger.Log($"Could not solve a Problem in ProblemBoard for user with id {userId}. User not found.");
                return;
            }

            if(problemId >= account.Problems.Count || problemId < 0)
            {
                Logger.Log($"[ProblemBoardService] Invalid Problem ID - user {author.Username} problem ID {problemId}. Has {account.Problems.Count} problems.");
                throw new ArgumentException("invalid Problem ID");
            }

            var problem = account.Problems[problemId];
            var msg = await _problemBoardChannel.GetMessageAsync(problem.MessageId);
            await msg.DeleteAsync();

            Logger.Log($"Count before deleting: {account.Problems.Count}");
            account.Problems.RemoveAt(problemId);
            Logger.Log($"Count after deleting: {account.Problems.Count}");
            _problemProvider.SaveAccount(account);
            Logger.Log($"Count after saving: {account.Problems.Count}");

            Logger.Log($"Just deleted {problemId}.");
            await UpdateAllProblems(account.Problems);

            if(silent) return;

            await _generalChannel.SendMessageAsync($":shield: {author.Mention}, your problem with ID ({problemId}) has been solved.");
        }

        public async Task SolveProblemForUser(ulong problemMessageId, ulong userId, bool silent = false)
        {
            var acc = _problemProvider.GetUserAccountByPredicate(p => p.Problems.Any(pp => pp.MessageId == problemMessageId));
            var index = acc.Problems.IndexOf(acc.Problems.FirstOrDefault(p => p.MessageId == problemMessageId));
            await SolveProblemForUser(index, userId, silent);
        }

        public async Task RoutineProblemCleanup()
        {
            Logger.Log("[RoutineProblemCleanup] Performing a routine problem cleanup");
            performingCleanup = true;

            if(_problemBoardChannel is null) InitializeResources();

            var usersToMention = new List<ulong>();
            var toDelete = _problemProvider.GetExpiredProblems();
            //var toWarn = _problemProvider.GetSoonToBeExpiredProblems();

            var hadProblems = toDelete.Any();

            foreach(var problem in toDelete)
            {
                try
                {
                    await SolveProblemForUser(problem.MessageId, problem.UserId, true);
                    if(usersToMention.Contains(problem.UserId)) continue;
                    usersToMention.Add(problem.UserId);
                }
                catch(Exception e)
                {
                    Logger.Log($"[RoutineProblemCleanup] Failed to delete a problem. Ignoring...\nException: {e}");
                }
            }

            var mentions = new StringBuilder();
            foreach(var userId in usersToMention)
            {
                var user = _tutorialServer.GetUser(userId);
                if(user is null) continue;
                mentions.Append($"{user.Mention} ");
            }

            performingCleanup = false;
            if(!hadProblems)
            {
                Logger.Log("[RoutineProblemCleanup] Cleanup completed... nothing to delete.");
                return;
            }

            await _generalChannel.SendMessageAsync($":warning: Some problems by these users were removed: {mentions.ToString()}\n\n_Problems are removed 24 hours after their creation._");
        }

        private async Task UpdateAllProblems(List<UserProblem> problems)
        {
            Logger.Log($"Count in UpdateAllProblems: {problems.Count}");
            for(int i = 0; i < problems.Count; i++)
            {
                var imsg = await _problemBoardChannel.GetMessageAsync(problems[i].MessageId);
                Logger.Log($"[UPDATING] {imsg.Content}");
                if(imsg is null) continue;
                var msg = imsg as RestUserMessage;
                if(msg.Content.Contains($"[Problem ID: {i}]")) continue;
                var rgx = new Regex(@"\[Problem ID: \d+\]");
                await msg.ModifyAsync(m => m.Content = $"[Problem ID: {i}]{rgx.Replace(msg.Content, string.Empty)}");
            }
        }

        private void InitializeResources()
        {
            _tutorialServer = _client.GetGuild(Constants.TutorialServerId);
            _problemBoardChannel = _tutorialServer.GetTextChannel(Constants.TutoriaProblemBoardId);
            _generalChannel = _tutorialServer.GetTextChannel(Constants.TutorialGeneralId);
        }
    }
}