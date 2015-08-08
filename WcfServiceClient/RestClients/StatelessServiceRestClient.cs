using System;
using System.Configuration;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Threading.Tasks;
using WcfServiceLibrary;

namespace WcfServiceClient.RestClients
{
    public class StatelessServiceRestClient : IStatelessService, IDisposable
    {
        private readonly WebChannelFactory<IStatelessService> _webChannelFactory;
        private readonly IStatelessService _statelessService;

        public StatelessServiceRestClient(Uri endpointUri)
        {
            int maxReceivedMessageSize = Int32.Parse(ConfigurationManager.AppSettings["MaxReceivedMessageSize"]);
            WebHttpBinding webHttpBinding = new WebHttpBinding { MaxReceivedMessageSize = maxReceivedMessageSize };
            _webChannelFactory = new WebChannelFactory<IStatelessService>(
                webHttpBinding,
                endpointUri
            );

            IEndpointBehavior endpointBehavior = new WebHttpBehavior();
            _webChannelFactory.Endpoint.EndpointBehaviors.Add(endpointBehavior);
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
