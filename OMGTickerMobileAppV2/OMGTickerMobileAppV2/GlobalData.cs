using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace OMGTickerMobileAppV2
{
    public static class GlobalData
    {
        public static OMGITWebServices.OMGLoginData loginData = new OMGITWebServices.OMGLoginData();
        public static readonly EndpointAddress EndPoint = new EndpointAddress("https://webapps.oregonmed.net/OMGITWebservices/OMGMobileServices.svc");
        public static BasicHttpBinding binding = new BasicHttpBinding
        {
            Name = "basicHttpBinding",
            MaxBufferSize = 2147483647,
            MaxReceivedMessageSize = 2147483647
        };

        //   _client = new OMGITServiceClient(binding, EndPoint)


        public static void SetLoginData(OMGITWebServices.OMGLoginData _loginData)
        {
            loginData = _loginData;
            TimeSpan timeout = new TimeSpan(0, 0, 30);
            binding.SendTimeout = timeout;
            binding.OpenTimeout = timeout;
            binding.ReceiveTimeout = timeout;

        }

    }


}
