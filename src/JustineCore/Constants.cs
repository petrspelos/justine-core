namespace JustineCore
{
    public static class Constants
    {
        public const ulong ServiceServerId = 464334692502470666;
        public const ulong ServiceChannelId = 470187312827990016;
        public const ulong ServiceGeneralChannelId = 464334693089935361;
        public const ulong ServiceRole1 = 470188590538948608;
        public const ulong TutorialBotShowcaseChannelId = 381399595600838658;

#if DEBUG
        public const ulong TutorialServerId = 464334692502470666;
        public const ulong WaitingRoomChannelId = 470187312827990016;
        public const ulong TutorialMemberRoleId = 470188590538948608;
        public const ulong TutorialBotRoleId = 470188590538948608;
        public const ulong TutorialGeneralId = 470187312827990016;
        public const ulong TutorialBotDevRoleId = 470188590538948608;
        public const ulong TutoriaProblemBoardId = 464334693089935361;
# else
        public const ulong TutorialServerId = 377879473158356992;
        public const ulong WaitingRoomChannelId = 411864218548043785;
        public const ulong TutorialMemberRoleId = 411865173318696961;
        public const ulong TutorialBotRoleId = 381399631369863171;
        public const ulong TutorialGeneralId = 377879473644765185;
        public const ulong TutorialBotDevRoleId = 381409798903824394;
        public const ulong TutoriaProblemBoardId = 470217412147675137;
# endif
        public const int ProblemsDeleteAfterHours = 2;

#if DEBUG
        public const string TutorialProblemMarker = "[p]";
        public const string TutorialSolvedMarker = "[s]";
        public const string TutorialHelpMarker = "[h]";
#else
        public const string TutorialProblemMarker = "<:problem:470220494835286016>";
        public const string TutorialSolvedMarker = "<:solved:470220493635715072>";
        public const string TutorialHelpMarker = "<:help:473177492396703744>";
#endif

        public const ulong MiunieId = 411505318124847114;
        public const ulong JustineId = 370963330782986250;

        public const string DefaultAvatarUrl = "https://i.imgur.com/vM8PWVB.png";
        public const string DiscordBgHex = "#36393e";
        public const int DiggingGoldPerHour = 5;
        public const int StreakBonusPerHour = 1;
        public static readonly string[] ValidUpgradeLabels = new[] {"str", "spd", "lck", "int", "end"}; // hp temorarily removed
        public static readonly string[] MissionPitches = new[] 
        {
            ":crystal_ball: Solving a very spooky mystery with the gang.",
            "studying the ancient art of making sure the dough doesn't burn when baking a bread. Difficult stuff. :bread: :ok_hand:",
            "deciding whether you're so thirsty as to justify a trip to the mall, or if you're going to just deal with drinking water. :thinking:",
            "thinking about stuff you might do, while not doing it so that you can at least fool yourself and feel accomplished. ¯\\_(ツ)_/¯",
            "sitting on your ass and not saying anything. :no_mouth:",
            "playing a game while your fake adventure waits around to tell you that you failed. :game_die: :video_game:",
            "going to literal hell to defeat the Devil himself. And if you go by McDonald's, could you buy me a McFlurry? :icecream: :slight_smile:",
            "admiring the amazing Justine BOT, the superior state of being and her amazingness. Now that's an important mission. :pray: :prayer_beads:",
            "checking out Miunie's source code to see if you can contribute. :keyboard: And I've heard that if you contribute, you auto-win this mission. :thinking:",
            "visiting the grand programming wizard Snoops to steal some embeds knowledge or whatever. :sparkles:",
            "traveling to Sweden to talk to LeMorrow. No stealing, no objective. Just a chill talk. :flag_se:",
            "I don't really have a mission, but did you know that Diet Coke was only invented in 1982?",
            "going into the old forest of Grindel-roof to get the ancient sword Bligar-groof in order to visit the depths of Lili-Kii the Turr-Turr land and defeat the Qee'Karr the Tsuur chief. -- Good luck I guess. :dagger:"
        };
        public static readonly string[] MissionFailCauses = new[]
        {
            "You had this amazing idea for a joke, but it fell so flat, you might as well die.",
            "You did not hear what your friend said for the third time so you decided to laugh and hope it wasn't a question. **It was a question.**",
            "The pizza guy said \"enjoy your pizza\" and you responded with \"you too\".",
            "You saw someone wave at you, so you waved back, but they were actually waving at their friend behind you.",
            "You were forced to dance in public. But you know how it is with your dancing. Nobody was angry, just disappointed.",
            "You realized you put your shoes on before your socks. How did that happen?",
            "You patted your pants and realized you've lost your phone.",
            "You decided to pay with a credit card and it got rejected. How embarrassing.",
            "While buying clothes, you accidentally tried on a coat of another customer.",
            "You opened your laptop in public and that weird video started playing at full volume."
        };
        public static readonly string[] MissionSuccessCauses = new[]
        {
            "You actually failed the objective but managed to get a 50% off your sandwich.",
            "You got into an awkward conversation in the mall. You panicked and decided to rob the place.",
            "You've met a guy coming home from a whole day of digging gold. :smirk:",
            "What a success! Everyone is proud. Except for uncle Jon.",
            "A stunning success. Everyone is dead. You're not sure if you'll ever sleep again.",
            "You succeeded, but seriously... It probably wasn't worth it.",
            "Yes! Another fake mission that does nothing and gives you slightly more gold than just safely digging.",
            "A group of children mistook you for a popular YouTuber. You used them to complete your objective.",
            "You were about to go on your adventure, but then you noticed the Steam sale. You regret nothing.",
            "Heh, and you thought you'd never get a taste of human flesh. :thinking:"
        };
    }
}