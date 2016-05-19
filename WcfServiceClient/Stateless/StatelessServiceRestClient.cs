using System;
using System.IO;
using System.ServiceModel.Web;
using System.Threading.Tasks;
using WcfServiceLibrary;

namespace WcfServiceClient.RestClients
{
    public class StatelessServiceRestClient : IStatelessService, IDisposable
    {
        private readonly WebChannelFactory<IStatelessService> _webChannelFactory;
        private readonly IStatelessService _statelessService;

        public StatelessServiceRestClient(string enpointConfigurationName)
        {
            _webChannelFactory = new WebChannelFactory<IStatelessService>(enpointConfigurationName);
            _statelessService = _webChannelFactory.CreateChannel();
        }

        public int GetServerId()
        {
            return _statelessService.GetServerId();
        }

        public Task<int> GetServerIdAsync()
        {
            throw new NotImplementedException();
        }

        public RegisteredUserInfo RegisterUser(UserInfo userInfo)
        {
            return _statelessService.RegisterUser(userInfo);
        }

        public Task<RegisteredUserInfo> RegisterUserAsync(UserInfo userInfo)
        {
            throw new NotImplementedException();
        }

        public UploadedFileInfo UploadFile(Stream fileStream)
        {
            return _statelessService.UploadFile(fileStream);
        }

        public Task<UploadedFileInfo> UploadFileAsync(Stream fileStream)
        {
            throw new NotImplementedException();
        }

        public Stream DownloadFile()
        {
            return _statelessService.DownloadFile();
        }

        public Task<Stream> DownloadFileAsync()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_webChannelFactory != null)
                {
                    ((IDisposable)_webChannelFactory).Dispose();
                }
            }
        }
    }
}
