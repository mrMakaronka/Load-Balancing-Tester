using System;
using System.Configuration;
using System.Globalization;
using System.Net;
using System.ServiceModel;
using System.Threading;
using WcfServiceHost.Utils;
using WcfServiceLibrary;

namespace WcfServiceHost
{
    class Program
    {
        static void Main()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");

            int serverId;
            if (!int.TryParse(ConfigurationManager.AppSettings["ServerId"], out serverId))
            {
                throw new ConfigurationErrorsException("Parameter ServerId is not set");
            }
            Console.WriteLine("Server ID: " + serverId);
            Console.WriteLine();

            StatelessService statelessTestServiceInstance = new StatelessService(serverId);
            using (ServiceHost statelessServiceHost = new ServiceHost(statelessTestServiceInstance))
            {
                using (ServiceHost statefulSimplexServiceHost = new ServiceHost(typeof (StatefulSimplexService)))
                {
                    using (ServiceHost statefulDuplexServiceHost = new ServiceHost(typeof (StatefulDuplexService)))
                    {
                        try
                        {
                            statelessServiceHost.Open();
                            statefulSimplexServiceHost.Open();
                            statefulDuplexServiceHost.Open();
                            Console.WriteLine("IPs: ");
                            foreach (IPAddress ip in HostUtils.GetLocalIpAddresses())
                            {
                                Console.WriteLine(ip.ToString());
                            }
                            Console.WriteLine();
                            Console.WriteLine("Press <ENTER> to terminate");
                            Console.ReadLine();
                            statelessServiceHost.Close();
                            statefulSimplexServiceHost.Close();
                            statefulDuplexServiceHost.Close();
                        }
                        catch (CommunicationException cex)
                        {
                            Console.WriteLine("An exception occurred: {0}", cex.Message);
                            statelessServiceHost.Abort();
                            statefulSimplexServiceHost.Abort();
                            statefulDuplexServiceHost.Abort();
                        }
                    }
                }
            }
        }
    }
}
