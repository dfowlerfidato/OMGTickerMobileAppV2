using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace OMGTickerMobileAppV2
{
    public class LocationNotificationSendPage : ContentPage
    {
        private OMGITServiceClient _client;
        public static readonly EndpointAddress EndPoint = new EndpointAddress("https://webapps.oregonmed.net/OMGITWebservices/OMGMobileServices.svc");
        BasicHttpBinding binding = CreateBasicHttp();

        Entry SubjectMessage = new Entry { Placeholder = "         SUBJECT         ", HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.Center };

        Entry NotificationMessage = new Entry { Placeholder = "         MESSAGE         ", HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.Center };
        Picker LocationPicker = new Picker { SelectedIndex = 0,  HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.Center };

        Button sendNotificationPush = new Button { Text = "Send Notification Message", TextColor = Color.White, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, BackgroundColor = Color.FromHex("14986D") };

        StackLayout adminLayout = new StackLayout { HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand, BackgroundColor = Color.FromHex("1a1a1a"), Padding = 10 };
        Label locationLabel = new Label();
        System.Collections.Generic.Dictionary<int, string> validLocations = new Dictionary<int, string>();

        public LocationNotificationSendPage()
        {
            Title = "Notify";
            locationLabel.Text = "Location to Send to:";

            _client = new OMGITServiceClient(binding, EndPoint);
            //System.Collections.Generic.SortedDictionary<int,string> validLocations = (SortedDictionary<int,string>)
                
            _client.GetValidSendLocationsCompleted += ClientSendLocationsCompleted;
            _client.GetValidSendLocationsAsync(GlobalData.loginData);
    

            LocationPicker.Title = "Select Location to Send to:";
            while (validLocations.Count < 1)
            {
                Task.Delay(100).Wait();
            }

            foreach (KeyValuePair<int,string> entry in validLocations)
            {
                LocationPicker.Items.Add(entry.Value);
                
            }


            sendNotificationPush.Clicked += SendNotification_Clicked;
            adminLayout.Children.Add(SubjectMessage);
            adminLayout.Children.Add(NotificationMessage);
            adminLayout.Children.Add(locationLabel);
            adminLayout.Children.Add(LocationPicker);
            adminLayout.Children.Add(sendNotificationPush);
            ScrollView adminView = new ScrollView { BackgroundColor = Color.FromHex("14986D"), Content = adminLayout };
            Content = adminView;
            Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0);
        }

        private void ClientSendLocationsCompleted(object sender, GetValidSendLocationsCompletedEventArgs e)
        {
            validLocations = e.Result;
        }


        private void SendNotification_Clicked(object sender, EventArgs e)
        {
            OMGITWebServices.OMGLocationNotificationMessage lm = new OMGITWebServices.OMGLocationNotificationMessage();
            lm.Message = NotificationMessage.Text;
            lm.Subject = SubjectMessage.Text;

            string selLocation = (string)LocationPicker.Items[LocationPicker.SelectedIndex];
            foreach (KeyValuePair<int, string> entry in validLocations)
            {
                if (entry.Value == selLocation)
                    lm.ClinicID = entry.Key;

            }
       //     lm.ClinicID = 8;
            _client = new OMGITServiceClient(binding, EndPoint);
            _client.SendOMGNotificationToLocationsAsync(lm, GlobalData.loginData);
            DisplayAlert("NotificationSent", "Location Notification was successfully sent", "Close");
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
