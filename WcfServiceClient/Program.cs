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
                StatefulServiceClient statefulServiceClient = new StatefulServiceClient(instanceContext, "NetTcpCustomStatefulEndPoint");

                using (new OperationContextScope(statefulServiceClient.InnerChannel))
                {
                    statefulServiceClient.Start(0);
                    for (int i = 0; i < 10; i++)
                        statefulServiceClient.AddTo(4);

                    statefulServiceClient.Stop();
                    statefulServiceClient.Close();
                }
                Console.ReadLine();
            }
            catch (CommunicationException cex)
            {
                Console.WriteLine("An exception occurred: {0}", cex.Message);
            }
        }

        [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
        public class CallbackHandler : IStatefulServiceCallback
        {
            public void ServerId(int serverId)
            {
                Console.WriteLine("ServerId: {0}", serverId);
            }

            public void Equals(int result)
            {
                Console.WriteLine("Equals({0})", result);
            }

            public void Equation(string eqn)
            {
                Console.WriteLine("Equation({0})", eqn);
            }
        }
    }
}
