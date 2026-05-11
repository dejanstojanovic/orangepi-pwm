using OrangePi.Display.Status.Service.Models;

namespace OrangePi.Display.Status.Service.Services.Info
{
    public interface IInfoService : IDisplayInfoService
    {
        Task<StatusValue> GetValue();
        string Label { get; }
    }
}
