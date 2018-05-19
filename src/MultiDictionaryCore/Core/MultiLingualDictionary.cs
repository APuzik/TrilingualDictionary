﻿using MultiDictionaryCore.Core.Interfaces;
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

        public Term GetTermByWord(TermTranslation translation)
        {
            return DataSource.GetTerm(translation);
        }

        public void AddTranslation(TermTranslation translation)
        {
            DataSource.AddTranslation(translation);
        }
    }
}