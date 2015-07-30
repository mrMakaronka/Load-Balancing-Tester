using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WcfServiceLibrary;
using WcfServiceTest.Utils;
using WcfServiceLibrary.Utils;
using System.Configuration;
using System.Collections.Concurrent;
using NUnit.Framework;

namespace WcfServiceTest
{
    [TestFixture]
    public class TestServiceTest
    {
        private static StatelessServiceClient _statelessService = new StatelessServiceClient("BasicHttpEndPoint");

        private static int _serversNumber;
        private static int _requestsPerTest;
        private static Random _random;

        private ConcurrentDictionary<int, int> _responseNumberFromServers;

        static object[] ServiceBindingCases =
        {
            new object[] { new StatelessServiceClient("BasicHttpEndPoint") },
            new object[] { new StatelessServiceClient("WebHttpEndPoint") },
            new object[] { new StatelessServiceClient("NetTcpEndPoint") },
            new object[] { new StatelessServiceClient("UdpEndPoint") }
        };

        [TestFixtureSetUp]
        public static void ClassInitialize()
        {
            _serversNumber = int.Parse(ConfigurationManager.AppSettings["ServersNumber"]);
            _requestsPerTest = int.Parse(ConfigurationManager.AppSettings["RequestsPerTest"]);

            //_statelessService = new StatelessServiceClient("BasicHttpEndPoint");
            _random = new Random((int)DateTime.Now.Ticks);
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

        [Test, TestCaseSource("ServiceBindingCases")]
        public void GetServerIdTest(StatelessServiceClient statelessService)
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

        /*[TestMethod]
        public void RegisterUserTest()
        {
            for (int i = 0; i < _requestsPerTest; i++)
            {   
                //Arrange
                UserInfo userInfo = new UserInfo();
                userInfo.Name = RandomUtils.GetRandomString(_random, _random.Next(100));
                userInfo.Age = _random.Next();

                //Act
                RegisteredUserInfo registeredUserInfo = _statelessService.RegisterUser(userInfo);
                _responseNumberFromServers.AddOrUpdate(registeredUserInfo.ServerId, 1, (id, count) => count + 1);

                //Assert
                Assert.AreEqual(userInfo.Name, registeredUserInfo.UserInfo.Name);
                Assert.AreEqual(userInfo.Age, registeredUserInfo.UserInfo.Age);
            }
        }*/

        /*[TestMethod]
        public void GetDataUsingDataContractTestPutTrue()
        {
            //Arrange
            CompositeType testCompositeType = new CompositeType();
            testCompositeType.BoolValue = false;
            testCompositeType.StringValue = TestString;

            //Act
            CompositeType resultCompositeType = _testService.GetDataUsingDataContract(testCompositeType);

            //Assert
            Assert.AreEqual("FALSE", resultCompositeType.StringValue);
        }

        [TestMethod]
        public void GetDataUsingDataContractTestInnerType()
        {
            //Arrange
            CompositeType testCompositeType = new CompositeType();
            testCompositeType.BoolValue = false;
            testCompositeType.StringValue = TestString;

            const int TestStringsCount = 100;
            const int RandomStringSize = 100;
            string[] testStrings = RandomUtils.GetArrayOfRandomStrings(TestStringsCount, RandomStringSize);
            string[] expectedResultStrings = CryptoUtils.HashSHA1StringArray(testStrings);

            InnerCompositeType testInnerCompositeType = new InnerCompositeType();
            testInnerCompositeType.ListOfStrings = testStrings;
            testInnerCompositeType.ServerName = TestString;
            testCompositeType.InnerCompositeType = testInnerCompositeType;

            //Act
            CompositeType resultCompositeType = _testService.GetDataUsingDataContract(testCompositeType);

            //Assert
            Assert.AreEqual(ServerName, resultCompositeType.InnerCompositeType.ServerName);
            CollectionAssert.AreEqual(expectedResultStrings, resultCompositeType.InnerCompositeType.ListOfStrings);
        }

        [TestMethod]
        public void GetLargeDataTest()
        {
            //Arrange
            byte[] testBytes = Encoding.UTF8.GetBytes(TestString);
            byte[] nullBytes = new byte[TestString.Length];
            for (int i = 0; i < TestString.Length; i++)
            {
                nullBytes[i] = 0;
            }

            Stream testStream = new MemoryStream(testBytes);
            //Act
            try
            {
                string serverName = _testService.GetLargeData(TestString.Length, ref testStream);

                //Assert
                Assert.AreEqual(ServerName, serverName);
                byte[] buffer = new byte[TestString.Length];
                testStream.Read(buffer, 0, buffer.Length);
                CollectionAssert.AreEqual(testBytes, buffer);
                testStream.Read(buffer, 0, buffer.Length);
                CollectionAssert.AreEqual(nullBytes, buffer);
            }
            finally
            {
                if (testStream != null)
                {
                    ((IDisposable)testStream).Dispose();
                }
            }
        }*/
    }
}
