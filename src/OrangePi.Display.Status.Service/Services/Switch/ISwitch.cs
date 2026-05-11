namespace OrangePi.Display.Status.Service.Services.Switch
{
    public interface ISwitch
    {
        event EventHandler<bool> Changed;
        bool IsOn { get; }
        Task StartMonitoringAsync(CancellationToken cancellationToken);
    }
}
