using System;
using System.Collections.Generic;
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

        public ProblemBoardService(DiscordSocketClient client, ProblemProvider problemProvider)
        {
            _client = client;
            _problemProvider = problemProvider;
        }

        internal async Task MessageReceived(SocketMessage socketMessage)
        {
            //if(_tutorialServer is null) InitializeResources();

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
                    await _generalChannel.SendMessageAsync($"{context.User.Mention}, it looks like you're trying to mark a problem as solved.\n\nHowever, I can't seem to parse the id '{problemIdString.Trim()}'. Make sure you send just the emoji with a number.\n\nThe ID of your problem should be defined in Problem Board.");
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
        }

        public async Task CreateProblemForUser(string message, ulong userId)
        {
            if(message.Length >= 1800) return;

            if(_problemBoardChannel is null) InitializeResources();

            var account = _problemProvider.GetAccount(userId);

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

        public async Task SolveProblemForUser(int problemId, ulong userId)
        {
            if(_problemBoardChannel is null) InitializeResources();

            var account = _problemProvider.GetAccount(userId);

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

            await _generalChannel.SendMessageAsync($":shield: {author.Mention}, your problem with ID ({problemId}) has been solved.");
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