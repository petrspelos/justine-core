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
            JobManager.Initialize(new Registry());
            JobManager.JobException += OnJobFailed;

            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            
            Discord.Logger.Log("=== Justine Core started. ===");
            Unity.RegisterTypes();

            var appConfig = Unity.Resolve<AppConfig>();
            appConfig.ApplyArguments(args);
            
            Connection = Unity.Resolve<Discord.Connection>();

            await Connection.ConnectAsync(CancellationToken.None);
        }

        private static void OnJobFailed(JobExceptionInfo exInfo)
        {
            Discord.Logger.Log($@"=== A job threw an exception ===
Job Name: {exInfo.Name};
Exception: {exInfo.Exception.Message};
Stack trace: {exInfo.Exception.StackTrace};");
            Connection.NotifyOwner("Yo, Peter. Turns out a job threw an exception. <:down:409830013387538446>");
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
