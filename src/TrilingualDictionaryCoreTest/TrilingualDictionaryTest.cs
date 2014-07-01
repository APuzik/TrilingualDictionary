using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TrilingualDictionaryCore;
using System.Collections.Generic;
using System.Threading;

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
            Conception.LanguageId languageIdRus = Conception.LanguageId.Russian;
            m_Dictionary.AddConception(wordRus, languageIdRus);

            int excpected = 1;
            int actual = m_Dictionary.ConceptionsCount;
            Assert.AreEqual(excpected, actual);
        }

        /// <summary>
        ///A test for AddConception
        ///</summary>
        [TestMethod()]
        public void AddConceptionTest()
        {
            int expected = 1;
            int actual = AddConceptionRus();
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected, m_Dictionary.ConceptionsCount); 
        }

        /// <summary>
        ///A test for AddDescriptionToConception
        ///</summary>
        [TestMethod()]
        public void AddDescriptionToConceptionTest()
        {
            int conceptionId = AddConceptionRus();

            string wordEng = "Generator";
            Conception.LanguageId languageIdEng = Conception.LanguageId.English;
            m_Dictionary.AddDescriptionToConception(conceptionId, wordEng, languageIdEng);

            int expected = 1;
            Assert.AreEqual(expected, m_Dictionary.ConceptionsCount); 
        }

        [TestMethod()]
        [ExpectedException(typeof(TriLingException))]
        public void AddDescriptionToAbsentConceptionTest()
        {
            int conceptionId = AddConceptionRus();

            string wordEng = "Generator";
            Conception.LanguageId languageIdEng = Conception.LanguageId.English;
            int nonExistingConceptionId = 2;
            m_Dictionary.AddDescriptionToConception(nonExistingConceptionId, wordEng, languageIdEng);
        }

        [TestMethod()]
        [ExpectedException(typeof(TriLingException))]
        public void AddDescriptionOfExisitingLanguageToConceptionTest()
        {
            int conceptionId = AddConceptionRus();

            string word = "Генератор2";
            Conception.LanguageId languageIdRus = Conception.LanguageId.Russian;
            m_Dictionary.AddDescriptionToConception(conceptionId, word, languageIdRus);
        }

        private int AddConceptionRus()
        {
            string word = "Генератор";
            Conception.LanguageId languageId = Conception.LanguageId.Russian;

            int conceptionId = m_Dictionary.AddConception(word, languageId);
            return conceptionId;
        }

        /// <summary>
        ///A test for ChangeDescriptionOfConception
        ///</summary>
        [TestMethod()]
        public void ChangeDescriptionOfConceptionTest()
        {
            int conceptionId = AddConceptionRus();

            string word = "Генерато2";
            Conception.LanguageId languageIdEng = Conception.LanguageId.Russian;
            m_Dictionary.ChangeDescriptionOfConception(conceptionId, word, languageIdEng);

            int expected = 1;
            Assert.AreEqual(expected, m_Dictionary.ConceptionsCount);             
        }

        [TestMethod()]
        [ExpectedException(typeof(TriLingException))]
        public void ChangeDescriptionOfAbsentConceptionTest()
        {
            int conceptionId = AddConceptionRus();

            string word = "Генерато2";
            Conception.LanguageId languageIdEng = Conception.LanguageId.Russian;
            int nonExistingConceptionId = 2;
            m_Dictionary.ChangeDescriptionOfConception(nonExistingConceptionId, word, languageIdEng);
        }

        /// <summary>
        ///A test for RemoveConception
        ///</summary>
        [TestMethod()]
        public void RemoveConceptionTest()
        {            
            int conceptionId = AddConceptionRus();
            int expected = 1;
            Assert.AreEqual(expected, m_Dictionary.ConceptionsCount);

            m_Dictionary.RemoveConception(conceptionId);
            expected = 0;
            Assert.AreEqual(expected, m_Dictionary.ConceptionsCount);
        }

        [TestMethod()]
        public void RemoveAbsentConceptionTest()
        {
            int conceptionId = AddConceptionRus();
            int expected = 1;
            Assert.AreEqual(expected, m_Dictionary.ConceptionsCount);

            int nonExistingConceptionId = 2;
            m_Dictionary.RemoveConception(nonExistingConceptionId);
            Assert.AreEqual(expected, m_Dictionary.ConceptionsCount);
        }

        /// <summary>
        ///A test for RemoveDescriptionFromConception
        ///</summary>
        [TestMethod()]
        public void RemoveDescriptionFromConceptionTest()
        {
            int conceptionId = AddConceptionRus();

            string wordEng = "Generator";
            Conception.LanguageId languageIdRus = Conception.LanguageId.Russian;
            Conception.LanguageId languageIdEng = Conception.LanguageId.English;
            int expected = 1;

            m_Dictionary.AddDescriptionToConception(conceptionId, wordEng, languageIdEng);            
            Assert.AreEqual(expected, m_Dictionary.ConceptionsCount);

            m_Dictionary.RemoveDescriptionFromConception(conceptionId, languageIdEng);
            Assert.AreEqual(expected, m_Dictionary.ConceptionsCount);

            expected = 0;
            m_Dictionary.RemoveDescriptionFromConception(conceptionId, languageIdRus);
            Assert.AreEqual(expected, m_Dictionary.ConceptionsCount);
        }

        [TestMethod()]
        [ExpectedException(typeof(TriLingException))]
        public void RemoveDescriptionFromAbsentConceptionTest()
        {
            int conceptionId = AddConceptionRus();

            string wordEng = "Generator";
            Conception.LanguageId languageIdEng = Conception.LanguageId.English;
            int expected = 1;

            m_Dictionary.AddDescriptionToConception(conceptionId, wordEng, languageIdEng);
            Assert.AreEqual(expected, m_Dictionary.ConceptionsCount);

            int nonExistingConceptionId = 2;
            m_Dictionary.RemoveDescriptionFromConception(nonExistingConceptionId, languageIdEng);
        }

        /// <summary>
        ///A test for ConceptionsCount
        ///</summary>
        [TestMethod()]
        public void ConceptionsCountTest()
        {
            int expected = 0;
            Assert.AreEqual(expected, m_Dictionary.ConceptionsCount);

            int conceptionId = AddConceptionRus();
            expected = 1;
            Assert.AreEqual(expected, m_Dictionary.ConceptionsCount);
        }

        /// <summary>
        ///A test for multithreading Add/Remove
        ///</summary>
        //[TestMethod()]
        public void AddRemoveMTTest()
        {
            Thread workerThread1 = new Thread(AddConceptionsMT);
            Thread workerThread2 = new Thread(RemoveConceptionsMT);
            workerThread1.Start();
            Thread.Sleep(2000);
            workerThread2.Start();

            workerThread2.Join();
            workerThread1.Join();
        }

        private void AddConceptionsMT()
        {
            while (AddConceptionRus() < 1000000)
            {
                int k = 1;
            }

        }

        private void RemoveConceptionsMT()
        {
            Random rnd = new Random();
            do
            {
                int n = m_Dictionary.ConceptionsCount;
                int val = rnd.Next(n);
                try
                {
                    m_Dictionary.RemoveConception(val);
                }
                catch(Exception ex)
                {
                    int j = 0;
                }
            } while (m_Dictionary.ConceptionsCount > 0);
        }

        /// <summary>
        ///A test for SplitText
        ///</summary>
        [TestMethod()]
        public void SplitTextTest()
        {
            List<string> inputData = new List<string>();
            inputData.Add("АВМ (ана#логовая вычисли#тельная маши#на)");
            inputData.Add("дву(х)по#люсник");
            inputData.Add("флу[ю]ктуа#ции");

            List<List<string>> expectedData = new List<List<string>>();
            expectedData.Add(new List<string>());
            expectedData[0].Add("АВМ");
            expectedData[0].Add("(ана#логовая вычисли#тельная маши#на)");
            expectedData.Add(new List<string>());
            expectedData[1].Add("дву(х)по#люсник");
            expectedData.Add(new List<string>());
            expectedData[2].Add("флу[ю]ктуа#ции");

            
            for (int i = 0; i < inputData.Count; i++)
            {
                string text = inputData[i];
                List<string> actual = PlaintTextDataLoader.SplitText(text, " ");
                Assert.AreEqual(expectedData[i].Count, actual.Count);
                for (int j = 0; j < actual.Count; j++)
                {
                    Assert.AreEqual(expectedData[i][j], actual[j]);
                }
            }
        }
    }
}
