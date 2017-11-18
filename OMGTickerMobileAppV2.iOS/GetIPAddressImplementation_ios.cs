using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OMGTickerMobileAppV2.iOS;
using System.Net.NetworkInformation;
using System.Net.Sockets;

[assembly: Xamarin.Forms.Dependency(typeof(GetIPAddressImplementation_ios))]
namespace OMGTickerMobileAppV2.iOS
{
    public class GetIPAddressImplementation_ios : IIPAddressManager
    {
        public GetIPAddressImplementation_ios() { }

        public string GetIPAddress()
        {
            String ipAddress = "";

            foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                    netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    foreach (var addrInfo in netInterface.GetIPProperties().UnicastAddresses)
                    {
                        if (addrInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            ipAddress = addrInfo.Address.ToString();

                        }
                    }
                }
            }

            return ipAddress;
        }

    }

}


