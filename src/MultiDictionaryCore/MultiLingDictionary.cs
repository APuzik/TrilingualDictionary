using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiDictionaryCore
{
    class MultiLingDictionary
    {
        Dictionary<int, Term> terms = new Dictionary<int, Term>();        
        Dictionary<int, EntityTranslation> termTranslations = new Dictionary<int, EntityTranslation>();

        Dictionary<int, Language> languages = new Dictionary<int, Language>();        
        Dictionary<int, EntityTranslation> langTranslations = new Dictionary<int, EntityTranslation>();

        Dictionary<int, Semantic> semantics = new Dictionary<int, Semantic>();        
        Dictionary<int, EntityTranslation> semanticTranslations = new Dictionary<int, EntityTranslation>();

        Dictionary<int, Topic> topics = new Dictionary<int, Topic>(); 
        Dictionary<int, EntityTranslation> topicTranslations = new Dictionary<int, EntityTranslation>();

        Dictionary<int, PartOfSpeech> partsOfSpeech = new Dictionary<int, PartOfSpeech>(); 
        Dictionary<int, EntityTranslation> partOfSpeechTranslations = new Dictionary<int, EntityTranslation>();

        Dictionary<int, ChangPartType> changTypes = new Dictionary<int, ChangPartType>(); 
        Dictionary<int, EntityTranslation> changTypeTranslations = new Dictionary<int, EntityTranslation>();

        Dictionary<int, ChangPart> changeables = new Dictionary<int, ChangPart>(); 
        Dictionary<int, EntityTranslation> changeableTranslations = new Dictionary<int, EntityTranslation>();

        bool AddTerm(Semantic sem, Topic topic, Term parent)
        {
            try
            {
                Term newTerm = new Term
                {
                    Topic = topic,
                    Semantic = sem,
                    Parent = parent
                };
                int id = SaveToDB(newTerm);
                newTerm.Id = id;

                terms.Add(id, newTerm);
                return true;
            }
            catch(Exception ex)
            {

            }
            return false;
        }

        bool AddTermTranslation(Language lang, Term term, string translation)
        {
            try
            {
                EntityTranslation newTermTranslation = new EntityTranslation
                {
                    Entity = term,
                    Translation= translation
                };
                int id = SaveTermTranslationToDB(lang, newTermTranslation);
                newTermTranslation.Id = id;

                termTranslations.Add(id, newTermTranslation);
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        bool AddLanguage()
        {
            try
            {
                Language newLang = new Language();
                int id = SaveLanguageToDB(newLang);
                newLang.Id = id;

                languages.Add(id, newLang);
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        bool AddSemantic()
        {
            try
            {
                Semantic newSemantic = new Semantic();
                int id = SaveSemanticToDB(newSemantic);
                newSemantic.Id = id;

                semantics.Add(id, newSemantic);
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        bool AddLanguage()
        {
            try
            {
                Language newLang = new Language();
                int id = SaveLanguageToDB(newLang);
                newLang.Id = id;

                languages.Add(id, newLang);
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        bool AddLanguage()
        {
            try
            {
                Language newLang = new Language();
                int id = SaveLanguageToDB(newLang);
                newLang.Id = id;

                languages.Add(id, newLang);
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        private int SaveLanguageToDB(Language newLang)
        {
            throw new NotImplementedException();
        }

        private int SaveTermTranslationToDB(Language lang, EntityTranslation translationForLang)
        {
            throw new NotImplementedException();
        }

        private int SaveToDB(Term newTerm)
        {
            throw new NotImplementedException();
        }

        private int SaveSemanticTranslationToDB(EntityTranslation semantic)
        {
            //semantic.DBOperations.InsertEntityQuery
        }
    }
}
