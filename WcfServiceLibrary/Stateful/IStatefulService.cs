using System.ServiceModel;

namespace WcfServiceLibrary
{
	[ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(ICalculatorDuplexCallback))]
	public interface IStatefulService
	{
        [OperationContract(IsInitiating = true, IsTerminating = false)]
        void Start(int initValue);

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        void AddTo(int n);

        [OperationContract(IsInitiating = false, IsTerminating = true)]
        void Stop();
	}

	public interface ICalculatorDuplexCallback
	{
        [OperationContract(IsOneWay = true)]
        void ServerId(int serverId);

		[OperationContract(IsOneWay = true)]
		void Equals(int result);

		[OperationContract(IsOneWay = true)]
		void Equation(string eqn);
	}
}
