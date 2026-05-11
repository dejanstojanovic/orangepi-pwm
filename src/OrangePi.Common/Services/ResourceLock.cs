namespace OrangePi.Common.Services
{
    public abstract class ResourceLock : IResourceLock
    {
        protected abstract string LockFilePath { get; }

        private FileStream? _lockFile;
        private bool _isLocked = false;

        public bool IsInUse()
        {
            try
            {
                using var probe = new FileStream(
                    LockFilePath,
                    FileMode.OpenOrCreate,
                    FileAccess.ReadWrite,
                    FileShare.None
                );
                return false;
            }
            catch (IOException)
            {
                return true;
            }
        }

        public void Acquire()
        {
            _lockFile = new FileStream(
                LockFilePath,
                FileMode.OpenOrCreate,
                FileAccess.ReadWrite,
                FileShare.None
            );
            _isLocked = true;
        }

        public bool TryAcquire()
        {
            try
            {
                _lockFile = new FileStream(
                    LockFilePath,
                    FileMode.OpenOrCreate,
                    FileAccess.ReadWrite,
                    FileShare.None
                );
                _isLocked = true;
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }

        public bool TryAcquire(TimeSpan timeout)
        {
            var deadline = DateTime.UtcNow + timeout;

            while (DateTime.UtcNow < deadline)
            {
                if (TryAcquire())
                    return true;

                Thread.Sleep(50);
            }

            return false;
        }

        public void Release()
        {
            if (_isLocked && _lockFile != null)
            {
                _lockFile.Dispose();
                _lockFile = null;
                _isLocked = false;
            }
        }

        public void Dispose() => Release();
    }
}
