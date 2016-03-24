using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using NUnit.Framework;
using WcfServiceClient.RestClients;
using WcfServiceLibrary;
using WcfServiceTest.Utils;
using Assert = NUnit.Framework.Assert;

namespace WcfServiceTest
{
    [TestFixture]
    public class StatelessServiceTest : IDisposable
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable (used in tests)
        private static readonly IList<IStatelessService> ServiceBindingCasesWithStreaming;
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable (used in tests)
        private static readonly IList<IStatelessService> ServiceBindingCasesWithoutStreaming;

        private static int _serversNumber;
        private static int _requestsPerTest;
        private static Random _random;

        private ConcurrentDictionary<int, int> _responseNumberFromServers;

        static StatelessServiceTest()
        {
            IStatelessService statelessService;
            string bindingName = ConfigurationManager.AppSettings["StatelessBindingForTest"];
            bool restService = bool.Parse(ConfigurationManager.AppSettings["RestService"]);
            if (restService)
            {
                statelessService = new StatelessServiceRestClient(bindingName);
            }
            else
            {
                statelessService = new StatelessServiceClient(bindingName);
            }

            ServiceBindingCasesWithoutStreaming = new List<IStatelessService> {statelessService};
            ServiceBindingCasesWithStreaming = new List<IStatelessService>();
            bool supportStreaming = bool.Parse(ConfigurationManager.AppSettings["SupportStreaming"]);
            if (supportStreaming)
            {
                ServiceBindingCasesWithStreaming.Add(statelessService);
            }
        }

        [TestFixtureSetUp]
        public static void ClassInitialize()
        {
            _serversNumber = int.Parse(ConfigurationManager.AppSettings["ServersNumber"]);
            _requestsPerTest = int.Parse(ConfigurationManager.AppSettings["RequestsPerTest"]);
            _random = new Random((int)DateTime.Now.Ticks);
        }

        [TestFixtureTearDown]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [SetUp]
        public void TestInitialize()
        {
            _responseNumberFromServers = new ConcurrentDictionary<int, int>();
        }

        [TearDown]
        public void TestCleanup()
        {
            foreach (KeyValuePair<int, int> responseNumber in _responseNumberFromServers)
            {
                Assert.AreEqual(_requestsPerTest / _serversNumber, responseNumber.Value); //Assume that _requestsPerTest % _serversNumber = 0
            }
        }

        [Test, TestCaseSource("ServiceBindingCasesWithoutStreaming")]
        public void GetServerIdTest(IStatelessService statelessService)
        {
            //Act
            for (int i = 0; i < _requestsPerTest; i++)
            {
                int serverId = statelessService.GetServerId();
                _responseNumberFromServers.AddOrUpdate(serverId, 1, (id, count) => count + 1);
            }

            //Assert
            //The only assertion for this test is contained in TestCleanup method
        }

        [Test, TestCaseSource("ServiceBindingCasesWithoutStreaming")]
        public void RegisterUserTest(IStatelessService statelessService)
        {
            for (int i = 0; i < _requestsPerTest; i++)
            {   
                //Arrange
                UserInfo userInfo = new UserInfo
                {
                    Name = RandomUtils.GetRandomString(_random, _random.Next(100)),
                    Age = _random.Next()
                };

                //Act
                RegisteredUserInfo registeredUserInfo = statelessService.RegisterUser(userInfo);
                _responseNumberFromServers.AddOrUpdate(registeredUserInfo.ServerId, 1, (id, count) => count + 1);

                //Assert
                Assert.AreEqual(userInfo.Name, registeredUserInfo.UserInfo.Name);
                Assert.AreEqual(userInfo.Age, registeredUserInfo.UserInfo.Age);
            }
        }

        [Test, TestCaseSource("ServiceBindingCasesWithStreaming")]
        public void UploadFileTest(IStatelessService statelessService)
        {
            //Arrange
            string fileForUploadingPath = ConfigurationManager.AppSettings["FileForUploadingPath"];
            for (int i = 0; i < _requestsPerTest; i++)
            {
                using (FileStream fileStream = File.Open(fileForUploadingPath, FileMode.Open))
                {
                    //Act
                    UploadedFileInfo uploadedFileInfo = statelessService.UploadFile(fileStream);
                    _responseNumberFromServers.AddOrUpdate(uploadedFileInfo.ServerId, 1, (id, count) => count + 1);

                    //Assert
                    Assert.AreEqual(fileStream.Length, uploadedFileInfo.FileSize);
                }
            }
        }

        [Test, TestCaseSource("ServiceBindingCasesWithStreaming")]
        public void DownloadFileTest(IStatelessService statelessService)
        {
            //Arrange
            string downloadedFilePath = ConfigurationManager.AppSettings["DownloadedFilePath"];
            int downloadedFileExpectedLength = 39564720;

            for (int i = 0; i < _requestsPerTest; i++)
            {
                //Act
                using (Stream downloadedFileStream = statelessService.DownloadFile())
                {
                    byte[] serverIdBytes = new byte[sizeof(int)];
                    downloadedFileStream.Read(serverIdBytes, 0, serverIdBytes.Length);
                    int serverId = BitConverter.ToInt32(serverIdBytes, 0);
                    _responseNumberFromServers.AddOrUpdate(serverId, 1, (id, count) => count + 1);

                    using (FileStream downloadedTmpFileStream = File.Create(downloadedFilePath))
                    {
                        downloadedFileStream.CopyTo(downloadedTmpFileStream);
                        
                        //Assert
                        Assert.AreEqual(downloadedFileExpectedLength, downloadedTmpFileStream.Length);
                    }
                    File.Delete(downloadedFilePath);
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (IStatelessService statelessService in ServiceBindingCasesWithoutStreaming)
                {
                    ((IDisposable)statelessService).Dispose();
                }
            }
        }
    }
}
