using System.Device.Gpio;
using System.Device.I2c;
using System.Device.Spi;
using System.Drawing;
using System.Drawing.Imaging;
using Iot.Device.Board;

namespace OrangePi.Display.Status.Service
{

    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        public Worker(
            ILogger<Worker> logger
            )
        {
            _logger = logger;

            //System.Device.Pwm.PwmChannel buzzerPwm = new SoftwarePwmChannel(12, 400, 0.5, false, null, false);
            //var buzzer = new Iot.Device.Buzzer.Buzzer(buzzerPwm);
            //buzzer.StartPlaying(110);
            //Task.Delay(1000).Wait();
            //buzzer.StopPlaying();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var i2cDevice = I2cDevice.Create(new I2cConnectionSettings(1, 1));
            using var lcd = new Iot.Device.Ssd13xx.Ssd1306(i2cDevice,128,64);

            lcd.Font = new DoubleByteFont();
            lcd.DrawString(2, 2, "TEST1", 2, false);
            lcd.DrawString(2, 34, "TEST2", 1, true);
            lcd.Display();

            //var bitmap = Iot.Device.Graphics.BitmapImage.CreateBitmap(128, 64, Iot.Device.Graphics.PixelFormat.Format16bppRgb565);
            //var draw = bitmap.GetDrawingApi();

            //lcd.ClearScreen();
            //lcd.DrawBitmap

            //++++++++++++++++++++++++++++++++++++++++
            //using SpiDevice device = SpiDevice.Create(new SpiConnectionSettings(busId: 1));//vidi sta je ovo

            var spiSettings = new SpiConnectionSettings(busId: 1,chipSelectLine:7);
            spiSettings.DataBitLength = 8;
            spiSettings.ClockFrequency = 30000;
            spiSettings.Mode = SpiMode.Mode0;
            spiSettings.DataFlow = DataFlow.MsbFirst;


            using var board = Board.Create();
            var device = board.CreateSpiDevice(spiSettings, new int[] { 3, 4 }, PinNumberingScheme.Logical);

            var spi = device;

            //using var spi = new SoftwareSpi(
            //    clk: 3, //SPI CLOCK (SCL)
            //    sdi: -1,
            //    sdo: 4, //SPI DATA (SDA)
            //    cs: 7, //ENABLE SIGNAL (CS)
            //    settings: spiSettings,
            //    gpioController: new GpioController(numberingScheme: PinNumberingScheme.Logical));

            // PINS
            // https://www.robotics.org.za/IPS-154-ST7789-SPI
            // https://www.waveshare.com/wiki/1.14inch_LCD_Module
            // https://techatronic.com/st7789-raspberry-pi/

            //https://github.com/dotnet/iot/blob/main/src/devices/CharacterLcd/samples/Program.cs

            //using var lcd = new Lcd2004(registerSelectPin: 0,
            //                        enablePin: 7,
            //                        dataPins: new int[] { 6 },
            //                        backlightPin: 8,
            //                        backlightBrightness: 0.1f,
            //                        readWritePin: 4,
            //                        controller: new GpioController(PinNumberingScheme.Logical, driver));

            //using var lcd = new LcdRgb(size: new System.Drawing.Size { Width = 240, Height = 135 },
            //    lcdInterface: LcdInterface.CreateGpio(
            //    registerSelectPin: 6,
            //    enablePin:7,
            //    dataPins: new int[] { 4 },
            //    //backlightPin:8,
            //    backlightBrightness:8),
            //    rgbDevice: device);

            //int currentLine = 0;



            var bitmap = new Bitmap(100, 100);
            var graph = Graphics.FromImage(bitmap);

            graph.FillRectangle(new SolidBrush(Color.FromArgb(alpha: 255, red: 0, green: 255, blue: 0)), new Rectangle
            {
                X = 5,
                Y = 5,
                Width = 100,
                Height = 100
            });
            graph.Flush();

            using (var memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, ImageFormat.Bmp);
                spi.Write(memoryStream.ToArray());
            }




            //while (!stoppingToken.IsCancellationRequested)
            //{

            //}


            //while (!stoppingToken.IsCancellationRequested)
            //{

            //}

        }
    }
}