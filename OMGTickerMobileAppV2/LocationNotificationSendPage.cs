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

        Entry SubjectMessage = new Entry { PlaceholderColor = Color.Black, Placeholder = "         SUBJECT         ", HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Center, BackgroundColor = Color.White, TextColor = Color.Black };

        Entry NotificationMessage = new Entry { PlaceholderColor = Color.Black, Placeholder = "         MESSAGE         ", HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Center, BackgroundColor = Color.White, TextColor = Color.Black };
        Picker LocationPicker = new Picker { SelectedIndex = 0, HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.Center };

        Button sendNotificationPush = new Button { Text = "Send Notification Message", TextColor = Color.White, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, BackgroundColor = Color.FromHex("14986D") };

        StackLayout adminLayout = new StackLayout { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill, BackgroundColor = Color.Black, Padding = 10 };
        Label locationLabel = new Label();
        System.Collections.Generic.Dictionary<int, string> ValidLocations = new Dictionary<int, string>();

        StackLayout AllClinicsLayout = new StackLayout { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill, BackgroundColor = Color.Black };

        Button Logout = new Button { Text = "Logout", TextColor = Color.White, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center, BackgroundColor = Color.FromHex("14986D") };

        public LocationNotificationSendPage()
        {
            Title = "Notify";
            locationLabel.Text = "Location to Send to:";

            try
            {

                _client = new OMGITServiceClient(binding, EndPoint);

                _client.AddLogMessageMobileAsync("In Location Notification Send Page", "Setting title",0,GlobalData.loginData);
                LocationPicker.Title = "Select Location to Send to:";


                //System.Collections.Generic.SortedDictionary<int,string> validLocations = (SortedDictionary<int,string>)

                _client.GetValidSendLocationsCompleted += ClientSendLocationsCompleted;
                _client.GetValidSendLocationsAsync(GlobalData.loginData);

                while (ValidLocations.Count < 1)
                {
                    Task.Delay(100).Wait();
                }

                //_client.AddLogMessageTickerAppAsync("In Location Notification Send Page", "Done getting valid locations");

                //foreach (KeyValuePair<int, string> entry in validLocations)
                //{
                //    LocationPicker.Items.Add(entry.Value);

                //}

                
                foreach (KeyValuePair<int, string> entry in ValidLocations)
                {
                    StackLayout ClinicLayout = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill };
                    ClinicLayout.Children.Add(new Switch { HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center});
                    ClinicLayout.Children.Add(new Label { Text = entry.Value, HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, TextColor = Color.White });
                    ClinicLayout.Children.Add(new Label { Text = entry.Key.ToString(), HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, IsVisible = false });
                    AllClinicsLayout.Children.Add(ClinicLayout);
                }

                _client.AddLogMessageMobileAsync("In Location Notification Send Page", "Items added to Location Picker", 0, GlobalData.loginData);
   

                sendNotificationPush.Clicked += SendNotification_Clicked;
                Logout.Clicked += Logout_Clicked;

                adminLayout.Children.Add(Logout);
                adminLayout.Children.Add(SubjectMessage);
                adminLayout.Children.Add(NotificationMessage);
                adminLayout.Children.Add(locationLabel);
                adminLayout.Children.Add(AllClinicsLayout);
                //adminLayout.Children.Add(LocationPicker);
                adminLayout.Children.Add(sendNotificationPush);
                ScrollView adminView = new ScrollView { BackgroundColor = Color.Black, Content = adminLayout };
                Content = adminView;
                Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0);


                _client.AddLogMessageMobileAsync("In Location Notification Send Page", "Done creating page", 0, GlobalData.loginData);
            }
            catch (Exception ex)
            {
                _client.AddLogMessageMobileAsync("In Location Notification Send Page. Error: ", ex.Message.ToString(), 0, GlobalData.loginData);
            }

        }
        
        private void ClientSendLocationsCompleted(object sender, GetValidSendLocationsCompletedEventArgs e)
        {
            _client.AddLogMessageMobileAsync("In Location Notification Send Page", "ClientSendLocationsCompleted Start", 0, GlobalData.loginData);

            try
            {

                ValidLocations = (Dictionary<int, string>)e.Result;

                /*
                foreach (string s in e.Result)
                {
                    _client.AddLogMessageTickerAppAsync("Result Value: ", s);
                    string [] keyvalue = s.Split('^');
                    validLocations.Add(Int32.Parse(keyvalue[0]), keyvalue[1]);
                }*/
                //validLocations[8] = "cbo-it";
            }
            catch (Exception ex)
            {
                _client.AddLogMessageMobileAsync("In Location Notification Send Page ClientSendLocationsCompleted. Error: ", ex.Message.ToString(), 0, GlobalData.loginData);
                _client.AddLogMessageMobileAsync("In Location Notification Send Page ClientSendLocationsCompleted. Inner Exception Error: ", ex.InnerException.ToString(), 0, GlobalData.loginData);

            }
            _client.AddLogMessageMobileAsync("In Location Notification Send Page. Error: ", "ClientSendLocationsCompleted End", 0, GlobalData.loginData);

        }


        private void SendNotification_Clicked(object sender, EventArgs e)
        {
            OMGITWebServices.OMGLocationNotificationMessage lm = new OMGITWebServices.OMGLocationNotificationMessage();
            lm.Message = NotificationMessage.Text;
            lm.Subject = SubjectMessage.Text;

            //string selLocation = (string)LocationPicker.Items[LocationPicker.SelectedIndex];
            //foreach (KeyValuePair<int, string> entry in ValidLocations)
            //{
            //    if (entry.Value == selLocation)
            //        lm.ClinicID = entry.Key;

            //}
            //     lm.ClinicID = 8;


            List<int> SendClinicIDs = new List<int>();

            foreach (StackLayout s in AllClinicsLayout.Children)
            {
                var curClinicName = s.Children[1] as Label;
                var curID = s.Children[2] as Label;
                var curSwitch = s.Children[0] as Switch;
                if (curSwitch.IsToggled)
                {
                    try
                    {
                        SendClinicIDs.Add(Int32.Parse(curID.Text));
                    }
                    catch (Exception ex) {; }
                }
            }

            foreach (int id in SendClinicIDs)
            {
                lm.ClinicID = id;
                _client = new OMGITServiceClient(binding, EndPoint);
                _client.SendOMGNotificationToLocationsAsync(lm, GlobalData.loginData);
                DisplayAlert("NotificationSent", "Location Notification was successfully sent", "Close");
            }
        }

        private void Logout_Clicked(object sender, EventArgs e)
        {
            Helpers.Settings.UserName = string.Empty;
            Helpers.Settings.AuthToken = string.Empty;
            Helpers.Settings.Salt = string.Empty;

            Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
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
