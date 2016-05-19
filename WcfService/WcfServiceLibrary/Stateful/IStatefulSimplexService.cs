using System.ServiceModel;

namespace WcfServiceLibrary
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface IStatefulSimplexService
    {
        [OperationContract(IsInitiating = true, IsTerminating = false)]
        int Start(int initValue);

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        int AddTo(int n);

        [OperationContract(IsInitiating = false, IsTerminating = true)]
        string Stop();
    }
}
