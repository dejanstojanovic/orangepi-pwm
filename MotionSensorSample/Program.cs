using System.Device.Gpio;
using System.Device.Pwm.Drivers;

using var controller = new GpioController();
var pin = controller.OpenPin(92, PinMode.Input);
while (true)
{
    var value = pin.Read();
    
    if (value == PinValue.High)
    {
        Console.WriteLine("DETECTED!");

    }

    await Task.Delay(TimeSpan.FromMilliseconds(200));
}