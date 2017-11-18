using System;
using System.Collections.Generic;
using System.ServiceModel;
using Xamarin.Forms;

namespace OMGTickerMobileAppV2
{
    public class MessageHistoryPage : ContentPage
    {

        private OMGITServiceClient _client;
        public static readonly EndpointAddress EndPoint = new EndpointAddress("https://webapps.oregonmed.net/OMGITWebservices/OMGMobileServices.svc");
        BasicHttpBinding binding = CreateBasicHttp();

        StackLayout historyLayout = new StackLayout { BackgroundColor = Color.FromHex("14986D") };

        string[] msgs = new string[10];

        public MessageHistoryPage()
        {
            Title = "History";

            _client = new OMGITServiceClient(binding, EndPoint);
            _client.GetRecentMessagesAsync();
            _client.GetRecentMessagesCompleted += ClientOnGetMessagesCompleted;

            while (msgs[0] == null)
            {
                continue;
            }

            foreach (string m in msgs)
            {
                historyLayout.Children.Add(new Frame { Padding = 3, Content = new Label { Text = m, FontSize = 20, TextColor = Color.White }, BackgroundColor = Color.FromHex("1a1a1a"), OutlineColor = Color.FromHex("14986D") });
            }

            ScrollView historyView = new ScrollView { Content = historyLayout };
            Content = historyView;
            Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0);
        }

        private void ClientOnGetMessagesCompleted(object sender, GetRecentMessagesCompletedEventArgs e)
        {
            msgs = e.Result;
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
