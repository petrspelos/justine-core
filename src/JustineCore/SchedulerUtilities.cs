using System;
using FluentScheduler;

namespace JustineCore
{
    public static class SchedulerUtilities
    {
        public static void ExecuteAt(Action action, int hours, int minutes)
        {
            JobManager.AddJob(action, s => s.ToRunOnceAt(hours, minutes));
        }

        public static void ExecuteAt(Action action, DateTime time)
        {
            JobManager.AddJob(action, s => s.ToRunOnceAt(time));
        }

        public static void ExecuteAfter(Action action, int seconds)
        {
            JobManager.AddJob(action, s => s.ToRunOnceIn(seconds).Seconds());
        }

        public static void ExecuteEveryDayAt(Action action, int hours, int minutes)
        {
            JobManager.AddJob(action, s => s.ToRunEvery(1).Days().At(hours, minutes));
        }

        public static void ExecuteEverHours(Action action, int hours)
        {
            JobManager.AddJob(action, s => s.ToRunEvery(1).Hours());
        }
    }
}