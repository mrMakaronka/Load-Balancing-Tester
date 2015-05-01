using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfServiceLibrary.Utils;

namespace WcfServiceLibrary
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class TestService : ITestService
    {
        private string _serverName;

        public TestService(string serverName)
        {
            _serverName = serverName;
        }

        public string GetStringData(string stringValue)
        {
            string hashString = EncryptionUtils.HashSHA1String(stringValue);
            string resultString = _serverName + "_" + hashString;
            return resultString;
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }

            if (composite.BoolValue)
            {
                composite.StringValue = "TRUE";
            }
            else
            {
                composite.StringValue = "FALSE";
            }
            if (composite.InnerCompositeType != null)
            {
                composite.InnerCompositeType.ServerName = _serverName;
                for (int i = 0; i < composite.InnerCompositeType.ListOfStrings.Count; i++)
                {
                    composite.InnerCompositeType.ListOfStrings[i] = EncryptionUtils.HashSHA1String(composite.InnerCompositeType.ListOfStrings[i]);
                }
            }

            return composite;
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller must dispose")]
        public OutputStreamMessage GetLargeData(InputStreamMessage inputMessage)
        {
            if (inputMessage == null)
            {
                throw new ArgumentNullException("inputMessage");
            }

            int nullBytesCount = inputMessage.NullCount;
            byte[] nullBytes = new byte[nullBytesCount];
            for (int i = 0; i < nullBytesCount; i++)
            {
                nullBytes[i] = 0;
            }
            
            MemoryStream outputStream = null;
            MemoryStream tempOutputStream = null;
            try
            {
                tempOutputStream = new MemoryStream();
                inputMessage.Data.CopyTo(tempOutputStream);
                tempOutputStream.Write(nullBytes, 0, nullBytesCount);
                tempOutputStream.Position = 0L;

                outputStream = tempOutputStream;
                tempOutputStream = null;
            } 
            finally
            {
                if (tempOutputStream != null) 
                {
                    tempOutputStream.Close();
                }
            }

            OutputStreamMessage outputStreamMessage = new OutputStreamMessage();
            outputStreamMessage.Data = outputStream;
            outputStreamMessage.ServerName = _serverName;
            return outputStreamMessage;
        }
    }
}
