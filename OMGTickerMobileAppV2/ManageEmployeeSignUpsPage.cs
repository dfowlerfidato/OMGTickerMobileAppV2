using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace OMGTickerMobileAppV2
{
    public class ManageEmployeeSignUpsPage : ContentPage
    {

        private OMGITServiceClient _client;
        public static readonly EndpointAddress EndPoint = new EndpointAddress("https://webapps.oregonmed.net/OMGITWebservices/OMGMobileServices.svc");
        BasicHttpBinding binding = CreateBasicHttp();

        Label RemoveLabel = new Label { VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.Fill, BackgroundColor = Color.White, TextColor = Color.Black, Text = "Remove", FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)), FontAttributes = FontAttributes.Bold };
        Label HistoryLabel = new Label { VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.Fill, BackgroundColor = Color.White, TextColor = Color.Black, Text = "History", FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)), FontAttributes = FontAttributes.Bold };
        Label EmployeeLabel = new Label { VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.Fill, BackgroundColor = Color.White, TextColor = Color.Black, Text = "Employee", FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)), FontAttributes = FontAttributes.Bold };
        Label ClinicNameLabel = new Label { VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.Fill, BackgroundColor = Color.White, TextColor = Color.Black, Text = "Clinic Name", FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)), FontAttributes = FontAttributes.Bold };

        StackLayout HeaderLayout = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Center };
        StackLayout FooterLayout = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Center };
        StackLayout ManageLayout = new StackLayout { BackgroundColor = Color.Black };

        System.Collections.Generic.Dictionary<int, OMGITWebServices.OMGUserSignupReportRow> UserLocations = new Dictionary<int, OMGITWebServices.OMGUserSignupReportRow>();

        List<System.Collections.Generic.Dictionary<int, OMGITWebServices.OMGUserSignupReportRow>> UserLocationsList = new List<Dictionary<int, OMGITWebServices.OMGUserSignupReportRow>>();

        int NumPages = 1;

        int PrevPage = 1;

        int PageSize = 10;

        int CurrentPage = 1;


        ScrollView ManageScrollView = new ScrollView { Orientation = ScrollOrientation.Vertical, BackgroundColor = Color.Black};

        Grid ManageGrid = new Grid();

        Button Logout = new Button { Text = "Logout", TextColor = Color.White, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center, BackgroundColor = Color.FromHex("14986D") };

        public ManageEmployeeSignUpsPage()
        {
            _client = new OMGITServiceClient(binding, EndPoint);

            _client.AddLogMessageMobileAsync("In Manage Employee Sign UPs Page", "", 0, GlobalData.loginData);

                //HeaderLayout.Children.Add(RemoveLabel);
            //HeaderLayout.Children.Add(HistoryLabel);
            //HeaderLayout.Children.Add(ClinicNameLabel);
            //HeaderLayout.Children.Add(EmployeeLabel);
            ManageGrid.RowDefinitions.Add(new RowDefinition { Height = 25});
            int i = 1;
            while (i <= PageSize + 1)
            {
                ManageGrid.RowDefinitions.Add(new RowDefinition { Height = 35 });
                i += 1;
            }
            ManageGrid.RowDefinitions.Add(new RowDefinition { Height = 35 });


            ManageGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            ManageGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            ManageGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            ManageGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            ManageGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            ManageGrid.Children.Add(RemoveLabel, 0, 0);
            ManageGrid.Children.Add(HistoryLabel, 1, 0);
            ManageGrid.Children.Add(ClinicNameLabel, 2, 4, 0, 1);
            ManageGrid.Children.Add(EmployeeLabel, 4, 0);

           // ManageLayout.Children.Add(HeaderLayout);

            _client.Report_GetUserSignUpsWithTableIDCompleted += ClientReport_GetUserSignUpsWithTableIDCompleted;
            _client.Report_GetUserSignUpsWithTableIDAsync(GlobalData.loginData);

            while (UserLocations.Count < 1)
            {
                continue;
            }

            

            NumPages += UserLocations.Count / PageSize;

            i = 1;

            int remainder = UserLocations.Count % PageSize;

            while (i < NumPages)
            {
                System.Collections.Generic.Dictionary<int,OMGITWebServices.OMGUserSignupReportRow> temp = UserLocations.Skip((i - 1) * PageSize).Take(PageSize).ToDictionary(x => x.Key, x => x.Value);
                UserLocationsList.Add(temp);
                i += 1;
            }

            System.Collections.Generic.Dictionary<int, OMGITWebServices.OMGUserSignupReportRow> lastPage = UserLocations.Skip((i - 1) * PageSize).Take(remainder).ToDictionary(x => x.Key, x => x.Value);
            UserLocationsList.Add(lastPage);




            i = 1;

            foreach (KeyValuePair<int, OMGITWebServices.OMGUserSignupReportRow> kvp in UserLocationsList[0])
            {
                Dictionary<int, string> DeleteArg = new Dictionary<int, string>();
                DeleteArg.Add(kvp.Key, kvp.Value.EmployeeName);
                //var row = new StackLayout { Orintation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Center };
                var EmpName = new Label { HorizontalTextAlignment = TextAlignment.Start, FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Button)), Text = kvp.Value.EmployeeName, TextColor = Color.White, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Fill };
                var ClinicName = new Label { HorizontalTextAlignment = TextAlignment.Start, FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Button)), Text = kvp.Value.ClinicName, TextColor = Color.White, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Fill };
                var DeleteButton = new Button { FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Button)), BackgroundColor = Color.FromHex("14986D"), VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center, TextColor = Color.White, Text = "Delete", CommandParameter = DeleteArg };
                var ViewButton = new Button { FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Button)), BackgroundColor = Color.FromHex("14986D"), VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center, TextColor = Color.White, Text = "View", CommandParameter = kvp.Value.EmployeeName };

                DeleteButton.Clicked += DeleteButton_Clicked;
                ViewButton.Clicked += ViewButton_Clicked;

                ManageGrid.Children.Add(DeleteButton, 0, i);
                ManageGrid.Children.Add(ViewButton, 1, i);
                Grid.SetColumnSpan(ClinicName, 2);
                ManageGrid.Children.Add(ClinicName, 2, 4, i, i + 1);
                ManageGrid.Children.Add(EmpName, 4, i);


                i += 1;

                //row.Children.Add(DeleteButton);
                //row.Children.Add(ViewButton);
                //row.Children.Add(ClinicName);
                //row.Children.Add(tableID);
                //row.Children.Add(EmpName);
                //ManageLayout.Children.Add(row);
            }

            //ManageLayout.Children.Add(FooterLayout);

            //Grid.SetColumnSpan(FooterLayout, 4);

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


            //FooterLayout.Children.Add(FirstPageButton);
            //FooterLayout.Children.Add(PrevPageButton);
            //FooterLayout.Children.Add(CurPageButton);
            //FooterLayout.Children.Add(NextPageButton);
            //FooterLayout.Children.Add(LastPageButton);

            ManageGrid.Children.Add(FirstPageButton, 0, PageSize + 2);
            ManageGrid.Children.Add(PrevPageButton, 1, PageSize + 2);
            ManageGrid.Children.Add(CurPageButton, 2, PageSize + 2);
            ManageGrid.Children.Add(NextPageButton, 3, PageSize + 2);
            ManageGrid.Children.Add(LastPageButton, 4, PageSize + 2);


            //ManageGrid.Children.Add(FooterLayout, 0, 3, 102, 103);


            //ManageScrollView.Content = ManageLayout;

            Logout.Clicked += Logout_Clicked;

            ManageLayout.Children.Add(Logout);
            ManageLayout.Children.Add(ManageGrid);

            ManageScrollView.Content = ManageLayout;

            Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0);

            Content = ManageScrollView;

        }


        private void ClientReport_GetUserSignUpsWithTableIDCompleted(object sender, Report_GetUserSignUpsWithTableIDCompletedEventArgs e)
        {
            try
            {
                UserLocations = e.Result;
            }
            catch (Exception ex)
            {
                _client.AddLogMessageMobileAsync("In Manage Employee signups Page ClientReport_GetUserSignUpsWithTableIDCompleted. Error: ", ex.Message.ToString(), 0, GlobalData.loginData);
                _client.AddLogMessageMobileAsync("In Manage Employee signups Page ClientReport_GetUserSignUpsWithTableIDCompleted. Inner Exception Error: ", ex.InnerException.ToString(), 0, GlobalData.loginData);

            }
        }

        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                var b = sender as Button;
                bool answer = false;
                int ID = -1;
                string username = "";
                Dictionary<int, string> d = (Dictionary<int, string>)b.CommandParameter;
                foreach (KeyValuePair<int, string> p in d)
                {
                    answer = await DisplayAlert("Confirm", "Are you sure you want to remove " + p.Value +  " from this clinic?", "Yes", "No");
                    ID = p.Key;
                    username = p.Value;
                }

                if (answer)
                {
                    _client.RemoveUserFromClinicNotificationsCompleted += ClientRemoveUserFromClinicNotificationsCompleted;
                    _client.RemoveUserFromClinicNotificationsAsync(username, ID, GlobalData.loginData);
                }
            }
            catch (Exception ex)
            {
                _client.AddLogMessageMobileAsync("In Manage Employee signups Page DeleteButton_Clicked. Error: ", ex.Message.ToString(), 0, GlobalData.loginData);
                _client.AddLogMessageMobileAsync("In Manage Employee signups Page DeleteButton_Clicked. Inner Exception Error: ", ex.InnerException.ToString(), 0, GlobalData.loginData);

            }
        }

        private void ClientRemoveUserFromClinicNotificationsCompleted(object sender, RemoveUserFromClinicNotificationsCompletedEventArgs e)
        {
            try
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Navigation.PopAsync();
                });
            }
            catch (Exception ex)
            {
                _client.AddLogMessageMobileAsync("In Manage Employee signups Page ClientRemoveUserFromClinicNotificationsCompleted. Error: ", ex.Message.ToString(), 0, GlobalData.loginData);
                _client.AddLogMessageMobileAsync("In Manage Employee signups Page ClientRemoveUserFromClinicNotificationsCompleted. Inner Exception Error: ", ex.InnerException.ToString(), 0, GlobalData.loginData);
            }
        }

        private void ViewButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                var b = sender as Button;
                string name = (string)b.CommandParameter;
                Device.BeginInvokeOnMainThread(() =>
                {
                    Navigation.PopAsync();
                    Navigation.PushAsync(new UserNotificationHistoryPage(name));
                });
                
            }
            catch (Exception ex)
            {
                _client.AddLogMessageMobileAsync("In Manage Employee signups Page ViewButton_Clicked. Error: ", ex.Message.ToString(), 0, GlobalData.loginData);
                _client.AddLogMessageMobileAsync("In Manage Employee signups Page ViewButton_Clicked. Inner Exception Error: ", ex.InnerException.ToString(), 0, GlobalData.loginData);

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
                _client.AddLogMessageMobileAsync("In Manage Employee signups Page NextPageButton_Clicked. Error: ", ex.Message.ToString(), 0, GlobalData.loginData);
                _client.AddLogMessageMobileAsync("In Manage Employee signups Page NextPageButton_Clicked. Inner Exception Error: ", ex.InnerException.ToString(), 0, GlobalData.loginData);

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
                _client.AddLogMessageMobileAsync("In Manage Employee signups Page PrevPageButton_Clicked. Error: ", ex.Message.ToString(), 0, GlobalData.loginData);
                _client.AddLogMessageMobileAsync("In Manage Employee signups Page PrevPageButton_Clicked. Inner Exception Error: ", ex.InnerException.ToString(), 0, GlobalData.loginData);

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
                _client.AddLogMessageMobileAsync("In Manage Employee signups Page FirstPageButton_Clicked. Error: ", ex.Message.ToString(), 0, GlobalData.loginData);
                _client.AddLogMessageMobileAsync("In Manage Employee signups Page FirstPageButton_Clicked. Inner Exception Error: ", ex.InnerException.ToString(), 0, GlobalData.loginData);

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
                _client.AddLogMessageMobileAsync("In Manage Employee signups Page LastPageButton_Clicked. Error: ", ex.Message.ToString(), 0, GlobalData.loginData);
                _client.AddLogMessageMobileAsync("In Manage Employee signups Page LastPageButton_Clicked. Inner Exception Error: ", ex.InnerException.ToString(), 0, GlobalData.loginData);

            }
        }


        private void UpdatePage()
        {
            int row = 1;



            foreach (KeyValuePair<int, OMGITWebServices.OMGUserSignupReportRow> kvp in UserLocationsList[CurrentPage - 1])
            {
                Dictionary<int, string> DeleteArg = new Dictionary<int, string>();
                DeleteArg.Add(kvp.Key, kvp.Value.EmployeeName);

                var delButton = ManageGrid.Children[(row * 4)] as Button;
                var viewButton = ManageGrid.Children[(row * 4) + 1] as Button;
                var clinicLabel = ManageGrid.Children[(row * 4) + 2] as Label;
                var empLabel = ManageGrid.Children[(row * 4) + 3] as Label;

                delButton.CommandParameter = DeleteArg;
                viewButton.CommandParameter = kvp.Value.EmployeeName;
                clinicLabel.Text = kvp.Value.ClinicName;
                empLabel.Text = kvp.Value.EmployeeName;

                row += 1;
            }

            if (row < PageSize)
            {
                while (row <= PageSize)
                {
                    var delButton = ManageGrid.Children[(row * 4)] as Button;
                    var viewButton = ManageGrid.Children[(row * 4) + 1] as Button;
                    var clinicLabel = ManageGrid.Children[(row * 4) + 2] as Label;
                    var empLabel = ManageGrid.Children[(row * 4) + 3] as Label;

                    delButton.IsVisible = false;
                    viewButton.IsVisible = false;
                    clinicLabel.IsVisible = false;
                    empLabel.IsVisible = false;

                    row += 1;
                }
            }
            else if (PrevPage == NumPages && row == PageSize + 1)
            {
                int num = 1;
                while (num <= PageSize)
                {
                    var delButton = ManageGrid.Children[(num * 4)] as Button;
                    var viewButton = ManageGrid.Children[(num * 4) + 1] as Button;
                    var clinicLabel = ManageGrid.Children[(num * 4) + 2] as Label;
                    var empLabel = ManageGrid.Children[(num * 4) + 3] as Label;

                    delButton.IsVisible = true;
                    viewButton.IsVisible = true;
                    clinicLabel.IsVisible = true;
                    empLabel.IsVisible = true;

                    num += 1;
                }

            }

            //var layout = ManageGrid.Children[404] as StackLayout;

            var firstPageButton = ManageGrid.Children[(PageSize * 4) + 4] as Button;
            var prevPageButton = ManageGrid.Children[(PageSize * 4) + 5] as Button;
            var curPageButton = ManageGrid.Children[(PageSize * 4) + 6] as Button;
            var nextPageButton = ManageGrid.Children[(PageSize * 4) + 7] as Button;
            var lastPageButton = ManageGrid.Children[(PageSize * 4) + 8] as Button;


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
                ManageScrollView.Content = ManageGrid;
                this.Content = ManageScrollView;
            });


            //Navigation.PopAsync();
            //Navigation.PushAsync(new NavigationPage(new ManageEmployeeSignUpsPage(index)));
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
