using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sandbox.WebApi.FileUpload.Web
{
    public class UploadMessageHandler : DelegatingHandler
    {
        public const string StatusIdHeaderName = "x-upload-status-id";
        
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // pass on the header to the formatter
            var sessionIdHeader = request.Headers
                                         .FirstOrDefault(h => h.Key.Equals(StatusIdHeaderName));
            if (sessionIdHeader.Value != null)
                request.Content.Headers.Add(sessionIdHeader.Key, sessionIdHeader.Value);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}