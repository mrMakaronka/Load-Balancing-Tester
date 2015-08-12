using System;
using System.ServiceModel;
using WcfServiceClient.RestClients;

namespace WcfServiceClient
{
    class Program
    {
        static void Main()
        {
            try
            {
                using (var serviceClient = new StatelessServiceRestClient("WebHttpEndPoint"))
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
