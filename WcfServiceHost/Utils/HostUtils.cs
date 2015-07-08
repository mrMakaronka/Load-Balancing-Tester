using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WcfServiceHost
{
    static class HostUtils
    {
        public static List<IPAddress> GetLocalIPAddresses()
        {
            List<IPAddress> ips = NetworkInterface.GetAllNetworkInterfaces()
            .Where(x => x.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            .Where(x => x.OperationalStatus == OperationalStatus.Up)
            .SelectMany(x => x.GetIPProperties().UnicastAddresses)
            .Where(x => x.Address.AddressFamily == AddressFamily.InterNetwork)
            .Select(x => x.Address)
            .ToList();
            return ips;
        }
    }
}
