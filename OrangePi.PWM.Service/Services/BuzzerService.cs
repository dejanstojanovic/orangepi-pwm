using System.Device.Pwm;
using System.Device.Pwm.Drivers;

namespace OrangePi.PWM.Service.Services
{
    public class BuzzerService : IBuzzerService
    {
        readonly PwmChannel _buzzerPwmChannel;
        public BuzzerService(int gpio)
        {
            GPIO = gpio;
            _buzzerPwmChannel = new SoftwarePwmChannel(gpio, 400, 0.5, false, null, false);
        }
        public int GPIO { get; init; }

        public async Task Play(int frequency, TimeSpan lenght)
        {
            using (var buzzer = new Iot.Device.Buzzer.Buzzer(_buzzerPwmChannel))
            {
                buzzer.StartPlaying(frequency);
                await Task.Delay(lenght);
                buzzer.StopPlaying();
            }
        }

        public async Task Play(int frequency, TimeSpan lenght, TimeSpan pause, int times)
        {
            using (var buzzer = new Iot.Device.Buzzer.Buzzer(_buzzerPwmChannel))
            {
                for (int i = 0; i < times; i++)
                {
                    buzzer.StartPlaying(frequency);
                    await Task.Delay(lenght);
                    buzzer.StopPlaying();
                    await Task.Delay(pause);
                }
            }
        }

        public void Dispose()
        {
            _buzzerPwmChannel.Dispose();
        }
    }
}
