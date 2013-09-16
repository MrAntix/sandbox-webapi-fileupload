using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Sandbox.WebApi.FileUpload.Web.Properties;

namespace Sandbox.WebApi.FileUpload.Web.Uploading.Azure
{
    public class AzureBlobStorageUploadWriterProvider :
        IUploadWriterProvider
    {
        CloudBlobContainer _container;

        public IUploadWriter GetStream(string name, string contentType)
        {
            if (_container == null)
                _container = GetWebApiContainer();

            var blobName = Guid.NewGuid().ToString("N");
            var blobReference = _container.GetBlockBlobReference(blobName);

            return new AzureBlobStorageUploadWriter(blobReference, contentType);
        }

        static CloudBlobContainer GetWebApiContainer()
        {
            // Retrieve storage account from connection-string
            var storageAccount = CloudStorageAccount.Parse(Settings.Default.AzureBlobStorageUpload_ConnectionString);

            // Create the blob client 
            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference(Settings.Default.AzureBlobStorageUpload_ContainerName);

            // Create the container if it doesn't already exist
            container.CreateIfNotExists();
            container.SetPermissions(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });

            return container;
        }
    }
}