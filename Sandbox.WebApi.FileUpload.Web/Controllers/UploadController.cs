using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web.Http;
using Sandbox.WebApi.FileUpload.Web.Models;

namespace Sandbox.WebApi.FileUpload.Web.Controllers
{
    public class UploadController : ApiController
    {
        public async Task<UploadModel> Post(UploadModel model)
        {
            
            return model;
        }

        public async Task<int> Get(string id)
        {
            var status = MemoryCache.Default.Get(id);

            if (status == null) return 100;

            return (int) status;
        }
    }
}