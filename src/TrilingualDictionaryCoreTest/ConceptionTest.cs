using TrilingualDictionaryCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace TrilingualDictionaryCoreTest
{
    
    
    /// <summary>
    ///This is a test class for ConceptionTest and is intended
    ///to contain all ConceptionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ConceptionTest
    {
        private Conception m_Conception = null;

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
        
        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
            int conceptionId = 1;
            string description = "Генератор";
            Conception.LanguageId languageId = Conception.LanguageId.Russian;
            m_Conception = new Conception(conceptionId, description, languageId);
        }
        
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Conception Constructor
        ///</summary>
        [TestMethod()]
        public void ConceptionConstructorTest()
        {
            Assert.IsNotNull(m_Conception);
            int expected = 1;
            Assert.AreEqual(expected, m_Conception.DescriptionsCount);
        }

        /// <summary>
        ///A test for AddDescription
        ///</summary>
        [TestMethod()]
        public void AddDescriptionTest()
        {
            string word = "Generator";
            Conception.LanguageId languageIdEng = Conception.LanguageId.English;
            m_Conception.AddDescription(word, languageIdEng);

            int expected = 2;
            Assert.AreEqual(expected, m_Conception.DescriptionsCount);
        }

        //[TestMethod()]
        //public void AddExisitingLanguageDescriptionTest()
        //{
        //    string word = "Генератор2";
        //    Conception.LanguageId languageIdRus = Conception.LanguageId.Russian;
        //    m_Conception.AddDescription(word, languageIdRus);
        //}

        /// <summary>
        ///A test for ChangeDescription
        ///</summary>
        [TestMethod()]
        public void ChangeDescriptionTest()
        {
            string word = "Генератор2";
            Conception.LanguageId languageIdRus = Conception.LanguageId.Russian;
            m_Conception.ChangeDescription(word, languageIdRus, 0);

            int expected = 1;
            Assert.AreEqual(expected, m_Conception.DescriptionsCount);

            CheckRegistryDescription(word, languageIdRus);
        }

        [TestMethod()]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void ChangeAbsentDescriptionTest()
        {
            string word = "Generator";
            Conception.LanguageId languageIdEng = Conception.LanguageId.English;
            m_Conception.ChangeDescription(word, languageIdEng, 0);
        }

        private void CheckRegistryDescription(string word, Conception.LanguageId languageId)
        {
            ConceptionDescription description = m_Conception.GetConceptionDescription(languageId, 0);
            bool ignoreCase = true;
            Assert.AreEqual(word, description.ConceptionRegistryDescription, ignoreCase);
        }

        /// <summary>
        ///A test for GetConceptionDescription
        ///</summary>
        [TestMethod()]
        public void GetConceptionDescriptionTest()
        {
            Conception.LanguageId languageIdRus = Conception.LanguageId.Russian;
            string word = "Генератор";

            CheckRegistryDescription(word, languageIdRus);
        }

        [TestMethod()]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void GetAbsentConceptionDescriptionTest()
        {
            Conception.LanguageId languageId = Conception.LanguageId.English;
            ConceptionDescription description = m_Conception.GetConceptionDescription(languageId, 0);
        }

        /// <summary>
        ///A test for RemoveDescription
        ///</summary>
        [TestMethod()]
        public void RemoveDescriptionTest()
        {
            string word = "Generator";
            Conception.LanguageId languageIdEng = Conception.LanguageId.English;
            m_Conception.AddDescription(word, languageIdEng);

            int expected = 2;
            Assert.AreEqual(expected, m_Conception.DescriptionsCount);

            m_Conception.RemoveDescription(Conception.LanguageId.Russian, 0);

            CheckRegistryDescription(word, Conception.LanguageId.English);
        }

        /// <summary>
        ///A test for ConceptionId
        ///</summary>
        [TestMethod()]
        public void ConceptionIdTest()
        {
            int expected = 1;
            Assert.AreEqual(expected, m_Conception.ConceptionId);
        }
    }
}
