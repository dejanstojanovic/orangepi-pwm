// GPIO Pin
using Iot.Device.Common;
using Iot.Device.DHTxx;
using UnitsNet;

using (Dht11 dht = new Dht11(257))
{
    //dht.MinTimeBetweenReads =  TimeSpan.FromSeconds(1);
    while (true)
    {
        

        Temperature temperature = default;
        RelativeHumidity humidity = default;
        bool success = dht.TryReadTemperature(out temperature); //&& dht.TryReadTemperature(out temperature);
        // You can only display temperature and humidity if the read is successful otherwise, this will raise an exception as
        // both temperature and humidity are NAN
        if (success)
        {
            Console.WriteLine($"Temperature: {temperature.DegreesCelsius:F1}\u00B0C");//, Relative humidity: {humidity.Percent:F1}%");

            //// WeatherHelper supports more calculations, such as saturated vapor pressure, actual vapor pressure and absolute humidity.
            //Console.WriteLine(
            //    $"Heat index: {WeatherHelper.CalculateHeatIndex(temperature, humidity).DegreesCelsius:F1}\u00B0C");
            //Console.WriteLine(
            //    $"Dew point: {WeatherHelper.CalculateDewPoint(temperature, humidity).DegreesCelsius:F1}\u00B0C");
        }
        else
        {
            Console.WriteLine("Error reading DHT sensor");
        }
        await Task.Delay(TimeSpan.FromSeconds(5));
    }
}