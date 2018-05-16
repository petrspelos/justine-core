using FluentScheduler;
using JustineCore.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;
using static JustineCore.Utility;

#pragma warning disable 4014

namespace JustineCore
{
    internal class Program
    {
        internal static Discord.Connection Connection;

        private static async Task Main(string[] args)
        {
            Console.WriteLine(ResolvePlaceholders("today is <proper-date>!"));
            
            JobManager.Initialize(new Registry());

            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            
            Discord.Logger.Log("=== Justine Core started. ===");
            Unity.RegisterTypes();

            var appConfig = Unity.Resolve<AppConfig>();
            appConfig.ApplyArguments(args);
            
            Connection = new Discord.Connection();

            await Connection.ConnectAsync(appConfig, CancellationToken.None);
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var exception = (Exception) unhandledExceptionEventArgs.ExceptionObject;
            Connection.NotifyOwner($"Whops, I crashed!\n{exception.Message}");
            Discord.Logger.Log($"[Unhandled Exception] {exception.Message}");
        }
    }
}
