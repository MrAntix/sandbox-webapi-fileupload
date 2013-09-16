using System;
using System.IO;

namespace Sandbox.WebApi.FileUpload.Web.Uploading
{
    public class UploadMultipartStream : Stream
    {
        public long Size { get; protected set; }
        public string Uri { get; protected set; }
        public string ContentType { get; protected set; }

        public UploadStatusManager Status { get; internal set; }
        public Func<object> GetModel { get; set; }

        readonly IUploadWriter _writer;

        public UploadMultipartStream(IUploadWriter writer)
        {
            _writer = writer;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _writer.WriteBlock(buffer, offset, count);

            var writeCount = count - offset;
            Status.Increment(writeCount, GetModel());

            Size += writeCount;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            Uri = _writer.GetUrl();
            _writer.Dispose();
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