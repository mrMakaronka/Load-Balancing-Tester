using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace WcfServiceLibrary
{
    [ServiceContract]
    public interface IStatelessService
    {
        [OperationContract]
        [WebGet]
        int GetServerId();

        [OperationContract]
        [WebInvoke(Method = "POST")]
        RegisteredUserInfo RegisterUser(UserInfo userInfo);

        [OperationContract]
        [WebInvoke(Method = "POST")]
        UploadedFileInfo UploadFile(Stream fileStream);

        [OperationContract]
        [WebInvoke(Method = "POST")]
        Stream DownloadFile();
    }

    [DataContract]
    public class UserInfo
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int Age { get; set; }
    }

    [DataContract]
    public class RegisteredUserInfo
    {
        [DataMember]
        public UserInfo UserInfo { get; set; }

        [DataMember]
        public int ServerId { get; set; }
    }

    [DataContract]
    public class UploadedFileInfo
    {
        [DataMember]
        public long FileSize { get; set; }

        [DataMember]
        public int ServerId { get; set; }
    }
}
