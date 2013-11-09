using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;

using Sandbox.WebApi.FileUpload.Web.Models;
using Sandbox.WebApi.FileUpload.Web.Uploading;

namespace Sandbox.WebApi.FileUpload.Web.Controllers
{
    public class UploadController : ApiController
    {
        public async Task<HttpResponseMessage> Post(UploadModel model)
        {
            var response = Request.CreateResponse();
            response.Content = new ObjectContent<UploadModel>(model, new JsonMediaTypeFormatter());

            return response;
        }

        public async Task<UploadStatus<UploadModel>> Get(string id)
        {
            return UploadStatusManager.GetStatus<UploadModel>(id);
        }
    }
}