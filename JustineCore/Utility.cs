using System;
using System.Collections.Generic;
using System.Net;
using Discord;
using FluentScheduler;
using Humanizer;
using ImageMagick;
using JustineCore.Discord.Features.RPG;

namespace JustineCore
{
    public static class Utility
    {
        public static Random Random = new Random(DateTime.Now.Millisecond);

        public static byte[] GetMissionFailureImage(IUser user, int gainedG, int lostHp, int currentG, int currentHp)
        {
            return GetMissionImage("img/failure_template.png", user, gainedG, lostHp, currentG, currentHp);
        }

        public static byte[] GetMissionSuccessImage(IUser user, int gainedG, int lostHp, int currentG, int currentHp)
        {
            return GetMissionImage("img/success_template.png", user, gainedG, lostHp, currentG, currentHp);
        }

        public static byte[] GetMissionImage(string templateImg, IUser user, int gainedG, int lostHp, int currentG, int currentHp)
        {
            var url = user.GetAvatarUrl(ImageFormat.Jpeg);
            if(url is null) url = Constants.DefaultAvatarUrl;

            var webClient = new WebClient();
            byte[] imageBytes = webClient.DownloadData(url);

            using (MagickImage image = new MagickImage(new MagickColor(Constants.DiscordBgHex), 200, 75))
            {
                using (MagickImage inner = new MagickImage(imageBytes))
                {
                    inner.Resize(40, 40);
                    image.Composite(inner, 10, 10, CompositeOperator.Over);
                }

                using (MagickImage template = new MagickImage(templateImg))
                {
                    image.Composite(template, 0, 0, CompositeOperator.Over);
                }

                new Drawables()
                    .FontPointSize(10)
                    .Font("Arial")
                    .FillColor(MagickColors.White)
                    .TextAlignment(TextAlignment.Left)
                    .Text(80, 45, $"+{gainedG} gold | -{lostHp} HP")
                    .FillColor(MagickColor.FromRgb(189,189,189))
                    .Text(20, 68, $"{currentHp}")
                    .Text(62, 68, $"{currentG}")
                    .Draw(image);

                image.Format = MagickFormat.Jpeg;
                return image.ToByteArray();
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