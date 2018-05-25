using MultiDictionaryCore.DataLayer.Interfaces;
using MultiDictionaryCore.DBEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiDictionaryCore.Core.Interfaces
{
    public interface IMultiLingualDictionary
    {
        List<TermTranslation> GetAllTranslations(int languageId);
        Term GetTermByWord(TermTranslation translation);
        List<TermTranslation> GetTopTranslations(int languageId);
        List<TermTranslation> GetChildrenTranslations(int languageId);
        List<TermTranslation> GetChildrenTranslations(int languageId, int parentTermId);
        List<TermTranslation> GetTranslationsForTerm(int languageId, int termId);
        List<TermTranslation> GetTranslationsForTerm(int termId);
        List<Term> GetTopTerms(int languageId);
        List<Term> GetAllChildrenTerms();
        ITermFactory TermFactory { get; set; }
        IDataSource DataSource { get; set; }

        void AddTranslation(TermTranslation tt);
        string GetTermSemantic(int termId);
        string GetTermTopic(int termId);
        List<string> GetTopics(int langId);
        List<string> GetChangeables(int langId);
        List<string> GetLangParts(int langId);
        List<string> GetSemantics(int langId);
    }
}
