using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace WorkManager.Tests
{
    [TestClass]
    abstract public class AbstractIntegerServiceAwareTestCase
    {
        protected IntegerWorkManager Manager { get; set; }

        [TestInitialize]
        public void SetUp()
        {
            
        }

        [TestCleanup]
        public void TearDown()
        {
            Mock.Reset();
        }

    }
}
