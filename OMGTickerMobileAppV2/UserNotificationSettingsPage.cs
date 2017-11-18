using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace OMGTickerMobileAppV2
{
    public class UserNotificationSettingsPage : ContentPage
    {

        private OMGITServiceClient _client;
        public static readonly EndpointAddress EndPoint = new EndpointAddress("https://webapps.oregonmed.net/OMGITWebservices/OMGMobileServices.svc");
        BasicHttpBinding binding = CreateBasicHttp();

        Label PhoneCarrierLabel = new Label { HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, Text = "Phone Carrier: ", TextColor = Color.White};
        Picker PhoneCarrierPicker = new Picker { SelectedIndex = 0, HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center };
        StackLayout PhoneCarrierLayout = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Center };

        Label AllowTextMessageLabel = new Label { HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, Text = "Allow Text Message Alerts: ", TextColor = Color.White };
        Switch AllowTextMessageSwitch = new Switch { HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center };
        StackLayout AllowTextMessageLayout = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Center };

        Label CellPhoneLabel = new Label { HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, Text = "Phone Number: ", TextColor = Color.White };
        Entry CellPhoneEntry = new Entry { HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, Placeholder = "Numbers ONLY. No Dashes", BackgroundColor = Color.White, TextColor = Color.Black};
        StackLayout CellPhoneLayout = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Center };

        Label AllowEmailLabel = new Label { HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, Text = "Allow Email Alerts: ", TextColor = Color.White };
        Switch AllowEmailSwitch = new Switch { HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center };
        StackLayout AllowEmailLayout = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Center };

        Label EmailLabel = new Label { HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, Text = "Email Address: ", TextColor = Color.White };
        Entry EmailEntry = new Entry { HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, Placeholder = "Enter your personal email address", BackgroundColor = Color.White, TextColor = Color.Black };
        StackLayout EmailLayout = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Center };

        Label SelectClinicLabel = new Label { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, Text = "Select Clinics To Receive Notifications For", TextColor = Color.White };
        StackLayout AllClinicsLayout = new StackLayout {HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill };

        Button Logout = new Button { Text = "Logout", TextColor = Color.White, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center, BackgroundColor = Color.FromHex("14986D") };

        Button Submit = new Button { Text = "Submit Changes", TextColor = Color.White, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, BackgroundColor = Color.FromHex("14986D") };

        StackLayout NotificationSettingsLayout = new StackLayout { HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand, BackgroundColor = Color.Black, Padding = 10 };

        System.Collections.Generic.Dictionary<int, OMGITWebServices.OMGNotifyCarrierTextMapping> CarrierTextMappings = new Dictionary<int, OMGITWebServices.OMGNotifyCarrierTextMapping>();
        System.Collections.Generic.Dictionary<int, string> ValidLocations = new Dictionary<int, string>();

        ScrollView NotificationSettingsView = new ScrollView { BackgroundColor = Color.Black };

        OMGITWebServices.OMGNotifyUserOptInSettings UserSettings = new OMGITWebServices.OMGNotifyUserOptInSettings();

        int addUpdateComplete = -1;

        int addClinicComplete = -1;

        int[] UClinics;

        bool doneDelete = false;

        StackLayout ErrorLayout = new StackLayout { HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.StartAndExpand, BackgroundColor = Color.Black, Padding = 10 };
        Label ErrorMessage = new Label { HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, Text = "", TextColor = Color.Red, BackgroundColor = Color.White };
        Button ReturnButton = new Button { Text = "Back to notification settings", TextColor = Color.White, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, BackgroundColor = Color.FromHex("14986D") };

        public UserNotificationSettingsPage()
        {
            Title = "Settings";

            try
            {

                _client = new OMGITServiceClient(binding, EndPoint);

                _client.AddLogMessageMobileAsync("In User Notification Settings Page", "Setting Title", 0, GlobalData.loginData);

                ReturnButton.Clicked += ReturnButton_Clicked;
                

                PhoneCarrierPicker.Title = "Select a carrier:";

                _client.GetUserOptInSettingsCompleted += ClientUserOptInSettingsCompleted;
                _client.GetUserOptInSettingsAsync(GlobalData.loginData.UserName, GlobalData.loginData);

                while (UserSettings.UserName == string.Empty)
                {
                    Task.Delay(100).Wait();
                }


                _client.GetCarrierTextMappingsCompleted += ClientCarrierTextMappingsCompleted;
                _client.GetCarrierTextMappingsAsync(GlobalData.loginData);

                while (CarrierTextMappings.Count < 1)
                {
                    Task.Delay(100).Wait();
                }


                foreach (KeyValuePair<int, OMGITWebServices.OMGNotifyCarrierTextMapping> entry in CarrierTextMappings)
                {
                    PhoneCarrierPicker.Items.Add(entry.Value.CarrierDisplayName.ToString());    
                }

                _client.GetValidLocationsCompleted += ClientValidLocationsCompleted;
                _client.GetValidLocationsAsync(GlobalData.loginData);

                while (ValidLocations.Count < 1)
                {
                    Task.Delay(100).Wait();
                }

                _client.GetUserClinicsForNotificationsCompleted += ClientGetUserClinicsForNotificationsCompleted;
                _client.GetUserClinicsForNotificationsAsync(GlobalData.loginData.UserName, GlobalData.loginData);

                while (UClinics == null) 
                {
                    Task.Delay(100).Wait();
                }

                foreach (KeyValuePair<int, string> entry in ValidLocations)
                {
                    StackLayout ClinicLayout = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill };
                    if (UClinics.Contains(entry.Key))
                    {
                        ClinicLayout.Children.Add(new Switch { HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, IsToggled = true });
                    }
                    else
                    {
                        ClinicLayout.Children.Add(new Switch { HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center});

                    }
                    ClinicLayout.Children.Add(new Label { Text = entry.Value, HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, TextColor = Color.White });
                    ClinicLayout.Children.Add(new Label { Text = entry.Key.ToString(), HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, IsVisible = false });
                    AllClinicsLayout.Children.Add(ClinicLayout);
                }

                int index = 0;
                int CarrierID = UserSettings.CarrierID;
                foreach (KeyValuePair<int, OMGITWebServices.OMGNotifyCarrierTextMapping> entry in CarrierTextMappings)
                {
                    if (entry.Key == CarrierID)
                    {
                        break;
                    }

                    index += 1;
                }
                PhoneCarrierPicker.SelectedIndex = index;

                if (UserSettings.TextMessagingOK == 1)
                {
                    AllowTextMessageSwitch.IsToggled = true;
                }

                if (UserSettings.EmailOK == 1)
                {
                    AllowEmailSwitch.IsToggled = true;
                }

                CellPhoneEntry.Text = UserSettings.CellPhoneNumber;
                EmailEntry.Text = UserSettings.EmailAddress;


                Submit.Clicked += Submit_Clicked;
                Logout.Clicked += Logout_Clicked;

                
                NotificationSettingsLayout.Children.Add(Logout);
                

                PhoneCarrierLayout.Children.Add(PhoneCarrierLabel);
                PhoneCarrierLayout.Children.Add(PhoneCarrierPicker);
                NotificationSettingsLayout.Children.Add(PhoneCarrierLayout);

                AllowTextMessageLayout.Children.Add(AllowTextMessageLabel);
                AllowTextMessageLayout.Children.Add(AllowTextMessageSwitch);
                NotificationSettingsLayout.Children.Add(AllowTextMessageLayout);

                CellPhoneLayout.Children.Add(CellPhoneLabel);
                CellPhoneLayout.Children.Add(CellPhoneEntry);
                NotificationSettingsLayout.Children.Add(CellPhoneLayout);

                AllowEmailLayout.Children.Add(AllowEmailLabel);
                AllowEmailLayout.Children.Add(AllowEmailSwitch);
                NotificationSettingsLayout.Children.Add(AllowEmailLayout);

                EmailLayout.Children.Add(EmailLabel);
                EmailLayout.Children.Add(EmailEntry);
                NotificationSettingsLayout.Children.Add(EmailLayout);

                NotificationSettingsLayout.Children.Add(SelectClinicLabel);
                NotificationSettingsLayout.Children.Add(AllClinicsLayout);

                NotificationSettingsLayout.Children.Add(Submit);

                NotificationSettingsView.Content = NotificationSettingsLayout;

                Content = NotificationSettingsView;
                Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0);
            }
            catch (Exception ex)
            {
                _client.AddLogMessageMobileAsync("In UserNotificationSettings Page. Error: ", ex.Message.ToString(), 0, GlobalData.loginData);


            }

        }


        private void ClientCarrierTextMappingsCompleted(object sender, GetCarrierTextMappingsCompletedEventArgs e)
        {
            try
            {
                CarrierTextMappings = (Dictionary<int, OMGITWebServices.OMGNotifyCarrierTextMapping>)e.Result;
            }
            catch (Exception ex)
            {
                _client.AddLogMessageMobileAsync("In UserNotificationSettings Page ClientCarrierTextMappingsCompleted. Error: ", ex.Message.ToString(), 0, GlobalData.loginData);
                _client.AddLogMessageMobileAsync("In UserNotificationSettings Page ClientCarrierTextMappingsCompleted. Inner Exception Error: ", ex.InnerException.ToString(), 0, GlobalData.loginData);

            }
        }

        private void ClientValidLocationsCompleted(object sender, GetValidLocationsCompletedEventArgs e)
        {
            try
            {
                ValidLocations = (Dictionary<int, string>)e.Result;
            }
            catch (Exception ex)
            {
                _client.AddLogMessageMobileAsync("In UserNotificationSettings Page ClientValidLocationsCompleted. Error: ", ex.Message.ToString(), 0, GlobalData.loginData);
                _client.AddLogMessageMobileAsync("In UserNotificationSettings Page ClientValidLocationsCompleted. Inner Exception Error: ", ex.InnerException.ToString(), 0, GlobalData.loginData);

            }
        }

        private void ClientUserOptInSettingsCompleted(object sender, GetUserOptInSettingsCompletedEventArgs e)
        {
            try
            {
                UserSettings = (OMGITWebServices.OMGNotifyUserOptInSettings)e.Result;
            }
            catch (Exception ex)
            {
                _client.AddLogMessageMobileAsync("In UserNotificationSettings Page ClientUserOptInSettingsCompleted. Error: ", ex.Message.ToString(), 0, GlobalData.loginData);
                _client.AddLogMessageMobileAsync("In UserNotificationSettings Page ClientUserOptInSettingsCompleted. Inner Exception Error: ", ex.InnerException.ToString(), 0, GlobalData.loginData);

            }
        }

        private void Submit_Clicked(object sender, EventArgs e)
        {
            Submit.IsEnabled = false;
            int CarrierID = 0;

            OMGITWebServices.OMGNotifyUserOptInSettings settings = new OMGITWebServices.OMGNotifyUserOptInSettings();

            

            string selCarrier = (string)PhoneCarrierPicker.Items[PhoneCarrierPicker.SelectedIndex];
            foreach (KeyValuePair<int, OMGITWebServices.OMGNotifyCarrierTextMapping> entry in CarrierTextMappings)
            {
                if (entry.Value.CarrierDisplayName == selCarrier)
                    CarrierID = entry.Key;

            }

            settings.CarrierID = CarrierID;
            try
            {
                settings.CellPhoneNumber = CellPhoneEntry.Text.Trim();
                settings.EmailAddress = EmailEntry.Text.Trim();
                if (AllowEmailSwitch.IsToggled)
                {
                    settings.EmailOK = 1;
                }
                if (AllowTextMessageSwitch.IsToggled)
                {
                    settings.TextMessagingOK = 1;
                }
                settings.UserName = GlobalData.loginData.UserName;
            }
            catch (Exception ex) {; }

            if (settings.CellPhoneNumber == string.Empty && settings.TextMessagingOK == 1)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    ErrorMessage.Text = "Information was not updated. You must enter a phone number if you select to receive text message notifications.";
                    ErrorLayout.Children.Add(ErrorMessage);
                    ErrorLayout.Children.Add(ReturnButton);
                    this.Content = ErrorLayout;
                });
                return;
            }

            if (settings.EmailAddress == string.Empty && settings.EmailOK == 1)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    ErrorMessage.Text = "Information was not updated. You must enter an email address if you select to receive email notifications.";
                    ErrorLayout.Children.Add(ErrorMessage);
                    ErrorLayout.Children.Add(ReturnButton);
                    this.Content = ErrorLayout;
                });
                return;
            }

            List<int> UserClinicIDs = new List<int>();

            foreach (StackLayout s in AllClinicsLayout.Children)
            {
                var curClinicName = s.Children[1] as Label;
                var curID = s.Children[2] as Label;
                var curSwitch = s.Children[0] as Switch;
                if (curSwitch.IsToggled)
                {
                    try
                    {
                        UserClinicIDs.Add(Int32.Parse(curID.Text));
                    }
                    catch (Exception ex) {; }
                }
            }

            if (UserClinicIDs.Count == 0)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    ErrorMessage.Text = "Information was not updated. You must select at least one clinic to receive notifications for.";
                    ErrorLayout.Children.Add(ErrorMessage);
                    ErrorLayout.Children.Add(ReturnButton);
                    this.Content = ErrorLayout;
                });
                return;
            }

            _client.DeleteAllUserClinicForNotificationsCompleted += ClientDeleteAllUserClinicForNotificationsCompleted;
            _client.DeleteAllUserClinicForNotificationsAsync(GlobalData.loginData);

            while (!doneDelete)
            {
                continue;
            }

            

            _client.AddUpdateUserOptInSettingsCompleted += ClientAddUpdateUserOptInSettingsCompleted;
            _client.AddUpdateUserOptInSettingsAsync(settings, GlobalData.loginData);

            while (addUpdateComplete == -1)
            {
                continue;
            }

            _client.AddUserClinicForNotificationsCompleted += ClientAddUserClinicForNotificationsCompleted;
            _client.AddUserClinicForNotificationsAsync(UserClinicIDs.ToArray(), GlobalData.loginData);

            while (addClinicComplete == -1)
            {
                continue;
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                Navigation.PushAsync(new TestSendPage());
            });

        }

        private void ClientAddUpdateUserOptInSettingsCompleted(object sender, AddUpdateUserOptInSettingsCompletedEventArgs e)
        {
            try
            {
                addUpdateComplete = e.Result;
            }
            catch (Exception ex)
            {
                _client.AddLogMessageMobileAsync("In UserNotificationSettings Page ClientAddUpdateUserOptInSettingsCompleted. Error: ", ex.Message.ToString(), 0, GlobalData.loginData);
                _client.AddLogMessageMobileAsync("In UserNotificationSettings Page ClientAddUpdateUserOptInSettingsCompleted. Inner Exception Error: ", ex.InnerException.ToString(), 0, GlobalData.loginData);

            }
        }

        private void ClientDeleteAllUserClinicForNotificationsCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            try
            {
                doneDelete = true;
            }
            catch (Exception ex)
            {
                _client.AddLogMessageMobileAsync("In UserNotificationSettings Page ClientDeleteAllUserClinicForNotificationsCompleted. Error: ", ex.Message.ToString(), 0, GlobalData.loginData);
                _client.AddLogMessageMobileAsync("In UserNotificationSettings Page ClientDeleteAllUserClinicForNotificationsCompleted. Inner Exception Error: ", ex.InnerException.ToString(), 0, GlobalData.loginData);
            }
        }

        private void ClientGetUserClinicsForNotificationsCompleted(object sender, GetUserClinicsForNotificationsCompletedEventArgs e)
        {
            try
            {
                UClinics = e.Result;
            }
            catch (Exception ex)
            {
                _client.AddLogMessageMobileAsync("In UserNotificationSettings Page ClientGetUserClinicsForNotificationsCompleted. Error: ", ex.Message.ToString(), 0, GlobalData.loginData);
                _client.AddLogMessageMobileAsync("In UserNotificationSettings Page ClientGetUserClinicsForNotificationsCompleted. Inner Exception Error: ", ex.InnerException.ToString(), 0, GlobalData.loginData);
            }
        }

        private void ClientAddUserClinicForNotificationsCompleted(object sender, AddUserClinicForNotificationsCompletedEventArgs e)
        {
            try
            {
                addClinicComplete = e.Result;
            }
            catch (Exception ex)
            {
                _client.AddLogMessageMobileAsync("In UserNotificationSettings Page ClientAddUserClinicForNotificationsCompleted. Error: ", ex.Message.ToString(), 0, GlobalData.loginData);
                _client.AddLogMessageMobileAsync("In UserNotificationSettings Page ClientAddUserClinicForNotificationsCompleted. Inner Exception Error: ", ex.InnerException.ToString(), 0, GlobalData.loginData);
            }
        }

        private void Logout_Clicked(object sender, EventArgs e)
        {
            Helpers.Settings.UserName = string.Empty;
            Helpers.Settings.AuthToken = string.Empty;
            Helpers.Settings.Salt = string.Empty;

            Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
        }

        private void ReturnButton_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                this.Content = NotificationSettingsView;
            });
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
