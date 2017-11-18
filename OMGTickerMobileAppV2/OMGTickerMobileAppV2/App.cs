using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.ServiceModel;
using System.Text;

using Xamarin.Forms;

namespace OMGTickerMobileAppV2
{
    public class App : Application
    {
        OMGITWebServices.OMGReturnMessage orm = new OMGITWebServices.OMGReturnMessage();
        public App()
        {
            OMGITServiceClient _client;
            EndpointAddress EndPoint = new EndpointAddress("https://webapps.oregonmed.net/OMGITWebservices/OMGMobileServices.svc");
            BasicHttpBinding binding = CreateBasicHttp();
            _client = new OMGITServiceClient(binding, EndPoint);
            _client.AddLogMessageTickerAppAsync("Reached App", "Contains Last Log in: " + Application.Current.Properties.ContainsKey("LastLoggedInUser").ToString());
            byte[] token = null;
            byte[] salt = null;
            string username = string.Empty;
            string authToken = string.Empty;
            OMGITWebServices.OMGAuthTokenData atd = new OMGITWebServices.OMGAuthTokenData();

            if (Application.Current.Properties.ContainsKey("LastLoggedInUser"))
            {

                username = Application.Current.Properties["LastLoggedInUser"] as string;
                atd.UserName = username;
                _client.AddLogMessageTickerAppAsync("Last Logged In User:", username);
                //Check to make sure the last logged in user has a token stored on the device
                if (Application.Current.Properties.ContainsKey(username + "Token"))
                {
                    token = Application.Current.Properties[username + "Token"] as byte[];
                }

                //Check to make sure a salt exists for the last logged in user
                if (Application.Current.Properties.ContainsKey(username + "Salt"))
                {
                    salt = Application.Current.Properties[username + "Salt"] as byte[];
                }

                //If both the token and salt exist decrypt the token
                if (token != null && salt != null)
                {
                    authToken = Crypto.DecryptAes(token, username, salt);
                    atd.AuthToken = authToken;
                }
            }

            atd.MobilePlatform = Device.OS.ToString();
            atd.OMGAppID = Constants.AppName;
            atd.PhoneIdentifier = DependencyService.Get<IGetDeviceInfo>().GetDeviceInfo();
            _client.ValidateAuthenticationTokenAsync(atd);
            _client.ValidateAuthenticationTokenCompleted += completedValidation;

            while (orm.ErrorMessage == "None" || orm.ErrorMessage == null)
            {
                continue;
            }
            //If the token is a valid non-expired token open the TickerDisplayPage page if not open the Login page

            if (orm.Success)
            {
                if (username == "hfowler" || username == "dfowler" || username == "sliu" || username == "khellwege")
                {
                    MainPage = new NavigationPage(new AdminMainPage());
                }
                else
                {
                    MainPage = new NavigationPage(new TickerDisplayPage());
                }
            }
            else
            {
                // The root page of your application
                MainPage = new NavigationPage(new LoginPage());
            }
        }

        public App(string alertMessage)
        {
            OMGITServiceClient _client;
            EndpointAddress EndPoint = new EndpointAddress("https://webapps.oregonmed.net/OMGITWebservices/OMGMobileServices.svc");
            BasicHttpBinding binding = CreateBasicHttp();
            _client = new OMGITServiceClient(binding, EndPoint);
            _client.AddLogMessageTickerAppAsync("Reached App tickerPage overload", "Contains Last Log in: " + Application.Current.Properties.ContainsKey("LastLoggedInUser").ToString());
            byte[] token = null;
            byte[] salt = null;
            string username = string.Empty;
            string authToken = string.Empty;
            OMGITWebServices.OMGAuthTokenData atd = new OMGITWebServices.OMGAuthTokenData();

            if (Application.Current.Properties.ContainsKey("LastLoggedInUser"))
            {

                username = Application.Current.Properties["LastLoggedInUser"] as string;
                atd.UserName = username;
                _client.AddLogMessageTickerAppAsync("Last Logged In User:", username);
                //Check to make sure the last logged in user has a token stored on the device
                if (Application.Current.Properties.ContainsKey(username + "Token"))
                {
                    token = Application.Current.Properties[username + "Token"] as byte[];
                }

                //Check to make sure a salt exists for the last logged in user
                if (Application.Current.Properties.ContainsKey(username + "Salt"))
                {
                    salt = Application.Current.Properties[username + "Salt"] as byte[];
                }

                //If both the token and salt exist decrypt the token
                if (token != null && salt != null)
                {
                    authToken = Crypto.DecryptAes(token, username, salt);
                    atd.AuthToken = authToken;
                }
            }

            atd.MobilePlatform = Device.OS.ToString();
            atd.OMGAppID = Constants.AppName;
            atd.PhoneIdentifier = DependencyService.Get<IGetDeviceInfo>().GetDeviceInfo();
            _client.ValidateAuthenticationTokenAsync(atd);
            _client.ValidateAuthenticationTokenCompleted += completedValidation;

            while (orm.ErrorMessage == "None" || orm.ErrorMessage == null)
            {
                continue;
            }
            //If the token is a valid non-expired token open the TickerDisplayPage page if not open the Login page

            if (orm.Success)
            {
                if (username == "hfowler" || username == "dfowler" || username == "sliu" || username == "khellwege")
                {
                    MainPage = new NavigationPage(new AdminMainPage(alertMessage));
                }
                else
                {
                    MainPage = new NavigationPage(new TickerDisplayPage(alertMessage));
                }
            }
            else
            {
                // The root page of your application
                MainPage = new NavigationPage(new LoginPage(alertMessage));
            }
        }

        private void completedValidation(object sender, ValidateAuthenticationTokenCompletedEventArgs e)
        {
            orm = e.Result;
        }

        protected override void OnStart()
        {
            
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
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
