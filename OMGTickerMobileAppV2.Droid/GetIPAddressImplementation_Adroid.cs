using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using OMGTickerMobileAppV2.Droid;
using System.Net;

[assembly: Xamarin.Forms.Dependency(typeof(GetIPAddressImplementation_Android))]

namespace OMGTickerMobileAppV2.Droid
{

    public class GetIPAddressImplementation_Android : IIPAddressManager
    {
        public GetIPAddressImplementation_Android() { }

        public string GetIPAddress()
        {
            string retval = string.Empty;
            try
            {

                System.Net.IPAddress[] adresses = Dns.GetHostAddresses(Dns.GetHostName());

                if (adresses != null && adresses[0] != null)
                {
                    retval =  adresses[0].ToString();
                }

            }
            catch (Exception) {; }
            return retval;
        }

    }
}
