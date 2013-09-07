using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Sandbox.WebApi.FileUpload.Web
{
    public class AzureBlobStorageMultipartMediaTypeFormatter : MediaTypeFormatter
    {
        public AzureBlobStorageMultipartMediaTypeFormatter()
        {
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
            var statusId = GetProgressHeaderValue(content.Headers);

            var container = await GetWebApiContainer();
            var streamProvider = new AzureBlobStorageMultipartStreamProvider(
                container,
                statusId,
                content.Headers.ContentLength.GetValueOrDefault());

            var contents = await content.ReadAsMultipartAsync(
                streamProvider);

            var fields = contents.GetFields();

            var model = new DynamicJObject();

            foreach (var field in fields)
            {
                model.Set(field.Key, field.Value);
            }

            return model.Get(string.Empty, type);
        }

        static string GetProgressHeaderValue(
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers)
        {
            return headers.Where(h => h.Key == UploadMessageHandler.StatusIdHeaderName)
                          .Select(h => h.Value.FirstOrDefault())
                          .FirstOrDefault();
        }

        static async Task<CloudBlobContainer> GetWebApiContainer()
        {
            // Retrieve storage account from connection-string
            var storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true");

            // Create the blob client 
            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference("fileuploadcontainer");

            // Create the container if it doesn't already exist
            await container.CreateIfNotExistsAsync();
            await container.SetPermissionsAsync(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });

            return container;
        }
    }
}