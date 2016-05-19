using System.ServiceModel;

namespace WcfServiceLibrary
{
	[ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IDuplexCallback))]
	public interface IStatefulDuplexService
	{
        [OperationContract(IsInitiating = true, IsTerminating = false)]
        void Start(int initValue);

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        void AddTo(int n);

        [OperationContract(IsInitiating = false, IsTerminating = true)]
        void Stop();
	}

	public interface IDuplexCallback
	{
        [OperationContract(IsOneWay = true)]
        void ServerId(int serverId);

		[OperationContract(IsOneWay = true)]
		void Equals(int result);

		[OperationContract(IsOneWay = true)]
		void Equation(string eqn);
	}
}
