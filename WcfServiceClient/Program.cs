using System;
using System.Globalization;
using System.ServiceModel;
using System.Threading;
using WcfServiceClient.RestClients;

namespace WcfServiceClient
{
    class Program
    {
        static void Main()
        {
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
            try
            {
                using (var serviceClient = new StatelessServiceRestClient("WebHttpEndPointSslTerminate"))
                {
                    while (true)
                    {
                        int serverId = serviceClient.GetServerId();
                        Console.WriteLine("Server ID: " + serverId);
                        
                        ConsoleKeyInfo keyInfo = Console.ReadKey();
                        if (keyInfo.Key == ConsoleKey.Q)
                        {
                            break;
                        }
                    }
                }
            }
            catch (CommunicationException cex)
            {
                Console.WriteLine("An exception occurred: {0}", cex.Message);
            }
        }
    }
}
