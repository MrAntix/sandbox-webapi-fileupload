using System;
using System.Runtime.Caching;

namespace Sandbox.WebApi.FileUpload.Web.Uploading
{
    public class UploadStatusManager
    {
        readonly string _id;
        readonly long _total;
        long _count;

        static readonly object LockObject = new object();

        public UploadStatusManager(string id, long total)
        {
            _total = total;
            _id = id;
        }

        public void Increment(long count, object model)
        {
            lock (LockObject)
            {
                _count += count;
                var percent = (int) Math.Round(100*_count/(decimal) _total);

                var statusType = typeof (UploadStatus<>)
                    .MakeGenericType(model.GetType());

                var status = Activator
                    .CreateInstance(statusType, _count, _total, percent, model);

                if (_id != null)
                {
                    MemoryCache.Default
                               .Set(_id, status, DateTimeOffset.UtcNow.AddHours(1));
                }
            }
        }

        public static UploadStatus<T> GetStatus<T>(string id)
        {
            return (UploadStatus<T>) MemoryCache.Default.Get(id);
        }
    }
}