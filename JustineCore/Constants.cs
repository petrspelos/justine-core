namespace JustineCore
{
    public static class Constants
    {
        public static readonly string[] ValidUpgradeLabels = new[] {"str", "spd", "lck", "int", "end"}; // hp temorarily removed

        public static readonly string[] MissionFailCauses = new[]
        {
            "A group of goblins stole your phone. And you know... It was a pretty expensive phone. :iphone:",
            "You got attacked by a scary skeleton that just wouldn't listen. :skull:"
        };

        public static readonly string[] MissionSuccessCauses = new[]
        {
            "On your mission, you managed to rob a store. Nice.",
            "You saved a local city from destruction. What a hero."
        };
    }
}