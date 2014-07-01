using TrilingualDictionaryCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TrilingualDictionaryCoreTest
{
    
    
    /// <summary>
    ///This is a test class for PlaintTextDataLoaderTest and is intended
    ///to contain all PlaintTextDataLoaderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PlaintTextDataLoaderTest
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
        ///A test for ExtractAreaFlag
        ///</summary>
        [TestMethod()]
        public void ExtractAreaFlagTest()
        {
            TrilingualDictionary dictionary = null; // TODO: Initialize to an appropriate value
            PlaintTextDataLoader target = new PlaintTextDataLoader(dictionary); // TODO: Initialize to an appropriate value
            string textRus = string.Empty; // TODO: Initialize to an appropriate value
            string partUkr = "мат., техн. шу#кане"; // TODO: Initialize to an appropriate value
            string expected = "мат., техн."; // TODO: Initialize to an appropriate value
            string actual = target.ExtractTopics(partUkr);
            Assert.AreEqual(expected, actual);

            partUkr = "мат., техн., кв. рф шу#кане"; // TODO: Initialize to an appropriate value
            expected = "мат., техн., кв. рф"; // TODO: Initialize to an appropriate value
            actual = target.ExtractTopics(partUkr);
            Assert.AreEqual(expected, actual);

            partUkr = "кв. рф шу#кане"; // TODO: Initialize to an appropriate value
            expected = "кв. рф"; // TODO: Initialize to an appropriate value
            actual = target.ExtractTopics(partUkr);
            Assert.AreEqual(expected, actual);
           
        }

        /// <summary>
        ///A test for ExtractChangableUkr
        ///</summary>
        [TestMethod()]
        public void ExtractChangableUkrTest()
        {
            TrilingualDictionary dictionary = null;
            PlaintTextDataLoader target = new PlaintTextDataLoader(dictionary);
            string partUkr = "я#кість, род. я#кості";
            //string expected = ", род. я#кості";
            string expected = "я#кості";
            string actual = target.ExtractChangableUkr(partUkr);
            Assert.AreEqual(expected, actual);

            partUkr = "автоперевi#д, -во#ду";
            //expected = ", -во#ду";
            expected = "-во#ду";
            actual = target.ExtractChangableUkr(partUkr);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ExtractLinkPart
        ///</summary>
        [TestMethod()]
        public void ExtractLinkPartTest()
        {
            TrilingualDictionary dictionary = null;
            PlaintTextDataLoader target = new PlaintTextDataLoader(dictionary);
            string text = "зало#млення (зало#млювання); (см. ещё рефра#кция)";
            string expected = "(см. ещё рефра#кция)";
            string actual = target.ExtractLinkPart(text);
            Assert.AreEqual(expected, actual);

            text = "ослаби#тель (см. аттенюа#тор)";
            expected = "(см. аттенюа#тор)";
            actual = target.ExtractLinkPart(text);
            Assert.AreEqual(expected, actual);

            text = "см. восьмери#чный";
            expected = "см. восьмери#чный";
            actual = target.ExtractLinkPart(text);
            Assert.AreEqual(expected, actual);

            text = "апроксима#ція; см. ещё приближе#ние";
            expected = "см. ещё приближе#ние";
            actual = target.ExtractLinkPart(text);
            Assert.AreEqual(expected, actual);            
        }

        /// <summary>
        ///A test for ExtractParentheses
        ///</summary>
        [TestMethod()]
        public void ExtractParenthesesTest()
        {
            TrilingualDictionary dictionary = null; // TODO: Initialize to an appropriate value
            PlaintTextDataLoader target = new PlaintTextDataLoader(dictionary); // TODO: Initialize to an appropriate value
            string textRus = "~ вы#нужденных (затуха#ющих, свобо#дных, со#бственных) колеба#ний";
            string clearUkr = "ампліту#да ви#мушених (з(а)гаса#ючих, вi#льних, вла#сних) колива#нь";
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual = target.ExtractParentheses(textRus, clearUkr);
            Assert.AreEqual(expected, actual);

            textRus = "андалузи#т";
            clearUkr = "андалузи#т, -ту (минерал)";
            expected = "(минерал)"; // TODO: Initialize to an appropriate value
            actual = target.ExtractParentheses(textRus, clearUkr);
            Assert.AreEqual(expected, actual);

            textRus = "техни#ческая ~";
            clearUkr = "(98066,5 Па) технi#чна атмосфе#ра";
            expected = "(98066,5 Па)"; // TODO: Initialize to an appropriate value
            actual = target.ExtractParentheses(textRus, clearUkr);
            Assert.AreEqual(expected, actual);

        }
    }
}
