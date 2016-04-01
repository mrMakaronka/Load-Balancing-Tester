using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Description;
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
        private static bool _duplexChannel;

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
            _duplexChannel = bool.Parse(ConfigurationManager.AppSettings["DuplexChannel"]);

            _random = new Random((int)DateTime.Now.Ticks);
            _randomMinvalue = 0;
            _randomMaxvalue = Int32.MaxValue / (_requestsPerSession + 1); // Consider initial value
            _instanceContext = new InstanceContext(new CallbackHandler());
        }

        [SetUp]
        public void TestInitialize()
        {
            _responseNumberFromServers = new ConcurrentDictionary<int, int>();
            for (int i = 1; i <= _serversNumber; i++)
            {
                _responseNumberFromServers.TryAdd(i, 0);
            }
        }

        [Test]
        public void AllSessionsToOneServerTest()
        {
            for (int i = 0; i < _sessionsPerTest; i++)
            {
                //Arrange
                _currentResult = 0;
                Tuple<int, string> resultTuple;

                //Act
                if (_duplexChannel)
                {
                    StatefulDuplexServiceClient statefulDuplexServiceClient = new StatefulDuplexServiceClient(_instanceContext, _bindingForTest);
                    resultTuple = ExecuteDuplexTestAction(statefulDuplexServiceClient);
                }
                else
                {
                    StatefulSimplexServiceClient statefulSimplexServiceClient = new StatefulSimplexServiceClient(_bindingForTest);
                    resultTuple = ExecuteSimplexTestAction(statefulSimplexServiceClient);
                }

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
                Tuple<int, string> resultTuple;
                int serverId = (i % _serversNumber) + 1;

                //Act
                if (_duplexChannel)
                {
                    StatefulDuplexServiceClient statefulDuplexServiceClient = new StatefulDuplexServiceClient(_instanceContext, _bindingForTest);
                    AddUrlParamToAllRequests(statefulDuplexServiceClient.Endpoint, _serverIdUrlParamName, serverId.ToString());
                    resultTuple = ExecuteDuplexTestAction(statefulDuplexServiceClient);
                }
                else
                {
                    StatefulSimplexServiceClient statefulDuplexServiceClient = new StatefulSimplexServiceClient(_bindingForTest);
                    AddUrlParamToAllRequests(statefulDuplexServiceClient.Endpoint, _serverIdUrlParamName, serverId.ToString());
                    resultTuple = ExecuteSimplexTestAction(statefulDuplexServiceClient);
                }

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
                Tuple<int, string> resultTuple;
                int stickyRandomParam = _random.Next();

                //Act
                if (_duplexChannel)
                {
                    StatefulDuplexServiceClient statefulDuplexServiceClient = new StatefulDuplexServiceClient(_instanceContext, _bindingForTest);
                    AddUrlParamToAllRequests(statefulDuplexServiceClient.Endpoint, _stickyUrlParamName, stickyRandomParam.ToString());
                    resultTuple = ExecuteDuplexTestAction(statefulDuplexServiceClient);
                }
                else
                {
                    StatefulSimplexServiceClient statefulDuplexServiceClient = new StatefulSimplexServiceClient(_bindingForTest);
                    AddUrlParamToAllRequests(statefulDuplexServiceClient.Endpoint, _stickyUrlParamName, stickyRandomParam.ToString());
                    resultTuple = ExecuteSimplexTestAction(statefulDuplexServiceClient);
                }

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

        private Tuple<int, string> ExecuteSimplexTestAction(StatefulSimplexServiceClient statefulSimplexServiceClient)
        {
            int initialNum = GetRandomInCorrectRange();
            int serverId = statefulSimplexServiceClient.Start(initialNum);
            _responseNumberFromServers.AddOrUpdate(serverId, 1, (id, count) => count + 1);

            int expectedResult = initialNum;
            StringBuilder expectedStringResultStringBuilder = new StringBuilder(initialNum.ToString());

            for (int j = 0; j < _requestsPerSession; j++)
            {
                int summand = GetRandomInCorrectRange();
                _currentResult = statefulSimplexServiceClient.AddTo(summand);

                expectedResult += summand;
                expectedStringResultStringBuilder.Append(string.Format(" + {0}", summand));
            }
            _currentEquationStringResult = statefulSimplexServiceClient.Stop();
            statefulSimplexServiceClient.Close();

            expectedStringResultStringBuilder.Append(string.Format(" = {0}", _currentResult));
            string expectedStringResult = expectedStringResultStringBuilder.ToString();

            Tuple<int, string> resultTuple = new Tuple<int, string>(expectedResult, expectedStringResult);
            return resultTuple;
        }

        private Tuple<int, string> ExecuteDuplexTestAction(StatefulDuplexServiceClient statefulDuplexServiceClient)
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

        private void AddUrlParamToAllRequests(ServiceEndpoint serviceEndpoint, string urlParamName, string urlParamValue)
        {
            string newServiceUriString = string.Format(serviceEndpoint.Address.Uri.AbsoluteUri + "?{0}={1}", urlParamName, urlParamValue);
            Uri newServiceUri = new Uri(newServiceUriString);
            EndpointAddress endpointAddress = new EndpointAddress(newServiceUri);
            serviceEndpoint.Address = endpointAddress;
        }

        private int GetRandomInCorrectRange()
        {
            int random = _random.Next(_randomMinvalue, _randomMaxvalue);
            return random;
        }

        #endregion
    }
}
