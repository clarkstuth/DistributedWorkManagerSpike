using System.ServiceModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace WorkManager.WebServiceHost.Tests
{
    [TestClass]
    public class WebServiceControllerTest
    {
        private IWorkSelector WorkSelector { get; set; }
        private WebServiceController Controller { get; set; }

        [TestInitialize]
        public void SetUp()
        {
            WorkSelector = Mock.Create<IWorkSelector>();
            Controller = new WebServiceController(WorkSelector);

        }

        [TestCleanup]
        public void TearDown()
        {
            Controller = null;
            WorkSelector = null;
            Mock.Reset();
        }

        [TestMethod]
        public void StartWorkingShouldStartWorking()
        {
            
        }

    }
}
