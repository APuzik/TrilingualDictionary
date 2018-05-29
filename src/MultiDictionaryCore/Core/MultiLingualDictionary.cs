using MultiDictionaryCore.Core.Interfaces;
using MultiDictionaryCore.DataLayer;
using MultiDictionaryCore.DataLayer.Interfaces;
using MultiDictionaryCore.DBEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiDictionaryCore.Core
{
    public class MultiLingualDictionary : IMultiLingualDictionary
    {
        public IDataSource DataSource { get; set; } = new SQLCEDataSource(string.Empty);
        public ITermFactory TermFactory { get; set; }

        public List<TermTranslation> GetAllTranslations(int languageId)
        {
            return DataSource.GetAllTranslations(languageId);
        }

        public List<TermTranslation> GetTranslationsForTerm(int languageId, int termId)
        {
            return DataSource.GetTranslationsForTerm(languageId, termId);
        }

        public List<TermTranslation> GetTranslationsForTerm(int termId)
        {
            return DataSource.GetTranslationsForTerm(termId);
        }

        public List<TermTranslation> GetChildrenTranslations(int languageId, int parentTermId)
        {
            return DataSource.GetChildrenTranslations(languageId, parentTermId);
        }

        public List<TermTranslation> GetChildrenTranslations(int languageId)
        {
            return DataSource.GetChildrenTranslations(languageId);
        }

        public List<Term> GetTopTerms(int languageId)
        {
            return DataSource.GetChildrenTerms(0);
        }
        public List<Term> GetAllChildrenTerms()
        {
            return DataSource.GetAllChildrenTerms();
        }

        public List<TermTranslation> GetTopTranslations(int languageId)
        {
            return DataSource.GetChildrenTranslations(languageId, 0);
        }

        public List<TermTranslation> GetTopTranslations(int languageId, string startsWith)
        {
            return DataSource.GetChildrenTranslations(languageId, 0, startsWith);
        }

        public Term GetTermByWord(TermTranslation translation)
        {
            return DataSource.GetTerm(translation);
        }

        public void AddTranslation(TermTranslation translation)
        {
            DataSource.AddTranslation(translation);
        }

        public SemanticTranslation GetTermSemantic(int termId)
        {
            return DataSource.GetTermSemantic(termId);
        }

        public TopicTranslation GetTermTopic(int termId)
        {
            return DataSource.GetTermTopic(termId);
        }

        public List<TopicTranslation> GetTopics(int langId)
        {
            return DataSource.GetTopics(langId);
        }
        public List<ChangeableTranslation> GetChangeables(int langId)
        {
            return DataSource.GetChangeables(langId);
        }
        public List<PartOfSpeechTranslation> GetLangParts(int langId)
        {
            return DataSource.GetLangParts(langId);
        }
        public List<SemanticTranslation> GetSemantics(int langId)
        {
            return DataSource.GetSemantics(langId);
        }

        public Term AddTerm(Term term)
        {
            return DataSource.AddTerm(term);
        }

        public void UpdateTranslation(TermTranslation translation)
        {
            DataSource.UpdateTranslation(translation);
        }

        public Term GetTermById(int termId)
        {
            return DataSource.GetTermById(termId);
        }

        public void UpdateTerm(Term term)
        {
            DataSource.UpdateTerm(term);
        }

        public void DeleteTranslation(int id)
        {
            DataSource.DeleteTranslation(id);
        }

        public void DeleteTerm(int termId)
        {
            DataSource.DeleteTerm(termId);
        }
    }
}
