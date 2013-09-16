using System.IO;

namespace Sandbox.WebApi.FileUpload.Web.Uploading.FileSystem
{
    public class FileSystemUploadWriter :
        IUploadWriter
    {
        readonly Stream _file;
        readonly string _url;

        public FileSystemUploadWriter(string path, string url)
        {
            _file = File.OpenWrite(path);
            _url = url;
        }

        public void WriteBlock(byte[] buffer, int offset, int count)
        {
            _file.Write(buffer, offset, count);
        }

        public string GetUrl()
        {
            return _url;
        }

        public void Dispose()
        {
            _file.Dispose();
        }
    }
}