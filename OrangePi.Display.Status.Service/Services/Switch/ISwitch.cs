namespace OrangePi.Display.Status.Service.Services.Switch
{
    internal interface ISwitch
    {
        event EventHandler<bool> IsOnChanged;
        bool IsOn { get; }
        Task StartMonitoringAsync(CancellationToken cancellationToken);
    }
}
