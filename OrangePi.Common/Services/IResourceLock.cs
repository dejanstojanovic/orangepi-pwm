namespace OrangePi.Common.Services
{
    public interface IResourceLock: IDisposable
    {
        void Acquire();
        bool TryAcquire();
        bool TryAcquire(TimeSpan timeout);
        void Release();
        bool IsInUse();
    }
}
