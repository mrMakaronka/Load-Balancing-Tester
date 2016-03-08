using System.Globalization;
using System.ServiceModel;

namespace WcfServiceLibrary
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession, AddressFilterMode = AddressFilterMode.Any)]
	public class StatefulService : IStatefulService
	{
		private double _result;
		private string _equation;

		public void Start(double initValue)
		{
		    _result = initValue;
		    _equation = initValue.ToString(CultureInfo.InvariantCulture);
		}

		public void AddTo(double n)
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
