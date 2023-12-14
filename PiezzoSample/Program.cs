using System.Device.Pwm.Drivers;

int gpio = 138;
double soundFrequency = 330;
int pwmFrequency = 100 * 1000;
double pwmDuty = 0.5;

TimeSpan length = TimeSpan.FromSeconds(1);

using (var pwmCahnnel = new SoftwarePwmChannel(gpio, pwmFrequency, pwmDuty, false, null, false))
{
    using (var buzzer = new Iot.Device.Buzzer.Buzzer(pwmCahnnel))
    {
        buzzer.StartPlaying(soundFrequency);
        await Task.Delay(length);
        buzzer.StopPlaying();

    }
}