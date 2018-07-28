using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Discord;
using FluentScheduler;
using Humanizer;
using JustineCore.Discord;
using JustineCore.Discord.Features.RPG;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Text;
using SixLabors.ImageSharp.Processing.Transforms;
using SixLabors.ImageSharp.Processing.Processors;
using SixLabors.ImageSharp.Processing.Drawing;
using SixLabors.Fonts;
using SixLabors.Primitives;

namespace JustineCore
{
    public static class Utility
    {
        public static Random Random = new Random(DateTime.Now.Millisecond);

        public static string GetTestImage(string avatarUrl)
        {
            var webClient = new WebClient();
            byte[] imageBytes = webClient.DownloadData(avatarUrl);

            using(var background = SixLabors.ImageSharp.Image.Load("img/success_template.png"))
            using(var template = SixLabors.ImageSharp.Image.Load("img/success_template.png"))
            using(var avatar = SixLabors.ImageSharp.Image.Load(imageBytes))
            {
                //Font font = SystemFonts.CreateFont("Arial", 15);

                avatar.Mutate(x => x
                    .Resize(new Size(40,40))
                    //.DrawText("sexy feet", font, Rgba32.Pink, new PointF(0, 0))
                );

                background.Mutate(x => x
                    .DrawImage(avatar, 1.0f, new Point(10,10))
                    .DrawImage(template, 1.0f, new Point(0,0))
                );

                background.Save("img/t.jpg");
                return "img/t.jpg";
            }
        }

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

        public static uint GetGeneralCurveCost(uint lvl)
        {
            return (uint)(Math.Pow(lvl, 2.0) * 15.0);
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

        public static void ExecuteEverHours(Action action, int hours)
        {
            JobManager.AddJob(action, s => s.ToRunEvery(1).Hours());
        }

        public static T GetRandomElement<T>(List<T> list)
        {
            return list[Random.Next(0, list.Count)];
        }
    }
}