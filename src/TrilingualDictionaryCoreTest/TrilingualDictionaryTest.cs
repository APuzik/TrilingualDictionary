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
        public void getConceptionsCountEmptyTest()
        {
            int excpected = 0;
            int actual = m_Dictionary.getConceptionsCount();
            Assert.AreEqual(excpected, actual);
        }

        [TestMethod()]
        public void getConceptionsCountOneTest()
        {
            string wordRus = "Генератор";
            int languageIdRus = 1;
            m_Dictionary.addConception(wordRus, languageIdRus);

            int excpected = 1;
            int actual = m_Dictionary.getConceptionsCount();
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

    }
}
