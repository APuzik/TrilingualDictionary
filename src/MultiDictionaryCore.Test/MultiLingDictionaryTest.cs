using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MultiDictionaryCore.Test
{
    [TestClass]
    public class MultiLingDictionaryTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            //MultiLingDictionary dict = new MultiLingDictionary();
            //int testTermId = 1;
            //int testLangId = 1;
            //string testTranslation = "Translation1";

            //TermTranslation item = new TermTranslation
            //{
            //    Translation = new EntityTranslation
            //    {
            //        Id = testTermId,
            //        Language = new Language { Id = testLangId },
            //        Value = testTranslation
            //    }
            //};
            //Term expected = new Term
            //{
            //    Id = testTermId
            //};
            //dict.AddTerm(null, item);
            //Term actual = dict.GetTerm(testLangId, testTranslation);
            //AssertTerms(expected, actual);
        }

        private void AssertTerms(Term1 expected, Term1 actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Semantic, actual.Semantic);
            Assert.AreEqual(expected.Topic, actual.Topic);
        }
    }
}
