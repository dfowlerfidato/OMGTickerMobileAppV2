using Gcm.Client;
using Android.App;
using Android.Content;
using Android.Util;
using System.Text;
using OMGTickerMobileAppV2.Droid;
using OMGTickerMobileAppV2;
using System.ServiceModel;
using System;

[assembly: Permission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "com.google.android.c2dm.permission.RECEIVE")]

//GET_ACCOUNTS is only needed for android versions 4.0.3 and below
[assembly: UsesPermission(Name = "android.permission.GET_ACCOUNTS")]
[assembly: UsesPermission(Name = "android.permission.INTERNET")]
[assembly: UsesPermission(Name = "android.permission.WAKE_LOCK")]

namespace OMGTickerMobileApp.Droid
{
    [Service]
    public class GcmService : GcmServiceBase
    {
        public static string RegistrationID { get; private set; }

        public GcmService()
            : base(PushHandlerBroadcastReceiver.SENDER_IDS) { }

        protected override void OnRegistered(Context context, string registrationId)
        {
            Log.Verbose("PushHandlerBroadcastReceiver", "GCM Registered: " + registrationId);

            //The RegistrationID is used to register for Push Notifications through Azure
            RegistrationID = registrationId;
            //Xamarin.Forms.Application.Current.Properties["DeviceToken"] = registrationId;
            OMGTickerMobileAppV2.Helpers.Settings.DeviceToken = registrationId;
        }

        protected override void OnMessage(Context context, Intent intent)
        {
            Log.Info("PushHandlerBroadcastReceiver", "GCM Message Received!");

            var msg = new StringBuilder();

            if (intent != null && intent.Extras != null)
            {
                foreach (var key in intent.Extras.KeySet())
                    msg.AppendLine(key + "=" + intent.Extras.Get(key).ToString());
            }


            //Store the message
            var prefs = GetSharedPreferences(context.PackageName, FileCreationMode.Private);
            var edit = prefs.Edit();
            edit.PutString("last_msg", msg.ToString());
            edit.Commit();

            string message = intent.Extras.GetString("message");
            string confirmToken = intent.Extras.GetString("confirmToken");
            string alertMessage = intent.Extras.GetString("alertMessage");

            if (!string.IsNullOrEmpty(message))
            {
                createNotification("New OMG Alert!", "Click To View", confirmToken, alertMessage);

                //         OMGITServiceClient _client;
                //                 _client = new OMGITServiceClient(binding, GlobalData.EndPoint);
                //    _client.TestSendNotificationAsync(testSendData);
                //            _client.AcknowledgeNotificationAsync(confirmToken,GlobalData.loginData);

                return;
            }

            string msg2 = intent.Extras.GetString("msg");
            if (!string.IsNullOrEmpty(msg2))
            {
                createNotification("New hub message!", msg2, confirmToken, alertMessage);
                return;
            }

            createNotification("Unknown message details", msg.ToString());

        }

        void createNotification(string title, string desc, string confirmToken = null, string alertMessage = null)
        {
            //Create notification

            //Create an intent to show ui
            Intent uiIntent = new Intent(this, typeof(MainActivity));

            if (confirmToken != null && alertMessage != null)
            {
                uiIntent.PutExtra("confirmToken", confirmToken.ToString());
                uiIntent.PutExtra("alertMessage", alertMessage.ToString());
            }

            Android.App.TaskStackBuilder stackBuilder = Android.App.TaskStackBuilder.Create(this);

            stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(MainActivity)));

            stackBuilder.AddNextIntent(uiIntent);

            const int pendingIntentId = 0;
            PendingIntent pendingIntent =
                stackBuilder.GetPendingIntent(pendingIntentId, PendingIntentFlags.OneShot);

            Notification.Builder builder = new Notification.Builder(this)
                .SetContentIntent(pendingIntent)
                .SetContentTitle(title)
                .SetContentText(desc)
                .SetSmallIcon(Android.Resource.Drawable.SymActionEmail);

            // Build the notification:
            Notification notification = builder.Build();
            notification.Flags = NotificationFlags.AutoCancel;

            // Get the notification manager:
            NotificationManager notificationManager =
                GetSystemService(Context.NotificationService) as NotificationManager;

            // Publish the notification:
            const int notificationId = 0;
            notificationManager.Notify(notificationId, notification);
        }

        protected override void OnUnRegistered(Context context, string registrationId)
        {
            Log.Error("PushHandlerBroadcastReceiver", "Unregistered RegisterationId : " + registrationId);
        }

        protected override void OnError(Context context, string errorId)
        {
            Log.Error("PushHandlerBroadcastReceiver", "GCM Error: " + errorId);
        }
    }

    [BroadcastReceiver(Permission = Gcm.Client.Constants.PERMISSION_GCM_INTENTS)]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_MESSAGE }, Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_REGISTRATION_CALLBACK }, Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_LIBRARY_RETRY }, Categories = new string[] { "@PACKAGE_NAME@" })]
    public class PushHandlerBroadcastReceiver : GcmBroadcastReceiverBase<GcmService>
    {

        //This comes from the Google API console Project Number under project settings
        public static string[] SENDER_IDS = new string[] { "860876882895" };
    }
}