namespace MultiDictionaryCore
{
    public class TermTranslation1
    {
        public EntityTranslation Translation { get; set; }
        public int LanguageId { get { return Translation.Language.Id; } }
    }
}