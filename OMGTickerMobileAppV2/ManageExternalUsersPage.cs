using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace OMGTickerMobileAppV2
{
    public class ManageExternalUsersPage : ContentPage
    {

        private OMGITServiceClient _client;
        public static readonly EndpointAddress EndPoint = new EndpointAddress("https://webapps.oregonmed.net/OMGITWebservices/OMGMobileServices.svc");
        BasicHttpBinding binding = CreateBasicHttp();

        Button Logout = new Button { Text = "Logout", TextColor = Color.White, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center, BackgroundColor = Color.FromHex("14986D") };

        public ManageExternalUsersPage()
        {
            _client = new OMGITServiceClient(binding, EndPoint);
        }

        private static BasicHttpBinding CreateBasicHttp()
        {
            BasicHttpBinding binding = new BasicHttpBinding
            {
                Name = "basicHttpBinding",
                MaxBufferSize = 2147483647,
                MaxReceivedMessageSize = 2147483647
            };
            TimeSpan timeout = new TimeSpan(0, 0, 30);
            binding.SendTimeout = timeout;
            binding.OpenTimeout = timeout;
            binding.ReceiveTimeout = timeout;
            return binding;
        }
    }
}
