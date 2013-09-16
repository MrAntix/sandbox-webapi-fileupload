using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Sandbox.WebApi.FileUpload.Web.Uploading.Azure
{
    public class AzureBlobStorageUploadWriter : IUploadWriter
    {
        readonly CloudBlockBlob _blobReference;
        readonly string _contentType;
        readonly List<string> _blockList;

        public AzureBlobStorageUploadWriter(
            CloudBlockBlob blobReference,
            string contentType)
        {
            _blobReference = blobReference;
            _contentType = contentType;

            _blockList = new List<string>();
        }

        public void WriteBlock(byte[] buffer, int offset, int count)
        {
            var blockId = Convert
                .ToBase64String(Guid.NewGuid().ToByteArray());

            using (var ms = new MemoryStream(buffer, offset, count))
            {
                _blobReference.PutBlock(blockId, ms, null);
            }

            _blockList.Add(blockId);
        }

        public string GetUrl()
        {
            return _blobReference.Uri.AbsoluteUri;
        }

        public void Dispose()
        {
            if (!_blockList.Any()) return;

            if (_contentType != null)
                _blobReference.Properties.ContentType = _contentType;

            _blobReference.PutBlockList(_blockList);

            _blockList.Clear();
        }
    }
}