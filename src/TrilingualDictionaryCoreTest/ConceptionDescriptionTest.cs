using TrilingualDictionaryCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TrilingualDictionaryCoreTest
{
    
    
    /// <summary>
    ///This is a test class for ConceptionDescriptionTest and is intended
    ///to contain all ConceptionDescriptionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ConceptionDescriptionTest
    {
        ConceptionDescription m_Description = null;

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
            string word = "Генератор";
            m_Description = new ConceptionDescription(null, word);
        }
        
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for ConceptionDescription Constructor
        ///</summary>
        [TestMethod()]
        public void ConceptionDescriptionConstructorTest()
        {
            Assert.IsNotNull(m_Description);
        }

        /// <summary>
        ///A test for ChangeDescription
        ///</summary>
        [TestMethod()]
        public void ChangeDescriptionTest()
        {
            string word = "Генератор2";
            m_Description.ChangeDescription(word);
            Assert.AreEqual(word, m_Description.ConceptionRegistryDescription);
        }
    }
}
