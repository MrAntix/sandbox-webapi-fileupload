namespace Sandbox.WebApi.FileUpload.Web.Uploading
{
    public interface IUploadWriterProvider
    {
        IUploadWriter GetStream(string name, string contentType);
    }
}