using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace OMGTickerMobileAppV2
{
    public class TestSendPage : ContentPage
    {
        private OMGITServiceClient _client;
        public static readonly EndpointAddress EndPoint = new EndpointAddress("https://webapps.oregonmed.net/OMGITWebservices/OMGMobileServices.svc");
        BasicHttpBinding binding = CreateBasicHttp();

        OMGITWebServices.OMGLocationNotificationMessage msg = new OMGITWebServices.OMGLocationNotificationMessage();

        ActivityIndicator SendingIndicator = new ActivityIndicator { IsRunning = false, Color = Color.White};

        Frame TestSendFrame = new Frame { BackgroundColor = Color.Gray, VerticalOptions = LayoutOptions.Center };
        Button TestSendButton = new Button { Text = "Send Test Notification", TextColor = Color.White, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, BackgroundColor = Color.FromHex("14986D") };

        Frame MainPageFrame = new Frame { BackgroundColor = Color.Gray, VerticalOptions = LayoutOptions.Center };
        Button MainPageButton = new Button { Text = "Back To Main Page", TextColor = Color.White, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, BackgroundColor = Color.FromHex("14986D") };

        Frame LabelFrame = new Frame { BackgroundColor = Color.Gray, VerticalOptions = LayoutOptions.Center };
        Label FinishLabel = new Label { Text = "Test Notification Sent", TextColor = Color.White, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, BackgroundColor = Color.FromHex("14986D") };

        StackLayout TestSendLayout = new StackLayout { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill };


        int TestDone = -1;
        public TestSendPage()
        {
            _client = new OMGITServiceClient(binding, EndPoint);

            TestSendButton.Clicked += TestSendButton_Clicked;
            MainPageButton.Clicked += MainPageButton_Clicked;

            TestSendFrame.Content = TestSendButton;
            MainPageFrame.Content = MainPageButton;
            LabelFrame.Content = FinishLabel;

            TestSendLayout.Children.Add(TestSendFrame);
            Content = TestSendLayout;
        }

        private void TestSendButton_Clicked(object sender, EventArgs e)
        {
            TestSendButton.IsEnabled = false;

            _client.SendTestNotificationToUserCompleted += ClientSendTestNotificationToUserCompleted;
            _client.SendTestNotificationToUserAsync(GlobalData.loginData);

            while (TestDone == -1)
            {
                continue;
            }


            TestSendLayout.Children.Remove(TestSendFrame);
            TestSendLayout.Children.Add(LabelFrame);

            Device.BeginInvokeOnMainThread(() =>
            {
                this.Content = TestSendLayout;
            });
             
        }

        private void MainPageButton_Clicked(object sender, EventArgs e)
        {
            MainPageButton.IsEnabled = false;

            Navigation.PopAsync();
        }
        private void ClientSendTestNotificationToUserCompleted(object sender, SendTestNotificationToUserCompletedEventArgs e)
        {
            try
            {
                //TestDone = e.Result;
                TestDone = 1;
            }
            catch (Exception ex)
            {
                _client.AddLogMessageMobileAsync("In Test Send Page ClientSendTestNotificationToUserCompleted. Error: ", ex.Message.ToString(), 0, GlobalData.loginData);
                _client.AddLogMessageMobileAsync("In Test Send Page ClientSendTestNotificationToUserCompleted. Inner Exception Error: ", ex.InnerException.ToString(), 0, GlobalData.loginData);

            }
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
