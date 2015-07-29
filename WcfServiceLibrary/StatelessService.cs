using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WcfServiceLibrary
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, AddressFilterMode = AddressFilterMode.Any)]
    public class StatelessService : IStatelessService
    {
        private int _serverId;
        private string _executableDirPath;

        public StatelessService(int serverId)
        {
            _serverId = serverId;
            _executableDirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public int GetServerId()
        {
            return _serverId;
        }

        public RegisteredUserInfo RegisterUser(UserInfo userInfo)
        {
            if (userInfo == null)
            {
                throw new ArgumentNullException("userInfo");
            }

            RegisteredUserInfo registeredUserInfo = new RegisteredUserInfo();
            registeredUserInfo.UserInfo = userInfo;
            registeredUserInfo.ServerId = _serverId;
            return registeredUserInfo;
        }

        public UploadedFileInfo UploadFile(Stream fileStream)
        {
            if (fileStream == null)
            {
                throw new ArgumentNullException("fileStream");
            }

            UploadedFileInfo uploadedFileInfo = new UploadedFileInfo();
            uploadedFileInfo.ServerId = _serverId;

            string uploadedFilePath = Path.Combine(_executableDirPath, "Resources\\UploadedFile.jpg");
            using (FileStream uploadedFileStream = File.Create(uploadedFilePath))
            {
                using (fileStream)
                {
                    fileStream.CopyTo(uploadedFileStream);
                    uploadedFileInfo.FileSize = uploadedFileStream.Length;
                }
            }
            File.Delete(uploadedFilePath);
            return uploadedFileInfo;
        }

        public Stream DownloadFile()
        {
            string downloadingFilePath = Path.Combine(_executableDirPath, "Resources\\FileForDownloading.jpg");
            FileStream downloadingFileStream = File.Open(downloadingFilePath, FileMode.Open);
            string tmpFilePath = Path.Combine(_executableDirPath, "Resources\\TMP.jpg");
            FileStream tmpFileStream = File.Create(tmpFilePath);

            byte[] valueBytes = BitConverter.GetBytes(_serverId);
            tmpFileStream.Write(valueBytes, 0, valueBytes.Length);
            downloadingFileStream.CopyTo(tmpFileStream);
            tmpFileStream.Position = 0;

            OperationContext operationContext = OperationContext.Current;
            if (operationContext != null)
            {
                operationContext.OperationCompleted += new EventHandler(delegate(object sender, EventArgs args)
                {
                    if (downloadingFileStream != null)
                    {
                        downloadingFileStream.Dispose();
                    }
                    if (tmpFileStream != null)
                    {
                        tmpFileStream.Dispose();
                    }
                    File.Delete(tmpFilePath);
                });
            }

            return tmpFileStream;
        }
    }
}
