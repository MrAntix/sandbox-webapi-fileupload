using System;

namespace Sandbox.WebApi.FileUpload.Web.Uploading
{
    public interface IUploadWriter : IDisposable
    {
        void WriteBlock(byte[] buffer, int offset, int count);
        string GetUrl();
    }
}