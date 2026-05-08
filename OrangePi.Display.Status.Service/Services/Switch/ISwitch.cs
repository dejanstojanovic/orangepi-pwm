namespace OrangePi.Display.Status.Service.Services.Switch
{
    internal interface ISwitch
    {
        bool IsOn { get; }
        Task StartMonitoringAsync(CancellationToken cancellationToken);
    }
}
