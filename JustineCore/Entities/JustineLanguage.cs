using System.Collections.Generic;

namespace JustineCore.Entities
{
    public class JustineLanguage
    {
        public uint LanguageId { get; set; }
        public string LanguageName { get; set; }
        public Dictionary<string, string> Resources { get; set; }
    }
}
