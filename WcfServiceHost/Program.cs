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

namespace WcfServiceHost
{
    class Program
    {
        static void Main()
        {
            string serverName = ConfigurationManager.AppSettings["ServerName"];
            if (serverName == null)
            {
                throw new ConfigurationErrorsException("Parameter ServerName is missed");
            }

            TestService testServiceInstance = new TestService(serverName);
            using (ServiceHost host = new ServiceHost(testServiceInstance))
            {
                try
                {
                    host.Open();
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
