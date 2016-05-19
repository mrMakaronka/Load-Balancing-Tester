using System;
using System.Globalization;
using System.ServiceModel;
using System.Threading;

namespace WcfServiceClient
{
    class Program
    {
        static void Main()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
            try
            {
                StatelessServiceClient statelessServiceClient = new StatelessServiceClient("BasicHttpEndPoint");
                while (true)
                {
                    int serverId = statelessServiceClient.GetServerId();
                    Console.WriteLine("Server ID: " + serverId);
                        
                    ConsoleKeyInfo keyInfo = Console.ReadKey();
                    if (keyInfo.Key == ConsoleKey.Q)
                    {
                        break;
                    }
                }
                statelessServiceClient.Close();
                Console.WriteLine();

                InstanceContext instanceContext = new InstanceContext(new CallbackHandler());
                StatefulDuplexServiceClient statefulDuplexServiceClient = new StatefulDuplexServiceClient(instanceContext, "NetTcpCustomStatefulEndPoint");
                statefulDuplexServiceClient.Start(0);
                for (int i = 0; i < 10; i++)
                    statefulDuplexServiceClient.AddTo(4);

                statefulDuplexServiceClient.Stop();
                statefulDuplexServiceClient.Close();
                Console.ReadLine();
                Console.WriteLine();

                StatefulSimplexServiceClient statefulSimplexServiceClient = new StatefulSimplexServiceClient("WsHttpEndPoint");
                int serverIdSimplex = statefulSimplexServiceClient.Start(0);
                Console.WriteLine("ServerId: {0}", serverIdSimplex);
                for (int i = 0; i < 10; i++)
                {
                    int result = statefulSimplexServiceClient.AddTo(5);
                    Console.WriteLine("Equals({0})", result);
                }
                string equationResult = statefulSimplexServiceClient.Stop();
                Console.WriteLine("Equation({0})", equationResult);
                statefulSimplexServiceClient.Close();
                Console.ReadLine();
            }
            catch (CommunicationException cex)
            {
                Console.WriteLine("An exception occurred: {0}", cex.Message);
            }
        }

        [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
        public class CallbackHandler : IStatefulDuplexServiceCallback
        {
            public void ServerId(int serverId)
            {
                Console.WriteLine("ServerId: {0}", serverId);
            }

            public void Equals(int result)
            {
                Console.WriteLine("Equals({0})", result);
            }

            public void Equation(string equationResult)
            {
                Console.WriteLine("Equation({0})", equationResult);
            }
        }
    }
}
