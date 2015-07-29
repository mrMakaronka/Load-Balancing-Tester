using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Web;
using WcfServiceLibrary;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Configuration;
using System.Net;

namespace WcfServiceHost
{
    class Program
    {
        static void Main()
        {
            int serverId;
            if (!int.TryParse(ConfigurationManager.AppSettings["ServerId"], out serverId))
            {
                throw new ConfigurationErrorsException("Parameter ServerId is not set");
            }

            StatelessService testServiceInstance = new StatelessService(serverId);
            using (ServiceHost host = new ServiceHost(testServiceInstance))
            {
                try
                {
                    host.Open();
                    Console.WriteLine("IPs: ");
                    foreach (IPAddress ip in HostUtils.GetLocalIPAddresses()) {
                        Console.WriteLine(ip.ToString());
                    }
                    Console.WriteLine();
                    Console.WriteLine("Press <ENTER> to terminate");
                    Console.ReadLine();
                    host.Close();
                }
                catch (CommunicationException cex)
                {
                    Console.WriteLine("An exception occurred: {0}", cex.Message);
                    host.Abort();
                }
            }
        }
    }
}
