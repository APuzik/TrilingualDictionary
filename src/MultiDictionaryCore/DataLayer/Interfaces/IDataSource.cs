using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiDictionaryCore.Core;
using MultiDictionaryCore.DBEntities;

namespace MultiDictionaryCore.DataLayer.Interfaces
{
    public interface IDataSource
    {
        List<TermTranslation> GetAllTranslations(int languageId);
        Term GetTerm(TermTranslation translation);
        List<TermTranslation> GetTranslationsForTerm(int languageId, int termId);
        List<Term> GetChildrenTerms(int parentTermId);
        List<Term> GetAllChildrenTerms();
        List<TermTranslation> GetChildrenTranslations(int languageId, int parentTermId);
        List<TermTranslation> GetChildrenTranslations(int languageId);
        int AddTranslation(TermTranslation translation);
    }
}
