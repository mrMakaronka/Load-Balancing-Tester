using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using WcfServiceLibrary;

namespace WcfServiceClient
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                while (true)
                {

                    //ChannelFactory<IStatelessServiceChannel> factory = new ChannelFactory<IStatelessServiceChannel>("WebHttpEndPoint");
                    //factory.Endpoint.Behaviors.Add(new WebHttpBehavior());

                    /*IStatelessServiceChannel client = factory.CreateChannel();

                    // Fixes the problem by creating a new service context for the following service call
                    using (new OperationContextScope((IContextChannel)client))
                    {
                        int serverId = client.GetServerId();
                        Console.WriteLine(serverId);
                    }*/
                    using (var testService = new StatelessServiceClient("WebHttpEndPoint"))
                    {
                        int serverId = testService.GetServerId();
                        Console.WriteLine(serverId);


                        //string executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                        /*string fileName = "FileForUploading.jpg";
                        string testFileLocation = Path.Combine(executableLocation, "Resources\\" + fileName);

                        using (FileStream fileStream = File.Open(testFileLocation, FileMode.Open))
                        {
                            UploadedFileInfo uploadedFileInfo = testService.UploadFile(fileStream);
                            Console.WriteLine("ID: " + uploadedFileInfo.FileSize);
                        }*/
                        //Stream fileStream = testService.DownloadFile();
                        //byte[] serverIdBytes = new byte[sizeof(int)];
                        //fileStream.Read(serverIdBytes, 0, serverIdBytes.Length);
                        //fileStream.ReadByte();
                        //BinaryReader binaryReader = new BinaryReader(fileStream);

                        //int serverId = binaryReader.ReadInt32();//BitConverter.ToInt32(serverIdBytes, 0);
                        /*int serverId = BitConverter.ToInt32(serverIdBytes, 0);
                        Console.WriteLine("Id: " + serverId);

                        long length = 0;
                        string uploadedFilePath = Path.Combine(executableLocation, "Resources\\DownloadedFile.jpg");
                        using (FileStream targetStream = File.Create(uploadedFilePath))
                        {
                            using (fileStream)
                            {
                                const int bufferLen = 65536;
                                byte[] buffer = new byte[bufferLen];
                                int count = 0;
                                while ((count = fileStream.Read(buffer, 0, bufferLen)) > 0)
                                {
                                    targetStream.Write(buffer, 0, count);
                                }
                                length = targetStream.Length;
                            }
                        }

                        Console.WriteLine("Size: " + length);*/
                        //testService.Abort();
                    }
                    Console.ReadKey();
                }
            }
            catch (CommunicationException cex)
            {
                Console.WriteLine("An exception occurred: {0}", cex.Message);
            }
        }
    }
}
