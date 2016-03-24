using System.Configuration;
using System.Globalization;
using System.ServiceModel;

namespace WcfServiceLibrary
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession, AddressFilterMode = AddressFilterMode.Any)]
	public class StatefulService : IStatefulService
	{
        private int _result;
		private string _equation;
        private readonly int _serverId;

        public StatefulService()
        {
            //InstanceContextMode is set to PerSession, so service object can not be passed to ServiceHost constructor, so we can not pass serverId to service using constructor.
            //Better practice is to implement a combination of custom ServiceHostFactory, ServiceHost and IInstanceProvider, but for test purposes it is enough.
            int.TryParse(ConfigurationManager.AppSettings["ServerId"], out _serverId);
        }

		public void Start(int initValue)
		{
		    _result = initValue;
		    _equation = initValue.ToString(CultureInfo.InvariantCulture);
            Callback.ServerId(_serverId);
		}

        public void AddTo(int n)
		{
			_result += n;
			_equation += " + " + n.ToString(CultureInfo.InvariantCulture);
			Callback.Equals(_result);
		}

        public void Stop()
        {
            Callback.Equation(_equation + " = " + _result.ToString(CultureInfo.InvariantCulture));
        }

		ICalculatorDuplexCallback Callback
		{
			get
			{
				return OperationContext.Current.GetCallbackChannel<ICalculatorDuplexCallback>();
			}
		}
	}
}
