using ScansParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Collections.Generic;

namespace ScanParsersTest
{
    
    
    /// <summary>
    ///This is a test class for MSWordScansParserTest and is intended
    ///to contain all MSWordScansParserTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MSWordScansParserTest
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
        ///A test for OpenDocument
        ///</summary>
        [TestMethod()]
        [DeploymentItem(@"TestData\1.doc", @"TestData")]
        [DeploymentItem(@"TestData\2.doc", @"TestData")]
        [DeploymentItem(@"TestData\3.doc", @"TestData")]
        [DeploymentItem(@"TestData\4.doc", @"TestData")]
        [DeploymentItem(@"TestData\5.doc", @"TestData")]
        [DeploymentItem(@"TestData\6.doc", @"TestData")]
        [DeploymentItem(@"TestData\7.doc", @"TestData")]
        [DeploymentItem(@"TestData\8.doc", @"TestData")]
        [DeploymentItem(@"TestData\9.doc", @"TestData")]
        [DeploymentItem(@"TestData\10.doc", @"TestData")]
        [DeploymentItem(@"TestData\11.doc", @"TestData")]
        [DeploymentItem(@"TestData\12.doc", @"TestData")]
        [DeploymentItem(@"TestData\13.doc", @"TestData")]
        [DeploymentItem(@"TestData\14.doc", @"TestData")]
        public void OpenDocumentTest()
        {
            string [] testFiles = {
                                    @"1.doc",
                                   @"2.doc", 
                                   //@"3.doc", 
                                   //@"4.doc", 
                                   //@"5.doc", 
                                   //@"6.doc", 
                                   //@"7.doc", 
                                   //@"8.doc", 
                                   //@"9.doc", 
                                   //@"10.doc",
                                   //@"11.doc",
                                   //@"12.doc",
                                   //@"13.doc",
                                   @"14.doc"
                                  };
            
            using (MSWordScansParser target = new MSWordScansParser())
            {
                foreach(string file in testFiles)
                    target.ConvertToTxt(GetDataPath(file));
            }
        }

        private string GetDataPath(string testFile)
        {
            return Path.GetFullPath(testFile);
        }

        /// <summary>
        ///A test for Test
        ///</summary>
        [TestMethod()]
        public void TestTest()
        {
            using (MSWordScansParser target = new MSWordScansParser())
            {
                target.Test();
            }
        }
    }
}


