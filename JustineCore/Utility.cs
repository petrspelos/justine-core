using System;
using System.Collections.Generic;
using FluentScheduler;
using Humanizer;

namespace JustineCore
{
    public static class Utility
    {
        public static Random Random = new Random(DateTime.Now.Millisecond);

        public static string ResolvePlaceholders(string template)
        {
            return template
                .Replace("<proper-date>", $"{DateTime.Now.Day.Ordinalize()} of {DateTime.Now:MMMM}, {DateTime.Now:yyyy}");
        }

        //y = log(x) * mult
        public static int GetLogValNoNegative(int value, int mult = 10)
        {
            return (int)(Math.Log(value + 1) * (double)mult);
        }

        public static int GetGeneralCurveLevel(int cost)
        {
            return (int)Math.Sqrt(((double)cost / 15.0));
        }

        public static int GetGeneralCurveCost(int lvl)
        {
            return (int)(Math.Pow(lvl, 2.0) * 15.0);
        }

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

        public static T GetRandomElement<T>(List<T> list)
        {
            return list[Random.Next(0, list.Count)];
        }
    }
}