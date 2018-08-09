using JustineCore.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;
//using static JustineCore.Utilities;

#pragma warning disable 4014

namespace JustineCore
{
    internal class Program
    {
        internal static Discord.Connection Connection;

        private static async Task Main(string[] args)
        {
            SchedulerUtilities.Initialize();
            Unity.RegisterTypes();

            // TODO: Integrate into the system
            //var appArgs = ParseArgumentArray(args);

            // TODO: Refactor
            //AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;            
            //Discord.Logger.Log("=== Justine Core started. ===");

            // TODO: Remove duplication...
            var appConfig = Unity.Resolve<AppConfig>();
            appConfig.ApplyArguments(args);
            
            Connection = Unity.Resolve<Discord.Connection>();

            await Connection.ConnectAsync(CancellationToken.None);
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var exception = (Exception) unhandledExceptionEventArgs.ExceptionObject;
            Discord.Logger.Log($"[Unhandled Exception] {exception.Message}");
            Discord.Logger.Log($"[Stringified Exception] {exception}");
            Discord.Logger.Log($"[Stack Trace] {exception.StackTrace}");
            Connection.NotifyOwner($"Whops, I crashed!\n{exception.Message}");
        }
    }
}
