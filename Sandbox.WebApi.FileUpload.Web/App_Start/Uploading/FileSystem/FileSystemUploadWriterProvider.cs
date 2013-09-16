using System;
using System.IO;
using Sandbox.WebApi.FileUpload.Web.Properties;

namespace Sandbox.WebApi.FileUpload.Web.Uploading.FileSystem
{
    public class FileSystemUploadWriterProvider :
        IUploadWriterProvider
    {
        readonly string _root;
        readonly string _rootUrl;

        public FileSystemUploadWriterProvider(
            string root, string rootUrl)
        {
            _root = root;
            _rootUrl = rootUrl;
        }

        public IUploadWriter GetStream(string fileName, string contentType)
        {
            var name = string.Concat(
                Guid.NewGuid().ToString(), 
                Path.GetExtension(fileName)
                );

            var path = string.Format(
                Settings.Default.FileSystemUpload_PathTemplate, _root, name);
            var uri = string.Format(
                Settings.Default.FileSystemUpload_UriTemplate, _rootUrl, name);

            return new FileSystemUploadWriter(path, uri);
        }
    }
}