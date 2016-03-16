using System;
using System.Globalization;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using WcfServiceClient.RestClients;

namespace WcfServiceClient
{
    class Program
    {
        static void Main()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
            try
            {
                /*using (var statelessServiceClient = new StatelessServiceClient("BasicHttpEndPoint"))
                {
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
                }*/

                InstanceContext instanceContext = new InstanceContext(new CallbackHandler());
                StatefulServiceClient statefulServiceClient = new StatefulServiceClient(instanceContext);

                using (new OperationContextScope(statefulServiceClient.InnerChannel))
                {
                    var prop = new HttpRequestMessageProperty();
                    prop.Headers.Add(HttpRequestHeader.Cookie, "ServerId=2");
                    OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, prop);

                    statefulServiceClient.Start(0);
                    statefulServiceClient.AddTo(7);
                    statefulServiceClient.AddTo(4);
                    statefulServiceClient.Stop();
                }

                Console.ReadLine();
                statefulServiceClient.Close();
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

            public void Equals(double result)
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
