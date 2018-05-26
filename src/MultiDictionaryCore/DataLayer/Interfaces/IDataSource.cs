﻿using System;
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
        List<TermTranslation> GetTranslationsForTerm(int termId);
        List<Term> GetChildrenTerms(int parentTermId);
        List<Term> GetAllChildrenTerms();
        List<TermTranslation> GetChildrenTranslations(int languageId, int parentTermId);
        List<TermTranslation> GetChildrenTranslations(int languageId);
        int AddTranslation(TermTranslation translation);
        SemanticTranslation GetTermSemantic(int termId);
        TopicTranslation GetTermTopic(int termId);
        List<SemanticTranslation> GetSemantics(int langId);
        List<PartOfSpeechTranslation> GetLangParts(int langId);
        List<ChangeableTranslation> GetChangeables(int langId);
        List<TopicTranslation> GetTopics(int langId);
    }
}
