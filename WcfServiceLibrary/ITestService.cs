using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfServiceLibrary
{
    [ServiceContract]
    public interface ITestService
    {
        [OperationContract]
        string GetStringData(string stringValue);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        [OperationContract]
        OutputStreamMessage GetLargeData(InputStreamMessage inputMessage);
    }

    [DataContract]
    public class CompositeType
    {
        private bool _boolValue;
        private string _stringValue;
        private InnerCompositeType _innerCompositeType;

        [DataMember]
        public bool BoolValue
        {
            get { return _boolValue; }
            set { _boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return _stringValue; }
            set { _stringValue = value; }
        }

        [DataMember]
        public InnerCompositeType InnerCompositeType
        {
            get { return _innerCompositeType; }
            set { _innerCompositeType = value; }
        }
    }

    [DataContract]
    public class InnerCompositeType
    {
        private IList<string> _listOfStrings;
        private string _serverName;

        public InnerCompositeType()
        {
            _listOfStrings = new List<string>();
        }

        [DataMember]
        public IList<string> ListOfStrings
        {
            get { return _listOfStrings; }
        }

        [DataMember]
        public string ServerName
        {
            get { return _serverName; }
            set { _serverName = value; }
        }
    }

    [MessageContract]
    public class InputStreamMessage
    {
        private int _nullCount;
        private Stream _data;

        [MessageHeader]
        public int NullCount {
            get { return _nullCount; }
            set { _nullCount = value; }
        }
        [MessageBodyMember]
        public Stream Data {
            get { return _data; }
            set { _data = value; }
        }
    }

    [MessageContract]
    public class OutputStreamMessage
    {
        private string _serverName;
        private Stream _data;

        [MessageHeader]
        public string ServerName
        {
            get { return _serverName; }
            set { _serverName = value; }
        }
        [MessageBodyMember]
        public Stream Data
        {
            get { return _data; }
            set { _data = value; }
        }
    } 
}
