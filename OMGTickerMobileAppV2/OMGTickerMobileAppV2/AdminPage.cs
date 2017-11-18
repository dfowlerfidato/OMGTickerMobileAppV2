using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace OMGTickerMobileAppV2
{
    public class AdminPage : ContentPage
    {

        private OMGITServiceClient _client;
        public static readonly EndpointAddress EndPoint = new EndpointAddress("https://webapps.oregonmed.net/OMGITWebservices/OMGMobileServices.svc");
        BasicHttpBinding binding = CreateBasicHttp();

        Entry testMessage = new Entry { Placeholder = "         MESSAGE         ", HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.Center };
        Entry testTags = new Entry { Placeholder = "         TAGS         ", HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.Center };

        Button sendTestPush = new Button { Text = "Send Test Push", TextColor = Color.White, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, BackgroundColor = Color.FromHex("14986D") };

        StackLayout adminLayout = new StackLayout { HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand, BackgroundColor = Color.FromHex("1a1a1a"), Padding = 10 };


        public AdminPage()
        {
            Title = "Admin";
            sendTestPush.Clicked += testPush_Clicked;
            adminLayout.Children.Add(testMessage);
            adminLayout.Children.Add(testTags);
            adminLayout.Children.Add(sendTestPush);
            ScrollView adminView = new ScrollView { BackgroundColor = Color.FromHex("14986D"), Content = adminLayout};
            Content = adminView;
            Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0);
        }

        private void testPush_Clicked (object sender, EventArgs e)
        {
            OMGITWebServices.OMGAdminTestSendData testSendData = new OMGITWebServices.OMGAdminTestSendData();
            testSendData.Message = testMessage.Text;
            testSendData.Tag = testTags.Text;
            testSendData.OperatingSystem = Device.OS.ToString();
            OMGITWebServices.OMGPushNotificationSendData OMGSendData = new OMGITWebServices.OMGPushNotificationSendData();
            OMGSendData.Message = testMessage.Text;
            OMGSendData.Tag = testTags.Text;
            OMGSendData.OperatingSystem = Device.OS.ToString();

            _client = new OMGITServiceClient(binding, EndPoint);
        //    _client.TestSendNotificationAsync(testSendData);
            _client.SendNotificationAsync(OMGSendData, GlobalData.loginData);
            DisplayAlert("Test Send", "Test push was successfully sent to " + testTags.Text, "Close");
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
