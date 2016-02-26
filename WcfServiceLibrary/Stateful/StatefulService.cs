using System.Globalization;
using System.ServiceModel;

namespace WcfServiceLibrary
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
	public class StatefulService : IStatefulService
	{
		double _Result;
		string _Equation;

		public StatefulService()
		{
			_Equation = _Result.ToString(CultureInfo.InvariantCulture);
		}

		public void Clear()
		{
			Callback.Equation(_Equation + " = " + _Result.ToString(CultureInfo.InvariantCulture));
			_Equation = _Result.ToString(CultureInfo.InvariantCulture);
		}

		public void AddTo(double n)
		{
			_Result += n;
			_Equation += " + " + n.ToString(CultureInfo.InvariantCulture);
			Callback.Equals(_Result);
		}

		public void SubtractFrom(double n)
		{
			_Result -= n;
			_Equation += " - " + n.ToString(CultureInfo.InvariantCulture);
			Callback.Equals(_Result);
		}

		public void MultiplyBy(double n)
		{
			_Result *= n;
			_Equation += " * " + n.ToString(CultureInfo.InvariantCulture);
			Callback.Equals(_Result);
		}

		public void DivideBy(double n)
		{
			_Result /= n;
			_Equation += " / " + n.ToString(CultureInfo.InvariantCulture);
			Callback.Equals(_Result);
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
