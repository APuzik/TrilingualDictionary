using TrilingualDictionaryCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TrilingualDictionaryCoreTest
{
    
    
    /// <summary>
    ///This is a test class for ConceptionTest and is intended
    ///to contain all ConceptionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ConceptionTest
    {


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
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
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
            int conceptionId = 0; // TODO: Initialize to an appropriate value
            string word = string.Empty; // TODO: Initialize to an appropriate value
            int languageId = 0; // TODO: Initialize to an appropriate value
            Conception target = new Conception(conceptionId, word, languageId);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for AddDescription
        ///</summary>
        [TestMethod()]
        public void AddDescriptionTest()
        {
            int conceptionId = 0; // TODO: Initialize to an appropriate value
            string word = string.Empty; // TODO: Initialize to an appropriate value
            int languageId = 0; // TODO: Initialize to an appropriate value
            Conception target = new Conception(conceptionId, word, languageId); // TODO: Initialize to an appropriate value
            string word1 = string.Empty; // TODO: Initialize to an appropriate value
            int languageId1 = 0; // TODO: Initialize to an appropriate value
            target.AddDescription(word1, languageId1);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for ChangeDescription
        ///</summary>
        [TestMethod()]
        public void ChangeDescriptionTest()
        {
            int conceptionId = 0; // TODO: Initialize to an appropriate value
            string word = string.Empty; // TODO: Initialize to an appropriate value
            int languageId = 0; // TODO: Initialize to an appropriate value
            Conception target = new Conception(conceptionId, word, languageId); // TODO: Initialize to an appropriate value
            string word1 = string.Empty; // TODO: Initialize to an appropriate value
            int languageId1 = 0; // TODO: Initialize to an appropriate value
            target.ChangeDescription(word1, languageId1);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for GetConceptionDescription
        ///</summary>
        [TestMethod()]
        public void GetConceptionDescriptionTest()
        {
            int conceptionId = 0; // TODO: Initialize to an appropriate value
            string word = string.Empty; // TODO: Initialize to an appropriate value
            int languageId = 0; // TODO: Initialize to an appropriate value
            Conception target = new Conception(conceptionId, word, languageId); // TODO: Initialize to an appropriate value
            int languageId1 = 0; // TODO: Initialize to an appropriate value
            ConceptionDescription expected = null; // TODO: Initialize to an appropriate value
            ConceptionDescription actual;
            actual = target.GetConceptionDescription(languageId1);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for RemoveDescription
        ///</summary>
        [TestMethod()]
        public void RemoveDescriptionTest()
        {
            int conceptionId = 0; // TODO: Initialize to an appropriate value
            string word = string.Empty; // TODO: Initialize to an appropriate value
            int languageId = 0; // TODO: Initialize to an appropriate value
            Conception target = new Conception(conceptionId, word, languageId); // TODO: Initialize to an appropriate value
            int languageId1 = 0; // TODO: Initialize to an appropriate value
            target.RemoveDescription(languageId1);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for ConceptionId
        ///</summary>
        [TestMethod()]
        public void ConceptionIdTest()
        {
            int conceptionId = 0; // TODO: Initialize to an appropriate value
            string word = string.Empty; // TODO: Initialize to an appropriate value
            int languageId = 0; // TODO: Initialize to an appropriate value
            Conception target = new Conception(conceptionId, word, languageId); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.ConceptionId;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
