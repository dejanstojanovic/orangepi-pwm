using Iot.Device.Vl53L1X;
using System.Device.I2c;

var _i2cSettings = new I2cConnectionSettings(3, Vl53L1X.DefaultI2cAddress);
using (var _i2cDevice = I2cDevice.Create(_i2cSettings))
{
    using (var _distanceSensor = new Vl53L1X(_i2cDevice))
    {
        while (true)
        {
            var distance = _distanceSensor.GetDistance();
            Console.WriteLine($"Distance: {distance.Millimeters}mm");
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
}