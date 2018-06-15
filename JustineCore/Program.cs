using FluentScheduler;
using JustineCore.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;
using Humanizer;

#pragma warning disable 4014

namespace JustineCore
{
    internal class Program
    {
        internal static Discord.Connection Connection;

        private static async Task Main(string[] args)
        {


#if DEBUG
            Discord.Logger.Log("[APPLICATION TYPE] Debug Mode");
#else
            Discord.Logger.Log("[APPLICATION TYPE] Release Mode");
#endif  
            JobManager.Initialize(new Registry());

            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            
            Discord.Logger.Log("=== Justine Core started. ===");
            Unity.RegisterTypes();

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
