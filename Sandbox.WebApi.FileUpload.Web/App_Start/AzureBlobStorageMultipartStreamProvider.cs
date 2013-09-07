using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Sandbox.WebApi.FileUpload.Web
{
    public class AzureBlobStorageMultipartStreamProvider : MultipartStreamProvider
    {
        readonly CloudBlobContainer _container;
        readonly UploadStatus _uploadStatus;

        public AzureBlobStorageMultipartStreamProvider(
            CloudBlobContainer container, string id, long total)
        {
            _container = container;
            _uploadStatus = new UploadStatus(id, total);
        }

        readonly IDictionary<string, object> _fields = new Dictionary<string, object>();

        public IEnumerable<KeyValuePair<string, object>> GetFields()
        {
            foreach (var name in _fields.Keys)
            {
                var stream = _fields[name];
                object value = null;

                if (stream != null)
                {
                    var azureStream = stream as AzureBlobStorageMultipartStream;
                    if (azureStream != null)
                        value = new AzureBlob
                        {
                            ContentType = azureStream.ContentType,
                            Size = azureStream.Size,
                            Uri = azureStream.Uri.AbsoluteUri
                        };

                    else
                        value = Encoding.UTF8
                                        .GetString(((MemoryStream)stream)
                                                       .ToArray());
                }

                yield return new KeyValuePair<string, object>(name, value);
            }
        }

        public override Stream GetStream(
            HttpContent parent, HttpContentHeaders headers)
        {
            var name = headers.ContentDisposition.Name.Trim('"').ToLower();
            var contentType = headers.ContentType == null
                                  ? null
                                  : headers.ContentType.MediaType.Split('/')[0];

            Stream stream;

            switch (contentType)
            {
                default:

                    stream = new MemoryStream();
                    break;
                case "video":
                case "image":
                    var blobName = Guid.NewGuid().ToString("N");
                    var blobReference = _container.GetBlockBlobReference(blobName);

                    stream = new AzureBlobStorageMultipartStream(blobReference, headers, _uploadStatus);
                    break;
            }

            _fields.Add(name, stream);

            return stream;
        }
    }
}