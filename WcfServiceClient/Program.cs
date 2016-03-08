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
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
            try
            {
                /*using (var statelessServiceClient = new StatelessServiceClient("BasicHttpEndPoint"))
                {*/
                    /*while (true)
                    {*/
                        //int serverId = statelessServiceClient.GetServerId();
                        //Console.WriteLine("Server ID: " + serverId);
                        
                        /*ConsoleKeyInfo keyInfo = Console.ReadKey();
                        if (keyInfo.Key == ConsoleKey.Q)
                        {
                            break;
                        }*/
                   // }
                //}

                InstanceContext instanceContext = new InstanceContext(new CallbackHandler());
                var statefulServiceClient = new StatefulServiceClient(instanceContext);
                
                statefulServiceClient.Start(0);
                statefulServiceClient.AddTo(7);
                statefulServiceClient.AddTo(4);
                statefulServiceClient.Stop();
                
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
