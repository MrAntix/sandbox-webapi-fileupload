using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace Sandbox.WebApi.FileUpload.Web.Uploading
{
    public class UploadMultipartMediaTypeFormatter : JsonMediaTypeFormatter
    {
        readonly Func<Type, HttpContent, IUploadWriterProvider> _getFilePartStreamProvider;

        public UploadMultipartMediaTypeFormatter(
            Func<Type, HttpContent, IUploadWriterProvider> getFilePartStreamProvider)
        {
            _getFilePartStreamProvider = getFilePartStreamProvider;
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("multipart/form-data"));
        }

        public override bool CanReadType(Type type)
        {
            return true;
        }

        public override bool CanWriteType(Type type)
        {
            return false;
        }

        public override async Task<object> ReadFromStreamAsync(
            Type type, Stream readStream, HttpContent content,
            IFormatterLogger formatterLogger)
        {
            var statusManager = new UploadStatusManager(
                GetProgressHeaderValue(content.Headers),
                content.Headers.ContentLength.GetValueOrDefault());

            var filePartStreamProvider = _getFilePartStreamProvider(type, content);

            var streamProvider = new UploadMultipartStreamProvider(
                filePartStreamProvider, 
                statusManager,
                type);

            var contents = await content.ReadAsMultipartAsync(
                streamProvider);

            return contents.GetModel(type);
        }

        static string GetProgressHeaderValue(
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers)
        {
            return headers.Where(h => h.Key == UploadMessageHandler.StatusIdHeaderName)
                          .Select(h => h.Value.FirstOrDefault())
                          .FirstOrDefault();
        }

        public static void Init<T>(
            HttpConfiguration config)
            where T : IUploadWriterProvider, new()
        {
            Init(config, (t, c) => new T());
        }

        public static void Init(
            HttpConfiguration config,
            Func<Type, HttpContent, IUploadWriterProvider> getFilePartStreamProvider)
        {
            config.Formatters.Add(new UploadMultipartMediaTypeFormatter(getFilePartStreamProvider));
            config.MessageHandlers.Add(new UploadMessageHandler());
        }
    }
}