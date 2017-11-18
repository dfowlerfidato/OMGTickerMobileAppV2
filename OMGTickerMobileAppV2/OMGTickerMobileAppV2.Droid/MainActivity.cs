using System;

using Android.App;
using Android.Content.PM;
using Android.OS;
using Gcm.Client;
using OMGTickerMobileApp.Droid;
using Android.Content;
using System.ServiceModel;

namespace OMGTickerMobileAppV2.Droid
{
    [Activity(Label = "OMGTickerMobileAppV2", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            OMGITServiceClient _client;
            EndpointAddress EndPoint = new EndpointAddress("https://webapps.oregonmed.net/OMGITWebservices/OMGMobileServices.svc");
            BasicHttpBinding binding = CreateBasicHttp();
            _client = new OMGITServiceClient(binding, EndPoint);
            _client.AddLogMessageTickerAppAsync("Reached OnCreate", string.Empty);
            base.OnCreate(bundle);
            /*
             *                 //To Do - acknowledge the message with a web service call to 
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
*/


            global::Xamarin.Forms.Forms.Init(this, bundle);

            string confirmToken = this.Intent.GetStringExtra("confirmToken");
            string alertMessage = this.Intent.GetStringExtra("alertMessage");
            if (confirmToken != null && alertMessage != null)
            {
                LoadApplication(new App(alertMessage));
                _client.AcknowledgeNotificationAsync(confirmToken, GlobalData.loginData);
            }
            else
            {
                LoadApplication(new App());
            }

            try
            {
                // Check to ensure everything's setup right
                GcmClient.CheckDevice(this);
                GcmClient.CheckManifest(this);

                // Register for push notifications
                System.Diagnostics.Debug.WriteLine("Registering...");
                GcmClient.Register(this, PushHandlerBroadcastReceiver.SENDER_IDS);
            }
            catch (Java.Net.MalformedURLException)
            {
                CreateAndShowDialog("There was an error creating the Mobile Service. Verify the URL", "Error");
            }
            catch (Exception e)
            {
                CreateAndShowDialog(e.Message, "Error");
            }


        }

        private void CreateAndShowDialog(String message, String title)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);

            builder.SetMessage(message);
            builder.SetTitle(title);
            builder.Create().Show();
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

