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

using Plugin.DeviceInfo;
using OMGTickerMobileAppV2.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(GetDeviceInfoImplementation_Android))]
namespace OMGTickerMobileAppV2.Droid
{
    public class GetDeviceInfoImplementation_Android : IGetDeviceInfo
    {
        public GetDeviceInfoImplementation_Android() { }

        public string GetDeviceInfo()
        {
            string DevInfo = Build.Serial;
            return DevInfo;
        }

    }


}