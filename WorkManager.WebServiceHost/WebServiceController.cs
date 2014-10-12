using System;
using System.ServiceModel;

namespace WorkManager.WebServiceHost
{
    public class WebServiceController : ServiceHost
    {
        private IWorkSelector Selector { get; set; }

        public WebServiceController(IWorkSelector selector) : base(typeof (IntegerWorkManager))
        {
            Selector = selector;
        }



    }
}
