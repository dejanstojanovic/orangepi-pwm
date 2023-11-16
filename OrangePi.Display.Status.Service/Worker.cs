using Microsoft.Extensions.Options;
using System.Device.Gpio;
using System.Device.I2c;
using System.Threading;
using Iot.Device.CharacterLcd;
using Iot.Device.Pcx857x;
using System.Device.Spi;
using Iot.Device.Spi;
using System.Drawing;
using System.Drawing.Imaging;
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

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //using SpiDevice device = SpiDevice.Create(new SpiConnectionSettings(busId: 1));//vidi sta je ovo

            using var spi = new SoftwareSpi(
                clk: 3, //SPI CLOCK (SCL)
                sdi: -1,
                sdo: 4, //SPI DATA (SDA)
                cs: 7, //ENABLE SIGNAL (CS)
                settings: null,
                gpioController: new GpioController(numberingScheme:PinNumberingScheme.Logical)
                );

            // PINS
            // https://www.robotics.org.za/IPS-154-ST7789-SPI
            // https://www.waveshare.com/wiki/1.14inch_LCD_Module
            // https://techatronic.com/st7789-raspberry-pi/

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
                bitmap.Save(memoryStream, ImageFormat.Jpeg);
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