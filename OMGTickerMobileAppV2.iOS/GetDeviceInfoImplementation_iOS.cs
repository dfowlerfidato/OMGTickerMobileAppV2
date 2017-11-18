using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OMGTickerMobileAppV2.iOS;

[assembly: Xamarin.Forms.Dependency(typeof(GetDeviceInfoImplementation_iOS))]
namespace OMGTickerMobileAppV2.iOS
{
    public class GetDeviceInfoImplementation_iOS : IGetDeviceInfo
    {
        public GetDeviceInfoImplementation_iOS() { }

        public string GetDeviceInfo()
        {
            string DevInfo = UIKit.UIDevice.CurrentDevice.IdentifierForVendor.AsString();
            return DevInfo;
        }

    }
}