using System.CodeDom;
using System.Device.Gpio;
using System.Device.Pwm.Drivers;

using var controller = new GpioController();
var pin = controller.OpenPin(52, PinMode.Input);
const int fins = 7;
var hits = new List<bool>();
var locker = new Object();

System.Timers.Timer timer = new System.Timers.Timer(TimeSpan.FromSeconds(1));
timer.Elapsed += timer_Elapsed;

void timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
{
    lock (locker)
    {
        var result = hits.Where((e, i) => i < hits.Count - 1)
                .Select((e, i) => new { A = e, B = hits[i + 1] })
                .Where((c, i) => i % 2 == 0)
                .Where(e => e.A != e.B)
                .Count();

        hits.Clear();

        Console.Clear();
        Console.WriteLine($"rpm: {(result * 60)}");

        timer.Stop();
        timer.Start();
    }

}

timer.Start();
while (true)
{
    var value = pin.Read();

    if (value == PinValue.High)
    {
        lock (locker)
        {
            hits.Add(true);
        }
    }
    else
    {
        lock (locker)
        {
            hits.Add(false);
        }
    }

    await Task.Delay(TimeSpan.FromMilliseconds(10));
}