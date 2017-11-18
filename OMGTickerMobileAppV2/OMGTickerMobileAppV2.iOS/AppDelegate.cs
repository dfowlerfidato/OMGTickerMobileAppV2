using Foundation;
using System;
using UIKit;
using System.ServiceModel;

namespace OMGTickerMobileAppV2.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {


        string alert = string.Empty;
        string alertMessage = string.Empty;
        string confirmToken = string.Empty;


        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            // registers for push for iOS8
            var settings = UIUserNotificationSettings.GetSettingsForTypes(
                UIUserNotificationType.Alert
                | UIUserNotificationType.Badge
                | UIUserNotificationType.Sound,
                new NSSet());

            LoadApplication(new App());

            //Register device with APNS to get a device token
            UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            UIApplication.SharedApplication.RegisterForRemoteNotifications();

            return base.FinishedLaunching(app, options);
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            //This is used to register for push notifications through Azure
            string DeviceToken = deviceToken.ToString();
            DeviceToken = DeviceToken.Replace(" ", "");
            DeviceToken = DeviceToken.Replace("<", "");
            DeviceToken = DeviceToken.Replace(">", "");
            Xamarin.Forms.Application.Current.Properties["DeviceToken"] = DeviceToken;
        }

        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            NSDictionary aps = userInfo.ObjectForKey(new NSString("aps")) as NSDictionary;
            //NSString trackid = userInfo.ObjectForKey(new NSString("trackid")) as NSString;

            //string track = string.Empty;


            if (aps.ContainsKey(new NSString("alert")))
                alert = (aps[new NSString("alert")] as NSString).ToString();
            if (aps.ContainsKey(new NSString("alertMessage")))
                alertMessage = (aps[new NSString("alertMessage")] as NSString).ToString();
            if (aps.ContainsKey(new NSString("confirmToken")))
                confirmToken = (aps[new NSString("confirmToken")] as NSString).ToString();
            //if (trackid != null)
            //    track = trackid.ToString();
            //show alert
            if (!string.IsNullOrEmpty(alert))
            {
                UIAlertView avAlert = new UIAlertView("Notification", alert, null, "OK", null);
                avAlert.Show();
                avAlert.Clicked += changePage;
                OMGITServiceClient _client;
                EndpointAddress EndPoint = new EndpointAddress("https://webapps.oregonmed.net/OMGITWebservices/OMGMobileServices.svc");
                BasicHttpBinding binding = CreateBasicHttp();
                _client = new OMGITServiceClient(binding, EndPoint);
                _client.AcknowledgeNotificationAsync(confirmToken, GlobalData.loginData);
            }
        }

        private void changePage(Object sender, UIButtonEventArgs a)
        {
            if (alertMessage != "" && alertMessage != null)
            {
                //App.Current.MainPage = new AdminMainPage(new TickerDisplayPage(alertMessage));
                App.Current.MainPage = new AdminMainPage(alertMessage);


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
