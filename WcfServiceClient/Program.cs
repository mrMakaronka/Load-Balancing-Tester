using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WcfServiceClient
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (var testService = new TestServiceClient())
                {
                    string message = testService.GetStringData("Test string");
                    Console.WriteLine(message);
                }
            }
            catch (CommunicationException cex)
            {
                Console.WriteLine("An exception occurred: {0}", cex.Message);
            }
        }
    }
}
