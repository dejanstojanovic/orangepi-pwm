namespace OrangePi.Common.Services
{
    public interface IBuzzerService:IDisposable
    {
        int PinNumber { get; init; }
        Task Play(int frequency, TimeSpan lenght);
    }
}
