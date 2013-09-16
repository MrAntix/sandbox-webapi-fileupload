namespace Sandbox.WebApi.FileUpload.Web.Uploading
{
    public class UploadStatus<T>
    {
        readonly long _count;
        readonly long _total;
        readonly int _percent;
        readonly T _model;

        public UploadStatus(long count, long total, int percent, T model)
        {
            _count = count;
            _total = total;
            _percent = percent;
            _model = model;
        }

        public long Count
        {
            get { return _count; }
        }

        public long Total
        {
            get { return _total; }
        }

        public int Percent
        {
            get { return _percent; }
        }

        public T Model
        {
            get { return _model; }
        }
    }
}