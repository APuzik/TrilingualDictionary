using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiDictionaryCore.DataLayer.Interfaces;
using MultiDictionaryCore.DataLayer;
using MultiDictionaryCore.DBEntities;
using System.Collections.Generic;

namespace MultiDictionaryCore.Test
{
    [TestClass]
    public class SQLCETest
    {
        [TestMethod]
        //[DeploymentItem(@".\TrilingualDictionary.sdf", @"Data")]
        public void GetAllTranslationsTest()
        {
            IDataSource ds = new SQLCEDataSource("");
            List<TermTranslation> allTranslations = ds.GetAllTranslations(1);
            List<TermTranslation> rusTranslations = ds.GetTranslationsForTerm(1, allTranslations[0].TermId);
            List<TermTranslation> engTranslations = ds.GetTranslationsForTerm(2, allTranslations[0].TermId);
            List<TermTranslation> ukrTranslations = ds.GetTranslationsForTerm(3, allTranslations[0].TermId);
            Assert.AreEqual(43016, allTranslations.Count);
            Assert.AreEqual("аббревиату#ра", allTranslations[0].Value);
        }
    }
}
