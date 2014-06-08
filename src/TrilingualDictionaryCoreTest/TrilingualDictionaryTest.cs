using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TrilingualDictionaryCore;

namespace TrilingualDictionaryCoreTest
{
    
    
    /// <summary>
    ///This is a test class for TrilingualDictionaryTest and is intended
    ///to contain all TrilingualDictionaryTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TrilingualDictionaryTest
    {
        TrilingualDictionary m_Dictionary;

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
            m_Dictionary = new TrilingualDictionary();
        }
        
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for TrilingualDictionary Constructor
        ///</summary>
        [TestMethod()]
        public void TrilingualDictionaryConstructorTest()
        {
            Assert.IsNotNull(m_Dictionary);
        }

        [TestMethod()]
        public void GetConceptionsCountEmptyTest()
        {
            int excpected = 0;
            int actual = m_Dictionary.ConceptionsCount;
            Assert.AreEqual(excpected, actual);
        }

        [TestMethod()]
        public void GetConceptionsCountOneTest()
        {
            string wordRus = "Генератор";
            int languageIdRus = 1;
            m_Dictionary.AddConception(wordRus, languageIdRus);

            int excpected = 1;
            int actual = m_Dictionary.ConceptionsCount;
            Assert.AreEqual(excpected, actual);
        }

        //[TestMethod()]
        //public void addNewConceptionTest()
        //{            
        //    string wordRus = "Генератор";
        //    string wordUkr = "Генератор";
        //    string wordEng = "Generator";
        //    int languageIdRus = 1;
        //    int languageIdUkr = 2;
        //    int languageIdEng = 3;

        //    int conceptionCountBefore = m_Dictionary.getConceptionsCount();
        //    int conceptionId = m_Dictionary.addConception(wordRus, languageIdRus);
        //    int conceptionCountAfter = m_Dictionary.getConceptionsCount();

        //    int excpected = conceptionCountBefore + 1;
        //    int actual = conceptionCountAfter;
        //    Assert.AreEqual(excpected, actual);
        //}


        //[TestMethod()]
        //public int getConceptionIdTest()
        //{
        //    TrilingualDictionary target = new TrilingualDictionary();
        //    string word = "Генератор";
        //    int languageId = 1;
        //    int conceptionId = m_Dictionary.getConceptionId(word, languageId);

        //}


        /// <summary>
        ///A test for AddConception
        ///</summary>
        [TestMethod()]
        public void AddConceptionTest()
        {
            TrilingualDictionary target = new TrilingualDictionary(); // TODO: Initialize to an appropriate value
            string word = string.Empty; // TODO: Initialize to an appropriate value
            int languageId = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.AddConception(word, languageId);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for TrilingualDictionary Constructor
        ///</summary>
        [TestMethod()]
        public void TrilingualDictionaryConstructorTest1()
        {
            TrilingualDictionary target = new TrilingualDictionary();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for AddDescriptionToConception
        ///</summary>
        [TestMethod()]
        public void AddDescriptionToConceptionTest()
        {
            TrilingualDictionary target = new TrilingualDictionary(); // TODO: Initialize to an appropriate value
            int conceptionId = 0; // TODO: Initialize to an appropriate value
            string word = string.Empty; // TODO: Initialize to an appropriate value
            int languageId = 0; // TODO: Initialize to an appropriate value
            target.AddDescriptionToConception(conceptionId, word, languageId);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for ChangeDescriptionOfConception
        ///</summary>
        [TestMethod()]
        public void ChangeDescriptionOfConceptionTest()
        {
            TrilingualDictionary target = new TrilingualDictionary(); // TODO: Initialize to an appropriate value
            int conceptionId = 0; // TODO: Initialize to an appropriate value
            string word = string.Empty; // TODO: Initialize to an appropriate value
            int languageId = 0; // TODO: Initialize to an appropriate value
            target.ChangeDescriptionOfConception(conceptionId, word, languageId);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for RemoveConception
        ///</summary>
        [TestMethod()]
        public void RemoveConceptionTest()
        {
            TrilingualDictionary target = new TrilingualDictionary(); // TODO: Initialize to an appropriate value
            int conceptionId = 0; // TODO: Initialize to an appropriate value
            target.RemoveConception(conceptionId);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for RemoveDescriptionFromConception
        ///</summary>
        [TestMethod()]
        public void RemoveDescriptionFromConceptionTest()
        {
            TrilingualDictionary target = new TrilingualDictionary(); // TODO: Initialize to an appropriate value
            int conceptionId = 0; // TODO: Initialize to an appropriate value
            int languageId = 0; // TODO: Initialize to an appropriate value
            target.RemoveDescriptionFromConception(conceptionId, languageId);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for ConceptionsCount
        ///</summary>
        [TestMethod()]
        public void ConceptionsCountTest()
        {
            TrilingualDictionary target = new TrilingualDictionary(); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.ConceptionsCount;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
