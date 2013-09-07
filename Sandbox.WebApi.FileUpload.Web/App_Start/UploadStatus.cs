using System;
using System.Runtime.Caching;

namespace Sandbox.WebApi.FileUpload.Web
{
    public class UploadStatus
    {
        readonly string _id;
        readonly long _total;
        long _position;

        static readonly object LockObject = new object();
        int _progress;

        public UploadStatus(string id, long total)
        {
            _total = total;
            _id = id;
        }

        public void Increment(long count)
        {
            lock (LockObject)
            {
                _position += count;
                _progress = (int) Math.Round(100*_position/(decimal) _total);

                if (_id != null)
                {
                    MemoryCache.Default
                               .Set(_id, _progress, DateTimeOffset.UtcNow.AddHours(1));
                }
            }
        }

        public int Progress
        {
            get { return _progress; }
        }
    }
}