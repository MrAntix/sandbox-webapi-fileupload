using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sandbox.WebApi.FileUpload.Web.Models
{
    public class UploadModel
    {
        public IEnumerable<FileModel> Files { get; set; }
    }

    public class FileModel
    {
        [Required]
        public string Caption { get; set; }
        public MediaModel Media { get; set; }
    }

    public class MediaModel
    {
        public int Size { get; set; }
        public string ContentType { get; set; }
        public string Uri { get; set; }
    }
}