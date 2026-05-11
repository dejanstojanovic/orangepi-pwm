using OrangePi.Common.Models;

namespace OrangePi.Common.Services
{
    public class BuzzerService : IBuzzerService
    {
        public BuzzerService(BuzzerConfig config)
        {
            PinNumber = config.PinNumber;
        }
        public BuzzerService(int pin)
        {
            PinNumber = pin;
        }
        public int PinNumber { get; init; }
        public double Frequency { get; init; }
        public int Duration { get; init; }

        public async Task Play(int frequency, TimeSpan duration)
        {
            using (var buzzer = new Iot.Device.Buzzer.Buzzer(this.PinNumber))
            {
                buzzer.PlayTone(frequency, duration.Milliseconds);
            }
        }

        public void Dispose()
        {
            //Nothing to dispose here
        }
    }
}
