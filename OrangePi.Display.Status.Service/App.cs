using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Displays;
using Meadow.Foundation.Graphics;

namespace OrangePi.Display.Status.Service
{
    public class App : App<F7FeatherV1>
    {
        St7789 display;
        const int displayWidth = 240;
        const int displayHeight = 135;

        public override Task Run()
        {
            var spiBus = Device.CreateSpiBus(Device.Pins.SCK, Device.Pins.MOSI, Device.Pins.MISO);

            display = new St7789(
                spiBus: spiBus,
                chipSelectPin: null,
                dcPin: Device.Pins.D01,
                resetPin: Device.Pins.D00,
                width: displayWidth, height: displayHeight);
            return base.Run();
        }

        public override Task Initialize()
        {
            var graphics = new MicroGraphics(display);

            graphics.DrawText(10, 10, "Hello from Pi", ScaleFactor.X2);

            return base.Initialize();
        }
    }
}