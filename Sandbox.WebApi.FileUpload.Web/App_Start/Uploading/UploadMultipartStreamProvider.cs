using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Sandbox.WebApi.FileUpload.Web.Uploading
{
    public class UploadMultipartStreamProvider : MultipartStreamProvider
    {
        public UploadMultipartStreamProvider(
            IUploadWriterProvider writerProvider,
            UploadStatusManager status,
            Type modelType)
        {
            _writerProvider = writerProvider;
            _status = status;
            _modelType = modelType;
        }

        readonly IDictionary<string, object> _fields = new Dictionary<string, object>();
        readonly IUploadWriterProvider _writerProvider;
        readonly UploadStatusManager _status;
        readonly Type _modelType;

        public object GetModel(Type type)
        {
            var model = new DynamicJObject();

            foreach (var field in GetFields())
            {
                model.Set(field.Key, field.Value);
            }

            return model.Get(string.Empty, type);
        }

        public IEnumerable<KeyValuePair<string, object>> GetFields()
        {
            foreach (var name in _fields.Keys)
            {
                var stream = _fields[name];
                object value = null;

                if (stream != null)
                {
                    var azureStream = stream as UploadMultipartStream;
                    if (azureStream != null)
                        value = new UploadFile
                            {
                                ContentType = azureStream.ContentType,
                                Size = azureStream.Size,
                                Uri = azureStream.Uri
                            };

                    else
                        value = Encoding.UTF8
                                        .GetString(((MemoryStream) stream)
                                                       .ToArray());
                }

                yield return new KeyValuePair<string, object>(name, value);
            }
        }

        public override Stream GetStream(
            HttpContent parent, HttpContentHeaders headers)
        {
            var fieldName = headers.ContentDisposition.Name.Trim('"').ToLower();
            var fileName = headers.ContentDisposition.FileName == null
                               ? null
                               : headers.ContentDisposition.FileName.Trim('"');

            string contentType = null;
            string streamType = null;
            if (headers.ContentType != null)
            {
                contentType = headers.ContentType.MediaType;
                streamType = contentType == null
                                 ? null
                                 : contentType.Split('/')[0];
            }

            Stream stream;
            switch (streamType)
            {
                default:

                    stream = new MemoryStream();
                    break;
                case "video":
                case "audio":
                case "image":

                    var uploadStream =
                        new UploadMultipartStream(_writerProvider.GetStream(fileName, contentType))
                            {
                                Status = _status,
                                GetModel = () => GetModel(_modelType)
                            };

                    stream = uploadStream;

                    break;
            }

            _fields.Add(fieldName, stream);

            return stream;
        }
    }
}