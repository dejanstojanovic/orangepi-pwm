using System.Device.Pwm;
using System.Device.Pwm.Drivers;

namespace OrangePi.Common.Services
{
    public class BuzzerService : IBuzzerService
    {

        public BuzzerService(int gpio,int pwmFrequency, double pwmDuty)
        {
            GPIO = gpio;
            PWMFrequency= pwmFrequency;
            PWMDuty = pwmDuty;
        }
        public int GPIO { get; init; }
        public int PWMFrequency { get; init; }
        public double PWMDuty { get; init; }

        public async Task Play(int frequency, TimeSpan lenght)
        {
            using (var pwmCahnnel = new SoftwarePwmChannel(this.GPIO, this.PWMFrequency, this.PWMDuty, false, null, false))
            {
                using (var buzzer = new Iot.Device.Buzzer.Buzzer(pwmCahnnel))
                {
                    buzzer.StartPlaying(frequency);
                    await Task.Delay(lenght);
                    buzzer.StopPlaying();
                }
            }
        }

        public async Task Play(int frequency, TimeSpan lenght, TimeSpan pause, int times)
        {
            using (var pwmCahnnel = new SoftwarePwmChannel(GPIO, PWMFrequency, PWMDuty, false, null, false))
            {
                using (var buzzer = new Iot.Device.Buzzer.Buzzer(pwmCahnnel))
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
        }

        public void Dispose()
        {

        }
    }
}
