using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace OMGTickerMobileAppV2
{
    public class AdminMainPage : TabbedPage
    {
        public AdminMainPage()
        {
            NavigationPage.SetHasNavigationBar(this,false);
            Children.Add(new TickerDisplayPage());
            Children.Add(new LocationNotificationSendPage());
            Children.Add(new MessageHistoryPage());
            Children.Add(new AdminPage());
        }

        public AdminMainPage(string alertMessage)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            Children.Add(new TickerDisplayPage(alertMessage));
            Children.Add(new LocationNotificationSendPage());
            Children.Add(new MessageHistoryPage());
            Children.Add(new AdminPage());

        }
    }
}
