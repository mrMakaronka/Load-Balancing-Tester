using System.Globalization;
using System.ServiceModel;

namespace WcfServiceLibrary
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession, AddressFilterMode = AddressFilterMode.Any)]
	public class StatefulService : IStatefulService
	{
		private double _result;
		private string _equation;

		public StatefulService()
		{
			_equation = _result.ToString(CultureInfo.InvariantCulture);
		}

		public void Clear()
		{
			Callback.Equation(_equation + " = " + _result.ToString(CultureInfo.InvariantCulture));
			_equation = _result.ToString(CultureInfo.InvariantCulture);
		}

		public void AddTo(double n)
		{
			_result += n;
			_equation += " + " + n.ToString(CultureInfo.InvariantCulture);
			Callback.Equals(_result);
		}

		public void SubtractFrom(double n)
		{
			_result -= n;
			_equation += " - " + n.ToString(CultureInfo.InvariantCulture);
			Callback.Equals(_result);
		}

		public void MultiplyBy(double n)
		{
			_result *= n;
			_equation += " * " + n.ToString(CultureInfo.InvariantCulture);
			Callback.Equals(_result);
		}

		public void DivideBy(double n)
		{
			_result /= n;
			_equation += " / " + n.ToString(CultureInfo.InvariantCulture);
			Callback.Equals(_result);
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
