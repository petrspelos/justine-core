using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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
using JustineCore.Entities;
using System.Linq;

namespace JustineCore
{
    public static class Utilities
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

        public static T GetRandomElement<T>(List<T> list)
        {
            return list[Random.Next(0, list.Count)];
        }

        public static int TryParseIntByKey(this Dictionary<string, string> d, string key)
        {
            if(!d.ContainsKey(key)) return 0;
            int.TryParse(d[key], out var result);
            return result;
        }

        public static bool TryParseBoolByKey(this Dictionary<string, string> d, string key)
        {
            if(!d.ContainsKey(key)) return false;
            bool.TryParse(d[key], out var result);
            return result;
        }

        public static AppArguments ParseArgumentArray(IEnumerable<string> args)
        {
            return new AppArguments 
            {
                Token = args.FirstOrDefault(s => s.StartsWith("-t:"))?.Substring(3),
                HelpMode = args.Any(s => s.ToLower() == "-help")
            };
        }
    }
}