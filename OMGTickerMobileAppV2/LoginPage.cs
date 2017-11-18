using System;

using Xamarin.Forms;
using System.ServiceModel;
using Plugin.Geolocator;
using System.Diagnostics;
using Plugin.Permissions.Abstractions;
using Plugin.Permissions;
using System.Threading.Tasks;
using System.Collections.Generic;
using Plugin.Settings;

namespace OMGTickerMobileAppV2
{
    public class LoginPage : ContentPage
    {
        string latitude = "";
        string longitude = "";
        string altitude = "";
        public string regID = "";

        private OMGITServiceClient _client;
        public static readonly EndpointAddress EndPoint = new EndpointAddress("https://webapps.oregonmed.net/OMGITWebservices/OMGMobileServices.svc");
        BasicHttpBinding binding = CreateBasicHttp();
        OMGITWebServices.OMGReturnMessage returnMessage = new OMGITWebServices.OMGReturnMessage();
        OMGITWebServices.OMGReturnMessage registerReturnMessage = new OMGITWebServices.OMGReturnMessage();

        Entry usernameEntry = new Entry {PlaceholderColor = Color.Black, Placeholder = "         USERNAME         ", HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, BackgroundColor = Color.White, TextColor = Color.Black };
        Entry passwordEntry = new Entry {PlaceholderColor = Color.Black, Placeholder = "         PASSWORD         ", IsPassword = true, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, BackgroundColor = Color.White, TextColor = Color.Black };

        Label keepLoggedInLabel = new Label { Text = "Keep me logged in", HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, TextColor = Color.White};
        Switch keepLoggedInSwitch = new Switch { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };

        StackLayout loginForm = new StackLayout { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill, BackgroundColor = Color.FromHex("1a1a1a") };

        Button submit = new Button { Text = "Log in", TextColor = Color.White, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, BackgroundColor = Color.FromHex("14986D") };

        Image logo = new Image { Aspect = Aspect.AspectFit, Source = "omglogog.png", HorizontalOptions = LayoutOptions.Center};

        Label alert = new Label { Text = "", IsVisible = false };

        private int incorrectPasswords = 0;

        string[] NotificationAdmins;

        public LoginPage()
        {
            
            NavigationPage.SetHasNavigationBar(this, false);

            submit.Clicked += Submit_Clicked;

            loginForm.Children.Add(logo);
            loginForm.Children.Add(usernameEntry);
            loginForm.Children.Add(passwordEntry);
            loginForm.Children.Add(keepLoggedInLabel);
            loginForm.Children.Add(keepLoggedInSwitch);
            loginForm.Children.Add(submit);
            ScrollView loginView = new ScrollView { Content = loginForm, BackgroundColor = Color.FromHex("1a1a1a") };
            Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0);
            Content = loginView;
        }

        public LoginPage(string alertMessage)
        {

            NavigationPage.SetHasNavigationBar(this, false);

            submit.Clicked += Submit_Clicked;

            loginForm.Children.Add(logo);
            loginForm.Children.Add(usernameEntry);
            loginForm.Children.Add(passwordEntry);
            loginForm.Children.Add(keepLoggedInLabel);
            loginForm.Children.Add(keepLoggedInSwitch);
            loginForm.Children.Add(submit);
            alert.Text = alertMessage;
            loginForm.Children.Add(alert);
            ScrollView loginView = new ScrollView { Content = loginForm, BackgroundColor = Color.FromHex("1a1a1a") };
            Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0);
            Content = loginView;
        }

        private async void Submit_Clicked(object sender, EventArgs e)
        {
            submit.IsEnabled = false;

            if (Application.Current.Properties.ContainsKey("DeviceToken"))
            {
                string DeviceToken = Application.Current.Properties["DeviceToken"] as string;
            }

            try
            {
                _client = new OMGITServiceClient(binding, EndPoint);
                _client.AddLogMessageMobileAsync("Reached Submit_Clicked in Login Page", "", 0, GlobalData.loginData);

//                _client.AddLogMessageTickerAppAsync("Reached Submit_Clicked in LoginPage", "");


                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);

                if (status == PermissionStatus.Granted)
                {
                    var locator = CrossGeolocator.Current;
                    locator.DesiredAccuracy = 50;

                    var position = await locator.GetPositionAsync(timeoutMilliseconds: 10000);
                    latitude = position.Latitude.ToString();
                    longitude = position.Longitude.ToString();
                    altitude = position.Altitude.ToString();

                    Debug.WriteLine("Position Status: {0}", position.Timestamp);
                    Debug.WriteLine("Position Latitude: {0}", position.Latitude);
                    Debug.WriteLine("Position Longitude: {0}", position.Longitude);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to get location, may need to increase timeout: " + ex);
            }

            OMGITWebServices.OMGLoginData loginData = new OMGITWebServices.OMGLoginData();

            string dontuseForLogin1 = "@oregonmed.net";
            string dontuseForLogin2 = "omgnet";
            if (usernameEntry.Text.Trim().ToLower().Contains(dontuseForLogin1) || usernameEntry.Text.Trim().ToLower().Contains(dontuseForLogin2))
            {
                await DisplayAlert("Only enter your username", "Don't use @oregonmed.net or OMGNET", "Close");
                submit.IsEnabled = true;
                return;
            }

            loginData.UserName = usernameEntry.Text.Trim().ToLower();
            loginData.Password = passwordEntry.Text;
            string alertMessage = alert.Text;
            loginData.OMGAppID = Constants.AppName + GlobalData.OMGAppVersion;
            loginData.Altitude = altitude;
            loginData.Latitude = latitude;
            loginData.Longitude = longitude;
            if (Device.OS == TargetPlatform.iOS)
            {
                loginData.MobilePlatform = "iOS";
            }
            else if (Device.OS == TargetPlatform.Android)
            {
                loginData.MobilePlatform = "Android";
            }
            else
            {
                loginData.MobilePlatform = "Unknown Platform";
            }
            loginData.PhoneIdentifier = DependencyService.Get<IGetDeviceInfo>().GetDeviceInfo();

            string ipaddress = DependencyService.Get<IIPAddressManager>().GetIPAddress();
            loginData.IPAddress = ipaddress;

            if (keepLoggedInSwitch.IsToggled)
            {
                loginData.CreateAuthToken = true;
            }

            GlobalData.SetLoginData(loginData);

            await Task.Run(() =>
            {
                _client = new OMGITServiceClient(binding, EndPoint);
                _client.LoginAsync(loginData);
                _client.LoginCompleted += ClientOnLoginCompleted;
            });


            try
            {
                while (returnMessage.ErrorMessage == null)
                {
                    continue;
                }

                await Task.Run(() =>
                {
                    _client = new OMGITServiceClient(binding, EndPoint);
                    _client.GetNotificationAdminsAsync(loginData);
                    _client.GetNotificationAdminsCompleted += ClientNotificationAdminsCompleted;
                });

                while (NotificationAdmins == null)
                {
                    continue;
                }

                if (returnMessage.Success)
                {
                    if (keepLoggedInSwitch.IsToggled)
                    {
                        var data = loginData.UserName;
                        var salt = Crypto.CreateSalt(16);
                        var bytes = Crypto.EncryptAes(returnMessage.AuthToken, data, salt);

                        Helpers.Settings.UserName = loginData.UserName;
                        Helpers.Settings.AuthToken = Convert.ToBase64String(bytes);
                        Helpers.Settings.Salt = Convert.ToBase64String(salt);

                        //Application.Current.Properties["LastLoggedInUser"] = loginData.UserName;
                        //Application.Current.Properties[loginData.UserName + "Token"] = bytes;
                        //Application.Current.Properties[loginData.UserName + "Salt"] = salt;
                        //await Application.Current.SavePropertiesAsync();
                    }
                    OMGITWebServices.OMGInstallationData installData = new OMGITWebServices.OMGInstallationData();
                    //if (Application.Current.Properties.ContainsKey("DeviceToken"))
                    if (Helpers.Settings.DeviceToken != string.Empty)
                    {
                        //installData.DeviceToken = Application.Current.Properties["DeviceToken"] as string;
                        installData.DeviceToken = Helpers.Settings.DeviceToken;
                    }

                    installData.Username = loginData.UserName;
                    installData.OperatingSystem = loginData.MobilePlatform;

                    await Task.Run(() =>
                    {
                        _client = new OMGITServiceClient(binding, EndPoint);
                        _client.RegisterForNotificationsAsync(installData, loginData);
                        _client.RegisterForNotificationsCompleted += ClientOnRegisterCompleted;
                        //_client.RegisterForNotificationsV2Async(installData, loginData);
                        //_client.RegisterForNotificationsV2Completed += ClientOnRegisterCompleted;
                    });

                    while (registerReturnMessage.ErrorMessage == null)
                    {
                        continue;
                    }

                    bool isAdmin = false;

                    foreach (string s in NotificationAdmins)
                    {
                        if (loginData.UserName == s)
                        {
                            isAdmin = true;
                            break;
                        }
                    }

                    if (isAdmin)
                    {
                        if (alertMessage != "")
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                Navigation.PushModalAsync(new NavigationPage(new AdminMainPage(alertMessage)));
                            });
                        }
                        else
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                Navigation.PushModalAsync(new NavigationPage(new AdminMainPage()));
                            });
                        }
                    }
                    else
                    {
                        if (alertMessage != "")
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                Navigation.PushModalAsync(new NavigationPage(new UserNotificationSettingsPage()));
                            });
                        }
                        else
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                Navigation.PushModalAsync(new NavigationPage(new UserNotificationSettingsPage()));
                            });
                        }
                    }
                }
                else
                {
                    if (incorrectPasswords == 0)
                    {
                        Label errorMessage = new Label { Text = returnMessage.ErrorMessage, TextColor = Color.Red, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, BackgroundColor = Color.White };
                        loginForm.Children.Add(errorMessage);
                    }
                    returnMessage.ErrorMessage = null;
                    incorrectPasswords += 1;
                    submit.IsEnabled = true;
                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

        }

        private void ClientOnLoginCompleted(object sender, LoginCompletedEventArgs e)
        {
            returnMessage = e.Result;
        }

        private void ClientOnRegisterCompleted(object sender, RegisterForNotificationsCompletedEventArgs e)
        {
            registerReturnMessage = e.Result;
        }

        private void ClientNotificationAdminsCompleted(object sender, GetNotificationAdminsCompletedEventArgs e)
        {
            NotificationAdmins = e.Result;
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
