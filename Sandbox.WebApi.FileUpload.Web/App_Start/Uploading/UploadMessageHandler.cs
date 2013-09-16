using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Sandbox.WebApi.FileUpload.Web.Uploading
{
    public class UploadMessageHandler : DelegatingHandler
    {
        public const string StatusIdHeaderName = "x-upload-status-id";

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // pass on the header to the formatter
            var sessionId = GetSessionIdFromHeader(request.Headers)
                            ?? GetSessionIdFromQueryString(request.RequestUri.Query);
            if (sessionId != null)
                request.Content.Headers.Add(StatusIdHeaderName, sessionId);

            var response = await base.SendAsync(request, cancellationToken);

            // check for accept json, if not send as html
            if (!request.Headers.Accept
                        .Any(h => h.MediaType.Equals("application/json", StringComparison.OrdinalIgnoreCase)))
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

            return response;
        }

        static string GetSessionIdFromHeader(IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers)
        {
            return headers
                .Where(h => h.Key.Equals(StatusIdHeaderName))
                .Select(h => h.Value.First()).FirstOrDefault();
        }

        static string GetSessionIdFromQueryString(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return null;
            return (from p in query.Substring(1).Split('&')
                    let pp = p.Split('=')
                    where pp[0].Equals(StatusIdHeaderName, StringComparison.OrdinalIgnoreCase)
                          && pp.Length > 1
                    select pp[1]).FirstOrDefault();
        }
    }
}