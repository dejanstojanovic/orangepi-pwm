using System.Device.Pwm;
using System.Device.Pwm.Drivers;

int gpio = 138;
double soundFrequency = 4000;
int pwmFrequency = 400;
double pwmDuty = 0.5;

TimeSpan length = TimeSpan.FromSeconds(0.3);


using (var buzzer = new Iot.Device.Buzzer.Buzzer(gpio))
{
    buzzer.StartPlaying(440);
    Thread.Sleep(1000);
    buzzer.StartPlaying(880);
    Thread.Sleep(1000);
    buzzer.StopPlaying();
}



//using (var pwmCahnnel = new SoftwarePwmChannel(gpio, pwmFrequency, pwmDuty, false, null, false))
//{
//    using (var buzzer = new Iot.Device.Buzzer.Buzzer(pwmCahnnel))
//    {
//        int[] tones = { 25,  28,  31,  35,  40,  45,  50,  56,  63,
//                           70,  80,  90, 100, 112, 125, 140, 160, 180,
//                          200, 225, 250, 285, 320, 355, 400, 450, 500,
//                          565, 635, 715, 800, 900};
//        foreach (var tone in tones)
//        {
//            buzzer.StartPlaying(tone);
//            await Task.Delay(length);
//            buzzer.StopPlaying();
//        }
//        buzzer.StopPlaying();

//        //buzzer.StartPlaying(soundFrequency);
//        //await Task.Delay(length);
//        //buzzer.StopPlaying();

//    }
//}