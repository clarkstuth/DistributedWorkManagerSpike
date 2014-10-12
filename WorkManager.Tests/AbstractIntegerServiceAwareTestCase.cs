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
            IntegerWorkManager.AllWork.Clear();
            IntegerWorkManager.AssignedWork.Clear();
            IntegerWorkManager.UnassignedWork.Clear();

            EmptyAvailableCallbacksObject();
            Mock.Reset();
        }

        protected void EmptyAvailableCallbacksObject()
        {
            while (IntegerWorkManager.AvailableCallbacks.Count > 0)
            {
                IntegerWorkManager.AvailableCallbacks.Take();
            }
        }

    }
}
