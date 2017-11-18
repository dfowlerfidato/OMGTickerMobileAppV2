using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace OMGTickerMobileAppV2
{
    public class AdminMainPage : ContentPage
    //public class AdminMainPage : TabbedPage
    {
        Button Logout = new Button { Text = "Logout", TextColor = Color.White, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center, BackgroundColor = Color.FromHex("14986D") };

        Frame TickerDisplayFrame = new Frame { BackgroundColor = Color.Gray, HeightRequest = 150, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Fill };
        Button TickerDisplayButton = new Button { BackgroundColor = Color.FromHex("14986D"), HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill, Text = "View Messages", TextColor = Color.White};

        Frame UserNotificationSettingsFrame = new Frame { BackgroundColor = Color.FromHex("14986D"), HeightRequest = 150, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Fill };
        Button UserNotificationSettingsButton = new Button { BackgroundColor = Color.Gray, HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill, Text = "User Notification Settings", TextColor = Color.White };

        Frame LocationNotificationSendFrame = new Frame { BackgroundColor = Color.Gray, HeightRequest = 150, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Fill };
        Button LocationNotificationSendButton = new Button { BackgroundColor = Color.FromHex("14986D"), HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill, Text = "Send Notification To Location", TextColor = Color.White };

        Frame AdminFrame = new Frame { BackgroundColor = Color.Gray, HeightRequest = 150, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Fill };
        Button AdminButton = new Button { BackgroundColor = Color.FromHex("14986D"), HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill, Text = "Send Test Push Notification", TextColor = Color.White };

        Frame ManageUserSignUpsFrame = new Frame { BackgroundColor = Color.Gray, HeightRequest = 150, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Fill };
        Button ManageUserSignUpsButton = new Button { BackgroundColor = Color.FromHex("14986D"), HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill, Text = "Manage Employee Sign Ups", TextColor = Color.White };


        Image logo = new Image { Aspect = Aspect.AspectFit, Source = "omglogog.png", HorizontalOptions = LayoutOptions.Center };

        //StackLayout AdminPageLayout = new StackLayout { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill, Orientation = StackOrientation.Horizontal };

        StackLayout AdminPageLayout = new StackLayout { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill, Orientation = StackOrientation.Vertical, BackgroundColor = Color.Black };

        Grid AdminGrid = new Grid();
        

        public AdminMainPage()
        {
            NavigationPage.SetHasNavigationBar(this,false);
            //Children.Add(new TickerDisplayPage());
            //Children.Add(new UserNotificationSettingsPage());
            //Children.Add(new LocationNotificationSendPage());
            //Children.Add(new AdminPage());


            Logout.Clicked += Logout_Clicked;
            TickerDisplayButton.Clicked += TickerDisplayButton_Clicked;
            UserNotificationSettingsButton.Clicked += UserNotificationSettingsButton_Clicked;
            LocationNotificationSendButton.Clicked += LocationNotificationSendButton_Clicked;
            AdminButton.Clicked += AdminButton_Clicked;
            ManageUserSignUpsButton.Clicked += ManageUserSignUpsButton_Clicked;

            TickerDisplayFrame.Content = TickerDisplayButton;
            UserNotificationSettingsFrame.Content = UserNotificationSettingsButton;
            LocationNotificationSendFrame.Content = LocationNotificationSendButton;
            AdminFrame.Content = AdminButton;
            ManageUserSignUpsFrame.Content = ManageUserSignUpsButton;

            //AdminPageLayout.Children.Add(TickerDisplayFrame);
            //AdminPageLayout.Children.Add(UserNotificationSettingsFrame);
            //AdminPageLayout.Children.Add(LocationNotificationSendFrame);
            //AdminPageLayout.Children.Add(AdminFrame);


            //ScrollView AdminPageScrollView = new ScrollView { Orientation = ScrollOrientation.Horizontal, Content = AdminPageLayout };


            //Content = AdminPageScrollView;

            AdminGrid.RowDefinitions.Add(new RowDefinition { Height = 150});
            AdminGrid.RowDefinitions.Add(new RowDefinition { Height = 150 });
            AdminGrid.RowDefinitions.Add(new RowDefinition { Height = 150 });
            AdminGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            AdminGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            //AdminGrid.Children.Add(TickerDisplayFrame, 0, 0);
            AdminGrid.Children.Add(UserNotificationSettingsFrame, 0, 0);
            AdminGrid.Children.Add(LocationNotificationSendFrame, 1, 0);
            //AdminGrid.Children.Add(AdminFrame, 1, 1);
            AdminGrid.Children.Add(ManageUserSignUpsFrame, 0, 1);

            AdminPageLayout.Children.Add(Logout);
            AdminPageLayout.Children.Add(logo);
            AdminPageLayout.Children.Add(AdminGrid);

            ScrollView AdminPageScrollView = new ScrollView { Orientation = ScrollOrientation.Vertical, BackgroundColor = Color.Black, Content = AdminPageLayout };

            Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0);

            Content = AdminPageScrollView;

        }

        public AdminMainPage(string alertMessage)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            //Children.Add(new TickerDisplayPage(alertMessage));
            //Children.Add(new UserNotificationSettingsPage());
            //Children.Add(new LocationNotificationSendPage());
            //Children.Add(new AdminPage());

        }

        private void TickerDisplayButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new TickerDisplayPage());
        }

        private void UserNotificationSettingsButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new UserNotificationSettingsPage());
        }

        private void LocationNotificationSendButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new LocationNotificationSendPage());
        }

        private void AdminButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AdminPage());
        }

        private void ManageUserSignUpsButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ManageEmployeeSignUpsPage());
        }

        private void Logout_Clicked(object sender, EventArgs e)
        {
            Helpers.Settings.UserName = string.Empty;
            Helpers.Settings.AuthToken = string.Empty;
            Helpers.Settings.Salt = string.Empty;

            Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
        }

    }
}
