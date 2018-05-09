namespace JustineCore.Language
{
    public interface ILocalization
    {
        void LoadLanguages();

        string GetResource(string key, int languageId = 0);

        string FromTemplate(string template, int languageId = 0);
    }
}