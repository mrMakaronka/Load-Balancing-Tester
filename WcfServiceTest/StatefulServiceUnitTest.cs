using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;
using System.Text;
using NUnit.Framework;

namespace WcfServiceTest
{
    [TestFixture]
    public class StatefulServiceTest
    {
        private static int _serversNumber;
        private static int _sessionsPerTest;
        private static int _requestsPerSession;
        private static string _bindingForTest;
        private static string _serverIdUrlParamName;
        private static string _stickyUrlParamName;

        private static Random _random;
        private static int _randomMinvalue;
        private static int _randomMaxvalue;

        private static ConcurrentDictionary<int, int> _responseNumberFromServers;
        private static int _currentResult;
        private static string _currentEquationStringResult;

        private static InstanceContext _instanceContext;

        [TestFixtureSetUp]
        public static void ClassInitialize()
        {
            _serversNumber = int.Parse(ConfigurationManager.AppSettings["ServersNumber"]);
            _sessionsPerTest = int.Parse(ConfigurationManager.AppSettings["SessionsPerTest"]);
            _requestsPerSession = int.Parse(ConfigurationManager.AppSettings["RequestsPerSession"]);
            _bindingForTest = ConfigurationManager.AppSettings["StatefulBindingForTest"];
            _serverIdUrlParamName = ConfigurationManager.AppSettings["ServerIdUrlParamName"];
            _stickyUrlParamName = ConfigurationManager.AppSettings["StickyUrlParamName"];

            _random = new Random((int)DateTime.Now.Ticks);
            _randomMinvalue = 0;
            _randomMaxvalue = Int32.MaxValue/(_requestsPerSession + 1); // Consider initial value
            _instanceContext = new InstanceContext(new CallbackHandler());
        }

        [SetUp]
        public void TestInitialize()
        {
            _responseNumberFromServers = new ConcurrentDictionary<int, int>();
        }

        [Test]
        public void AllSessionsToOneServerTest()
        {
            for (int i = 0; i < _sessionsPerTest; i++)
            {
                // Arrange
                _currentResult = 0;
                StatefulDuplexServiceClient statefulDuplexServiceClient = new StatefulDuplexServiceClient(_instanceContext, _bindingForTest);

                // Act
                Tuple<int, string> resultTuple = ExecuteTestAction(statefulDuplexServiceClient);

                //Assert
                Assert.AreEqual(resultTuple.Item1, _currentResult);
                Assert.AreEqual(resultTuple.Item2, _currentEquationStringResult);
            }
            Assert.True(_responseNumberFromServers.Values.Contains(_sessionsPerTest));
        }

        [Test]
        public void SessionsToDifferentServersChooseServerTest()
        {
            for (int i = 0; i < _sessionsPerTest; i++)
            {
                //Arrange
                _currentResult = 0;

                StatefulDuplexServiceClient statefulDuplexServiceClient = new StatefulDuplexServiceClient(_instanceContext, _bindingForTest);
                int serverId = (i % _serversNumber) + 1;
                AddUrlParamToAllRequests(statefulDuplexServiceClient, _serverIdUrlParamName, serverId.ToString());

                //Act
                Tuple<int, string> resultTuple = ExecuteTestAction(statefulDuplexServiceClient);

                //Assert
                Assert.AreEqual(resultTuple.Item1, _currentResult);
                Assert.AreEqual(resultTuple.Item2, _currentEquationStringResult);
            }
            foreach (KeyValuePair<int, int> responseNumber in _responseNumberFromServers)
            {
                Assert.AreEqual(_sessionsPerTest / _serversNumber, responseNumber.Value); //Assume that _sessionsPerTest % _serversNumber = 0
            }
        }

        [Test]
        public void SessionsToDifferentServersRandomServerTest()
        {
            for (int i = 0; i < _sessionsPerTest; i++)
            {
                //Arrange
                _currentResult = 0;

                StatefulDuplexServiceClient statefulDuplexServiceClient = new StatefulDuplexServiceClient(_instanceContext, _bindingForTest);
                int stickyRandomParam = _random.Next();
                AddUrlParamToAllRequests(statefulDuplexServiceClient, _stickyUrlParamName, stickyRandomParam.ToString());

                //Act
                Tuple<int, string> resultTuple = ExecuteTestAction(statefulDuplexServiceClient);

                //Assert
                Assert.AreEqual(resultTuple.Item1, _currentResult);
                Assert.AreEqual(resultTuple.Item2, _currentEquationStringResult);
            }
            foreach (KeyValuePair<int, int> responseNumber in _responseNumberFromServers)
            {
                Assert.True(responseNumber.Value > 0);
            }
        }

        #region Stateful service callback

        [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
        public class CallbackHandler : IStatefulDuplexServiceCallback
        {
            public void ServerId(int serverId)
            {
                _responseNumberFromServers.AddOrUpdate(serverId, 1, (id, count) => count + 1);
            }

            public void Equals(int result)
            {
                _currentResult = result;
            }

            public void Equation(string eqationString)
            {
                _currentEquationStringResult = eqationString;
            }
        }

        #endregion

        #region Auxiliary functionality

        private Tuple<int, string> ExecuteTestAction(StatefulDuplexServiceClient statefulDuplexServiceClient)
        {
            int initialNum = GetRandomInCorrectRange();
            statefulDuplexServiceClient.Start(initialNum);

            int expectedResult = initialNum;
            StringBuilder expectedStringResultStringBuilder = new StringBuilder(initialNum.ToString());

            for (int j = 0; j < _requestsPerSession; j++)
            {
                int summand = GetRandomInCorrectRange();
                statefulDuplexServiceClient.AddTo(summand);

                expectedResult += summand;
                expectedStringResultStringBuilder.Append(string.Format(" + {0}", summand));
            }
            statefulDuplexServiceClient.Stop();
            statefulDuplexServiceClient.Close();

            expectedStringResultStringBuilder.Append(string.Format(" = {0}", _currentResult));
            string expectedStringResult = expectedStringResultStringBuilder.ToString();
            
            Tuple<int, string> resultTuple = new Tuple<int, string>(expectedResult, expectedStringResult);
            return resultTuple;
        }

        private void AddUrlParamToAllRequests(StatefulDuplexServiceClient statefulDuplexServiceClient, string urlParamName, string urlParamValue)
        {
            string newServiceUriString =
                string.Format(statefulDuplexServiceClient.Endpoint.Address.Uri.AbsoluteUri + "?{0}={1}", urlParamName,
                    urlParamValue);
            Uri newServiceUri = new Uri(newServiceUriString);
            EndpointAddress endpointAddress = new EndpointAddress(newServiceUri);
            statefulDuplexServiceClient.Endpoint.Address = endpointAddress;
        }

        private int GetRandomInCorrectRange()
        {
            int random = _random.Next(_randomMinvalue, _randomMaxvalue);
            return random;
        }

        #endregion
    }
}
