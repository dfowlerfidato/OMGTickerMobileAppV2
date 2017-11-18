using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;


namespace OMGTickerMobileAppV2
{
    public partial class TickerDisplayPage : ContentPage
    {
        
        Label Notifications = new Label { Text = "You do not have any notifications at this time", TextColor = Color.White, FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label))};

        Frame notificationFrame = new Frame { Padding = 3, BackgroundColor = Color.FromHex("1a1a1a"), OutlineColor = Color.FromHex("14986D") };

        ScrollView tickerView = new ScrollView { BackgroundColor = Color.FromHex("14986D") };

        StackLayout tickerLayout = new StackLayout { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, BackgroundColor = Color.FromHex("14986D") };

        public TickerDisplayPage()
        {
            Title = "Messages";
            if (Application.Current.Properties.ContainsKey("LastLoggedInUser"))
            {
                Notifications.Text = "Hello " + Application.Current.Properties["LastLoggedInUser"] as string + ". There are currently no messages for you";
            }
            
            
            notificationFrame.Content = Notifications;
            tickerLayout.Children.Add(notificationFrame);
            tickerView.Content = tickerLayout;
            Content = tickerView;
            
            Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0);
        }

        public TickerDisplayPage(string Message)
        {
            Title = "Messages";
            Notifications.Text = Message;
            notificationFrame.Content = Notifications;
            tickerLayout.Children.Add(notificationFrame);
            tickerView.Content = tickerLayout;
            Content = tickerView;
        }

        public bool getNotifications()
        {
            return false;
        }
    }
}