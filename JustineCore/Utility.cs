using System;
using FluentScheduler;
using Humanizer;

namespace JustineCore
{
    public static class Utility
    {
        public static string ResolvePlaceholders(string template)
        {
            return template
                .Replace("<proper-date>", $"{DateTime.Now.Day.Ordinalize()} of {DateTime.Now:MMMM}, {DateTime.Now:yyyy}");
        }

        public static void ExecuteAt(Action action, int hours, int minutes)
        {
            JobManager.AddJob(action, s => s.ToRunOnceAt(hours, minutes));
        }

        public static void ExecuteAt(Action action, DateTime time)
        {
            JobManager.AddJob(action, s => s.ToRunOnceAt(time));
        }

        public static void ExecuteAfter(Action action, int interval)
        {
            JobManager.AddJob(action, s => s.ToRunOnceIn(interval));
        }

        public static void ExecuteEveryDayAt(Action action, int hours, int minutes)
        {
            JobManager.AddJob(action, s => s.ToRunEvery(1).Days().At(hours, minutes));
        }
    }
}