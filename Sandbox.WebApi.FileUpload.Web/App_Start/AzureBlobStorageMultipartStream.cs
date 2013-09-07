using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Sandbox.WebApi.FileUpload.Web
{
    public class AzureBlobStorageMultipartStream : Stream
    {
        readonly CloudBlockBlob _blobReference;
        readonly HttpContentHeaders _headers;
        readonly List<string> _blockList;
        readonly UploadStatus _status;

        public AzureBlobStorageMultipartStream(
            CloudBlockBlob blobReference,
            HttpContentHeaders headers,
            UploadStatus status)
        {
            _blobReference = blobReference;
            _headers = headers;
            _status = status;
            _blockList = new List<string>();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            var blockId = Convert
                .ToBase64String(Guid.NewGuid().ToByteArray());

            using (var ms = new MemoryStream(buffer, offset, count))
            {
                _blobReference.PutBlock(blockId, ms, null);
            }

            var writeCount = count - offset;
            _status.Increment(writeCount);

            Size += writeCount;

            _blockList.Add(blockId);
        }

        public long Size { get; private set; }

        public string ContentType
        {
            get { return _headers.ContentType.MediaType; }
        }

        public Uri Uri
        {
            get { return _blobReference.Uri; }
        }

        protected override void Dispose(bool disposing)
        {
            if (!_blockList.Any()) return;

            if (_headers.ContentType != null)
                _blobReference.Properties.ContentType = _headers.ContentType.MediaType;

            _blobReference.PutBlockList(_blockList);

            _blockList.Clear();
        }

        #region unimplemented

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead
        {
            get { throw new NotImplementedException(); }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        public override long Position { get; set; }

        #endregion
    }
}