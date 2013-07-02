using DMCBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DMCBaseUnitTest
{
    
    
    /// <summary>
    ///This is a test class for EventArgsTest and is intended
    ///to contain all EventArgsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class EventArgsTest
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
        ///A test for EventArgs`1 Constructor
        ///</summary>
        public void EventArgsConstructorTestHelper<T>()
        {
            T value = default(T); // TODO: Initialize to an appropriate value
            EventArgs<T> target = new EventArgs<T>(value);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        [TestMethod()]
        public void EventArgsConstructorTest()
        {
            EventArgsConstructorTestHelper<GenericParameterHelper>();
        }

        /// <summary>
        ///A test for Value
        ///</summary>
        public void ValueTestHelper<T>()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            EventArgs_Accessor<T> target = new EventArgs_Accessor<T>(param0); // TODO: Initialize to an appropriate value
            T expected = default(T); // TODO: Initialize to an appropriate value
            T actual;
            target.Value = expected;
            actual = target.Value;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        [TestMethod()]
        [DeploymentItem("DMCBase.dll")]
        public void ValueTest()
        {
            ValueTestHelper<GenericParameterHelper>();
        }
    }
}
