using System.Web;
using System.Web.Mvc;

namespace Sandbox.WebApi.FileUpload.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}