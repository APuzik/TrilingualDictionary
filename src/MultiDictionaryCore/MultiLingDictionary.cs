//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace MultiDictionaryCore
//{
//    public class MultiLingDictionary
//    {
//        Dictionary<int, Term> terms = new Dictionary<int, Term>();
//        Dictionary<string, List<Term>> termByTranslation = new Dictionary<string, List<Term>>();

//        Dictionary<int, EntityTranslation> termTranslations = new Dictionary<int, EntityTranslation>();

//        Dictionary<int, Language> languages = new Dictionary<int, Language>();        
//        Dictionary<int, EntityTranslation> langTranslations = new Dictionary<int, EntityTranslation>();

//        Dictionary<int, SemanticDescription> semantics = new Dictionary<int, SemanticDescription>();        
//        Dictionary<int, EntityTranslation> semanticTranslations = new Dictionary<int, EntityTranslation>();

//        Dictionary<int, TopicDescription> topics = new Dictionary<int, TopicDescription>(); 
//        Dictionary<int, EntityTranslation> topicTranslations = new Dictionary<int, EntityTranslation>();

//        Dictionary<int, PartOfSpeech> partsOfSpeech = new Dictionary<int, PartOfSpeech>(); 
//        Dictionary<int, EntityTranslation> partOfSpeechTranslations = new Dictionary<int, EntityTranslation>();

//        Dictionary<int, ChangPartType> changTypes = new Dictionary<int, ChangPartType>(); 
//        Dictionary<int, EntityTranslation> changTypeTranslations = new Dictionary<int, EntityTranslation>();

//        Dictionary<int, ChangPart> changeables = new Dictionary<int, ChangPart>(); 
//        //Dictionary<int, EntityTranslation> changeableTranslations = new Dictionary<int, EntityTranslation>();

//        public IDataSource Database { get; set; }
//        public ITermFactory TermFactory { get; set; }

//        public bool AddTranslation(Term term, TermTranslation item)
//        {
//            term.AddTranslation(item);
//            return true;
//        }

//        public Term AddTerm(Term parent, TermTranslation item)
//        {
//            Term term = TermFactory.CreateTerm();

//            AddTranslation(term, item);
//            terms.Add(term.Id, term);

//            return term;
//        }

//        public Term GetTerm(int languageId, string translation)
//        {
            
//        }

//        bool AddTerm(SemanticDescription sem, TopicDescription topic, Term parent)
//        {
//            try
//            {
//                Term newTerm = new Term
//                {
//                    Topic = topic,
//                    Semantic = sem,
//                    Parent = parent
//                };
//                int id = SaveToDB(newTerm);
//                newTerm.Id = id;

//                terms.Add(id, newTerm);
//                return true;
//            }
//            catch(Exception ex)
//            {

//            }
//            return false;
//        }

//        bool AddTermTranslation(Language lang, Term term, string translation)
//        {
//            try
//            {
//                EntityTranslation newTermTranslation = new EntityTranslation
//                {
//                    Entity = term,
//                    Value= translation
//                };
//                int id = SaveTermTranslationToDB(lang, newTermTranslation);
//                newTermTranslation.Id = id;

//                termTranslations.Add(id, newTermTranslation);
//                return true;
//            }
//            catch (Exception ex)
//            {

//            }
//            return false;
//        }

//        bool AddLanguage()
//        {
//            try
//            {
//                Language newLang = new Language();
//                int id = SaveLanguageToDB(newLang);
//                newLang.Id = id;

//                languages.Add(id, newLang);
//                return true;
//            }
//            catch (Exception ex)
//            {

//            }
//            return false;
//        }

//        bool AddSemantic()
//        {
//            try
//            {
//                SemanticDescription newSemantic = new SemanticDescription();
//                int id = SaveSemanticToDB(newSemantic);
//                newSemantic.Id = id;

//                semantics.Add(id, newSemantic);
//                return true;
//            }
//            catch (Exception ex)
//            {

//            }
//            return false;
//        }

//        //bool AddLanguage()
//        //{
//        //    try
//        //    {
//        //        Language newLang = new Language();
//        //        int id = SaveLanguageToDB(newLang);
//        //        newLang.Id = id;

//        //        languages.Add(id, newLang);
//        //        return true;
//        //    }
//        //    catch (Exception ex)
//        //    {

//        //    }
//        //    return false;
//        //}

//        //bool AddLanguage()
//        //{
//        //    try
//        //    {
//        //        Language newLang = new Language();
//        //        int id = SaveLanguageToDB(newLang);
//        //        newLang.Id = id;

//        //        languages.Add(id, newLang);
//        //        return true;
//        //    }
//        //    catch (Exception ex)
//        //    {

//        //    }
//        //    return false;
//        //}

//        private int SaveLanguageToDB(Language newLang)
//        {
//            throw new NotImplementedException();
//        }

//        private int SaveTermTranslationToDB(Language lang, EntityTranslation translationForLang)
//        {
//            throw new NotImplementedException();
//        }

//        private int SaveToDB(Term newTerm)
//        {
//            throw new NotImplementedException();
//        }

//        private int SaveSemanticTranslationToDB(EntityTranslation semantic)
//        {
//            //semantic.DBOperations.InsertEntityQuery
//        }
//    }

//    public interface ITermFactory
//    {
//        IDataSource DataSource { get; set; }
//        Term CreateTerm();
//    }

//    internal class TermFactory : ITermFactory
//    {
//        public Term CreateTerm()
//        {
//            return new Term();
//        }
//    }



//}
