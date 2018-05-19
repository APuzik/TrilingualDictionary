using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrilingualDictionaryCore
{
    public interface ITranslationFactory
    {
        ITranslatable CreateObject(string translation);
        void ChangeObject(ITranslatable obj, string translation);
    }

    public class TopicTranslationFactory : ITranslationFactory
    {
        public ITranslatable CreateObject(string translation)
        {
            return new TopicTranslation
            {
                Id = 0,
                LangForId = (LanguageId)(-1),
                TopicId = 0,
                Translation = translation
            };
        }

        public void ChangeObject(ITranslatable obj, string translation)
        {
            obj.Translation = translation;
        }
    }

    public class LangPartTranslationFactory : ITranslationFactory
    {
        public ITranslatable CreateObject(string translation)
        {
            return new PartOfSpeechTranslation
            {
                Id = 0,
                LangForId = (LanguageId)(-1),
                PartOfSpeechId = 0,
                Translation = translation
            };
        }

        public void ChangeObject(ITranslatable obj, string translation)
        {
            obj.Translation = translation;
        }
    }

    public class SemanticTranslationFactory : ITranslationFactory
    {
        public ITranslatable CreateObject(string translation)
        {
            return new SemanticTranslation
            {
                Id = 0,
                LangForId = (LanguageId)(-1),
                SemId = 0,
                Translation = translation
            };
        }

        public void ChangeObject(ITranslatable obj, string translation)
        {
            obj.Translation = translation;
        }
    }

    public class ChangeableTranslationFactory : ITranslationFactory
    {
        public ITranslatable CreateObject(string translation)
        {
            return new ChangableTranslation
            {
                Id = 0,
                LangForId = (LanguageId)(-1),
                ChangeableTypeId = 0,
                Translation = translation
            };
        }

        public void ChangeObject(ITranslatable obj, string translation)
        {
            obj.Translation = translation;
        }
    }

    public class LanguageTranslationFactory : ITranslationFactory
    {
        public ITranslatable CreateObject(string translation)
        {
            return new LanguageTranslation
            {
                Id = 0,
                LangForId = (LanguageId)(-1),
                LangIdNeeded = 0,
                Translation = translation
            };
        }

        public void ChangeObject(ITranslatable obj, string translation)
        {
            obj.Translation = translation;
        }
    }
}
