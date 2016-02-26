using System;
using System.Configuration;
using System.IO;
using System.ServiceModel;

namespace WcfServiceLibrary
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, AddressFilterMode = AddressFilterMode.Any)]
    public class StatelessService : IStatelessService
    {
        private readonly int _serverId;

        public StatelessService(int serverId)
        {
            _serverId = serverId;
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

            RegisteredUserInfo registeredUserInfo = new RegisteredUserInfo
            {
                UserInfo = userInfo,
                ServerId = _serverId
            };
            return registeredUserInfo;
        }

        public UploadedFileInfo UploadFile(Stream fileStream)
        {
            if (fileStream == null)
            {
                throw new ArgumentNullException("fileStream");
            }

            UploadedFileInfo uploadedFileInfo = new UploadedFileInfo {ServerId = _serverId};
            string uploadedFilePath = ConfigurationManager.AppSettings["UploadedFilePath"];
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
            string fileForDownloadingPath = ConfigurationManager.AppSettings["FileForDownloadingPath"];
            FileStream downloadingFileStream = File.Open(fileForDownloadingPath, FileMode.Open);
            string tmpFileForDownloadingPath = ConfigurationManager.AppSettings["TmpFileForDownloadingPath"];
            FileStream tmpFileStream = File.Create(tmpFileForDownloadingPath);

            byte[] valueBytes = BitConverter.GetBytes(_serverId);
            tmpFileStream.Write(valueBytes, 0, valueBytes.Length);
            downloadingFileStream.CopyTo(tmpFileStream);
            tmpFileStream.Position = 0;

            OperationContext operationContext = OperationContext.Current;
            if (operationContext != null)
            {
                operationContext.OperationCompleted += delegate
                {
                    downloadingFileStream.Dispose();
                    tmpFileStream.Dispose();
                    File.Delete(tmpFileForDownloadingPath);
                };
            }

            return tmpFileStream;
        }
    }
}
