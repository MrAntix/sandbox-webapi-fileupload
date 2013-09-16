using System;
using System.Web;
using System.Web.Http;
using Sandbox.WebApi.FileUpload.Web.Uploading;
using Sandbox.WebApi.FileUpload.Web.Uploading.Azure;
using Sandbox.WebApi.FileUpload.Web.Uploading.FileSystem;

namespace Sandbox.WebApi.FileUpload.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new {id = RouteParameter.Optional}
                );

            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();

            UploadMultipartMediaTypeFormatter
                .Init(config,
                      (t, c) => new FileSystemUploadWriterProvider(
                                    HttpContext.Current.Server.MapPath("~/"),
                                    HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/"
                                    ));

            //UploadMultipartMediaTypeFormatter
            //    .Init<AzureBlobStorageUploadWriterProvider>(config);
        }
    }
}