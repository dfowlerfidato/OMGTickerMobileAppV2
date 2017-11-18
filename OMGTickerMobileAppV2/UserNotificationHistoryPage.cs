using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace OMGTickerMobileAppV2
{
    public class UserNotificationHistoryPage : ContentPage
    {
        private OMGITServiceClient _client;
        public static readonly EndpointAddress EndPoint = new EndpointAddress("https://webapps.oregonmed.net/OMGITWebservices/OMGMobileServices.svc");
        BasicHttpBinding binding = CreateBasicHttp();

        Dictionary<int, OMGITWebServices.OMGUserNotificationHistoryRow> UserHistory = new Dictionary<int, OMGITWebServices.OMGUserNotificationHistoryRow>();

        List<Dictionary<int, OMGITWebServices.OMGUserNotificationHistoryRow>> UserHistoryList = new List<Dictionary<int, OMGITWebServices.OMGUserNotificationHistoryRow>>();

        Label SenderLabel = new Label { VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.Fill, BackgroundColor = Color.White, TextColor = Color.Black, Text = "Sender", FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)), FontAttributes = FontAttributes.Bold };
        Label SentToLabel = new Label { VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.Fill, BackgroundColor = Color.White, TextColor = Color.Black, Text = "Sent To", FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)), FontAttributes = FontAttributes.Bold };
        Label EmailLabel = new Label { VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.Fill, BackgroundColor = Color.White, TextColor = Color.Black, Text = "Email Address", FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)), FontAttributes = FontAttributes.Bold };
        Label TimeLabel = new Label { VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.Fill, BackgroundColor = Color.White, TextColor = Color.Black, Text = "Send Time", FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)), FontAttributes = FontAttributes.Bold };

        int NumPages = 1;

        int PrevPage = 1;

        int PageSize = 10;

        int CurrentPage = 1;

        StackLayout HistoryLayout = new StackLayout { BackgroundColor = Color.Black };

        Grid HistoryGrid = new Grid();

        ScrollView HistoryScrollView = new ScrollView { Orientation = ScrollOrientation.Vertical, BackgroundColor = Color.Black };

        Button Logout = new Button { Text = "Logout", TextColor = Color.White, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center, BackgroundColor = Color.FromHex("14986D") };

        public UserNotificationHistoryPage(string username)
        {
            try
            {
                _client = new OMGITServiceClient(binding, EndPoint);
                _client.AddLogMessageMobileAsync("In User Notification History Page", "", 0, GlobalData.loginData);

                HistoryGrid.RowDefinitions.Add(new RowDefinition { Height = 25 });

                int i = 1;
                while (i <= PageSize + 1)
                {
                    HistoryGrid.RowDefinitions.Add(new RowDefinition { Height = 35 });
                    i += 1;
                }
                HistoryGrid.RowDefinitions.Add(new RowDefinition { Height = 35 });

                HistoryGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                HistoryGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                HistoryGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                HistoryGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                HistoryGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                HistoryGrid.Children.Add(SenderLabel, 0, 0);
                HistoryGrid.Children.Add(SentToLabel, 1, 0);
                HistoryGrid.Children.Add(EmailLabel, 2, 4, 0, 1);
                HistoryGrid.Children.Add(TimeLabel, 4, 0);


                _client.Report_GetUserNotificationHistoryCompleted += ClientReport_GetUserNotificationHistoryCompleted;
                _client.Report_GetUserNotificationHistoryAsync(username, GlobalData.loginData);

                while (UserHistory.Count < 1)
                {
                    continue;
                }

                NumPages += UserHistory.Count / PageSize;

                i = 1;

                int remainder = UserHistory.Count % PageSize;

                while (i < NumPages)
                {
                    System.Collections.Generic.Dictionary<int, OMGITWebServices.OMGUserNotificationHistoryRow> temp = UserHistory.Skip((i - 1) * PageSize).Take(PageSize).ToDictionary(x => x.Key, x => x.Value);
                    UserHistoryList.Add(temp);
                    i += 1;
                }

                System.Collections.Generic.Dictionary<int, OMGITWebServices.OMGUserNotificationHistoryRow> lastPage = UserHistory.Skip((i - 1) * PageSize).Take(remainder).ToDictionary(x => x.Key, x => x.Value);
                UserHistoryList.Add(lastPage);




                i = 1;

                foreach (KeyValuePair<int, OMGITWebServices.OMGUserNotificationHistoryRow> kvp in UserHistoryList[0])
                {
                    var SenderName = new Label { HorizontalTextAlignment = TextAlignment.Start, FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)), Text = kvp.Value.SenderUserName, TextColor = Color.White, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Fill };
                    var SentToName = new Label { HorizontalTextAlignment = TextAlignment.Start, FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)), Text = kvp.Value.SentToUserName, TextColor = Color.White, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Fill };
                    var EmailAddress = new Label { HorizontalTextAlignment = TextAlignment.Start, FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)), VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center, TextColor = Color.White, Text = kvp.Value.EmailAddress };
                    var SentTime = new Label { HorizontalTextAlignment = TextAlignment.Start, FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)), VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center, TextColor = Color.White, Text = kvp.Value.SendTime };


                    HistoryGrid.Children.Add(SenderName, 0, i);
                    HistoryGrid.Children.Add(SentToName, 1, i);
                    HistoryGrid.Children.Add(EmailAddress, 2, 4, i, i + 1);
                    HistoryGrid.Children.Add(SentTime, 4, i);


                    i += 1;
                }


                var FirstPageButton = new Button();
                var PrevPageButton = new Button();
                var CurPageButton = new Button();
                var NextPageButton = new Button();
                var LastPageButton = new Button();

                if (Device.OS == TargetPlatform.Android)
                {
                    FirstPageButton = new Button { FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Button)), BackgroundColor = Color.FromHex("14986D"), VerticalOptions = LayoutOptions.Fill, HorizontalOptions = LayoutOptions.Fill, TextColor = Color.White, Text = "<<", IsEnabled = false };
                    PrevPageButton = new Button { FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Button)), BackgroundColor = Color.FromHex("14986D"), VerticalOptions = LayoutOptions.Fill, HorizontalOptions = LayoutOptions.Fill, TextColor = Color.White, Text = "< Prev", IsEnabled = false };
                    CurPageButton = new Button { FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Button)), BackgroundColor = Color.FromHex("14986D"), VerticalOptions = LayoutOptions.Fill, HorizontalOptions = LayoutOptions.Fill, TextColor = Color.White, Text = CurrentPage.ToString() + "/" + NumPages.ToString() };
                    if (NumPages == 1)
                    {
                        NextPageButton = new Button { FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Button)), BackgroundColor = Color.FromHex("14986D"), VerticalOptions = LayoutOptions.Fill, HorizontalOptions = LayoutOptions.Fill, TextColor = Color.White, Text = "Next >", IsEnabled = false };
                        LastPageButton = new Button { FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Button)), BackgroundColor = Color.FromHex("14986D"), VerticalOptions = LayoutOptions.Fill, HorizontalOptions = LayoutOptions.Fill, TextColor = Color.White, Text = ">>", IsEnabled = false };
                    }
                    else
                    {
                        NextPageButton = new Button { FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Button)), BackgroundColor = Color.FromHex("14986D"), VerticalOptions = LayoutOptions.Fill, HorizontalOptions = LayoutOptions.Fill, TextColor = Color.White, Text = "Next >" };
                        LastPageButton = new Button { FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Button)), BackgroundColor = Color.FromHex("14986D"), VerticalOptions = LayoutOptions.Fill, HorizontalOptions = LayoutOptions.Fill, TextColor = Color.White, Text = ">>" };
                    }
                }
                else
                {
                    FirstPageButton = new Button { FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Button)), BackgroundColor = Color.White, VerticalOptions = LayoutOptions.Fill, HorizontalOptions = LayoutOptions.Fill, TextColor = Color.Black, Text = "<<", IsEnabled = false };
                    PrevPageButton = new Button { FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Button)), BackgroundColor = Color.White, VerticalOptions = LayoutOptions.Fill, HorizontalOptions = LayoutOptions.Fill, TextColor = Color.Black, Text = "< Prev", IsEnabled = false };
                    CurPageButton = new Button { FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Button)), BackgroundColor = Color.White, VerticalOptions = LayoutOptions.Fill, HorizontalOptions = LayoutOptions.Fill, TextColor = Color.Black, Text = CurrentPage.ToString() + "/" + NumPages.ToString() };
                    if (NumPages == 1)
                    {
                        NextPageButton = new Button { FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Button)), BackgroundColor = Color.White, VerticalOptions = LayoutOptions.Fill, HorizontalOptions = LayoutOptions.Fill, TextColor = Color.Black, Text = "Next >", IsEnabled = false };
                        LastPageButton = new Button { FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Button)), BackgroundColor = Color.White, VerticalOptions = LayoutOptions.Fill, HorizontalOptions = LayoutOptions.Fill, TextColor = Color.Black, Text = ">>", IsEnabled = false };
                    }
                    else
                    {
                        NextPageButton = new Button { FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Button)), BackgroundColor = Color.White, VerticalOptions = LayoutOptions.Fill, HorizontalOptions = LayoutOptions.Fill, TextColor = Color.Black, Text = "Next >" };
                        LastPageButton = new Button { FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Button)), BackgroundColor = Color.White, VerticalOptions = LayoutOptions.Fill, HorizontalOptions = LayoutOptions.Fill, TextColor = Color.Black, Text = ">>" };
                    }
                }

                FirstPageButton.Clicked += FirstPageButton_Clicked;
                PrevPageButton.Clicked += PrevPageButton_Clicked;
                NextPageButton.Clicked += NextPageButton_Clicked;
                LastPageButton.Clicked += LastPageButton_Clicked;



                HistoryGrid.Children.Add(FirstPageButton, 0, PageSize + 2);
                HistoryGrid.Children.Add(PrevPageButton, 1, PageSize + 2);
                HistoryGrid.Children.Add(CurPageButton, 2, PageSize + 2);
                HistoryGrid.Children.Add(NextPageButton, 3, PageSize + 2);
                HistoryGrid.Children.Add(LastPageButton, 4, PageSize + 2);

                Logout.Clicked += Logout_Clicked;

                HistoryLayout.Children.Add(Logout);
                HistoryLayout.Children.Add(HistoryGrid);

                HistoryScrollView.Content = HistoryLayout;

                Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0);

                Content = HistoryScrollView;
            }
            catch (Exception ex) { string msg = ex.Message.ToString(); }


        }

        private void ClientReport_GetUserNotificationHistoryCompleted(object sender, Report_GetUserNotificationHistoryCompletedEventArgs e)
        {
            try
            {
                UserHistory = e.Result;
            }
            catch (Exception ex)
            {
                _client.AddLogMessageMobileAsync("In UserNotificationHistory Page ClientReport_GetUserNotificationHistoryCompleted. Error: ", ex.Message.ToString(), 0, GlobalData.loginData);
                _client.AddLogMessageMobileAsync("In UserNotificationHistory Page ClientReport_GetUserNotificationHistoryCompleted. Inner Exception Error: ", ex.InnerException.ToString(), 0, GlobalData.loginData);

            }
        }


        private void NextPageButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                PrevPage = CurrentPage;
                CurrentPage += 1;

                UpdatePage();
            }
            catch (Exception ex)
            {
                _client.AddLogMessageMobileAsync("In UserNotificationHistory Page NextPageButton_Clicked. Error: ", ex.Message.ToString(), 0, GlobalData.loginData);
                _client.AddLogMessageMobileAsync("In UserNotificationHistory Page NextPageButton_Clicked. Inner Exception Error: ", ex.InnerException.ToString(), 0, GlobalData.loginData);

            }
        }

        private void PrevPageButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                PrevPage = CurrentPage;
                CurrentPage -= 1;

                UpdatePage();
            }
            catch (Exception ex)
            {
                _client.AddLogMessageMobileAsync("In UserNotificationHistory Page PrevPageButton_Clicked. Error: ", ex.Message.ToString(), 0, GlobalData.loginData);
                _client.AddLogMessageMobileAsync("In UserNotificationHistory Page PrevPageButton_Clicked. Inner Exception Error: ", ex.InnerException.ToString(), 0, GlobalData.loginData);

            }
        }

        private void FirstPageButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                PrevPage = CurrentPage;
                CurrentPage = 1;

                UpdatePage();
            }
            catch (Exception ex)
            {
                _client.AddLogMessageMobileAsync("In UserNotificationHistory Page FirstPageButton_Clicked. Error: ", ex.Message.ToString(), 0, GlobalData.loginData);
                _client.AddLogMessageMobileAsync("In UserNotificationHistory Page FirstPageButton_Clicked. Inner Exception Error: ", ex.InnerException.ToString(), 0, GlobalData.loginData);

            }
        }

        private void LastPageButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                PrevPage = CurrentPage;
                CurrentPage = NumPages;

                UpdatePage();
            }
            catch (Exception ex)
            {
                _client.AddLogMessageMobileAsync("In UserNotificationHistory Page LastPageButton_Clicked. Error: ", ex.Message.ToString(), 0, GlobalData.loginData);
                _client.AddLogMessageMobileAsync("In UserNotificationHistory Page LastPageButton_Clicked. Inner Exception Error: ", ex.InnerException.ToString(), 0, GlobalData.loginData);

            }
        }

        private void Logout_Clicked(object sender, EventArgs e)
        {
            Helpers.Settings.UserName = string.Empty;
            Helpers.Settings.AuthToken = string.Empty;
            Helpers.Settings.Salt = string.Empty;

            Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
        }
        private void UpdatePage()
        {
            int row = 1;



            foreach (KeyValuePair<int, OMGITWebServices.OMGUserNotificationHistoryRow> kvp in UserHistoryList[CurrentPage - 1])
            {

                var senderLabel = HistoryGrid.Children[(row * 4)] as Label;
                var sentToLabel = HistoryGrid.Children[(row * 4) + 1] as Label;
                var emailLabel = HistoryGrid.Children[(row * 4) + 2] as Label;
                var timeLabel = HistoryGrid.Children[(row * 4) + 3] as Label;

                senderLabel.Text = kvp.Value.SenderUserName;
                sentToLabel.Text = kvp.Value.SentToUserName;
                emailLabel.Text = kvp.Value.EmailAddress;
                timeLabel.Text = kvp.Value.SendTime;

                row += 1;
            }

            if (row < PageSize)
            {
                while (row <= PageSize)
                {
                    var senderLabel = HistoryGrid.Children[(row * 4)] as Label;
                    var sentToLabel = HistoryGrid.Children[(row * 4) + 1] as Label;
                    var emailLabel = HistoryGrid.Children[(row * 4) + 2] as Label;
                    var timeLabel = HistoryGrid.Children[(row * 4) + 3] as Label;

                    senderLabel.IsVisible = false;
                    sentToLabel.IsVisible = false;
                    emailLabel.IsVisible = false;
                    timeLabel.IsVisible = false;

                    row += 1;
                }
            }
            else if (PrevPage == NumPages && row == PageSize + 1)
            {
                int num = 1;
                while (num <= PageSize)
                {
                    var senderLabel = HistoryGrid.Children[(num * 4)] as Label;
                    var sentToLabel = HistoryGrid.Children[(num * 4) + 1] as Label;
                    var emailLabel = HistoryGrid.Children[(num * 4) + 2] as Label;
                    var timeLabel = HistoryGrid.Children[(num * 4) + 3] as Label;

                    senderLabel.IsVisible = true;
                    sentToLabel.IsVisible = true;
                    emailLabel.IsVisible = true;
                    timeLabel.IsVisible = true;

                    num += 1;
                }

            }

            var firstPageButton = HistoryGrid.Children[(PageSize * 4) + 4] as Button;
            var prevPageButton = HistoryGrid.Children[(PageSize * 4) + 5] as Button;
            var curPageButton = HistoryGrid.Children[(PageSize * 4) + 6] as Button;
            var nextPageButton = HistoryGrid.Children[(PageSize * 4) + 7] as Button;
            var lastPageButton = HistoryGrid.Children[(PageSize * 4) + 8] as Button;


            curPageButton.Text = CurrentPage.ToString() + "/" + NumPages.ToString();


            if (CurrentPage == NumPages)
            {
                nextPageButton.IsEnabled = false;
                lastPageButton.IsEnabled = false;
            }
            else
            {
                nextPageButton.IsEnabled = true;
                lastPageButton.IsEnabled = true;
            }

            if (CurrentPage == 1)
            {
                prevPageButton.IsEnabled = false;
                firstPageButton.IsEnabled = false;
            }
            else
            {
                prevPageButton.IsEnabled = true;
                firstPageButton.IsEnabled = true;
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                HistoryScrollView.Content = HistoryGrid;
                this.Content = HistoryScrollView;
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
