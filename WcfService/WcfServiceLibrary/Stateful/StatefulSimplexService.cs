using System.Configuration;
using System.Globalization;
using System.ServiceModel;

namespace WcfServiceLibrary
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession, AddressFilterMode = AddressFilterMode.Any)]
    public class StatefulSimplexService : IStatefulSimplexService
    {
        private int _result;
        private string _equation;
        private readonly int _serverId;

        public StatefulSimplexService()
        {
            //InstanceContextMode is set to PerSession, so service object can not be passed to ServiceHost constructor, so we can not pass serverId to service using constructor.
            //Better practice is to implement a combination of custom ServiceHostFactory, ServiceHost and IInstanceProvider, but for test purposes it is enough.
            int.TryParse(ConfigurationManager.AppSettings["ServerId"], out _serverId);
        }

        public int Start(int initValue)
        {
            _result = initValue;
            _equation = initValue.ToString(CultureInfo.InvariantCulture);
            return _serverId;
        }

        public int AddTo(int n)
        {
            _result += n;
            _equation += " + " + n.ToString(CultureInfo.InvariantCulture);
            return _result;
        }

        public string Stop()
        {
            string equationResult = _equation + " = " + _result.ToString(CultureInfo.InvariantCulture);
            return equationResult;
        }
    }
}
