using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OrangePi.PWM
{
    class ThresholdRange
    {
        public ThresholdRange(double start, double end, double value)
        {
            this.Start = start;
            this.End = end;
            this.Value = value;
        }
        public double Start { get; init; }
        public double End { get; init; }
        public double Value { get; init; }
    }

    public class Program
    {
        static async Task Main(string[] args)
        {
            const int WPI = 2;
            List<(double Temperature, int Value)> thresholds = new List<(double Temperature, int Value)> {
                (30, 100), (40, 300), (50, 400), (60, 500), (70, 800), (80, 1000)
            };

            async Task<string> RunProcessAsync(string command, params string[] args)
            {
                using (Process process = new Process())
                {
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.FileName = command;
                    foreach (var arg in args)
                    {
                        process.StartInfo.ArgumentList.Add(arg);
                    }

                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    await process.WaitForExitAsync();

                    return output;
                }
            }

            double previousValue = 0;

            await RunProcessAsync("gpio", "mode", WPI.ToString(), "pwm");

            while (true)
            {

                var ranges = new List<ThresholdRange>();

                foreach (var threshold in thresholds)
                {
                    if (thresholds.FindIndex(p => p == threshold) == 0)
                        ranges.Add(new ThresholdRange(
                            start: int.MinValue,
                            end: threshold.Temperature,
                            value: 0));
                    else if (thresholds.FindIndex(p => p == threshold) == thresholds.Count() - 1)
                    {
                        ranges.Add(new ThresholdRange(
                            start: thresholds[thresholds.FindIndex(p => p == threshold) - 1].Temperature + 0.0001,
                            end: threshold.Temperature,
                            value: thresholds[thresholds.FindIndex(p => p == threshold) - 1].Value));

                        ranges.Add(new ThresholdRange(
                            start: threshold.Temperature,
                            end: int.MaxValue,
                            value: 1000));
                    }
                    else
                        ranges.Add(new ThresholdRange(
                            start: thresholds[thresholds.FindIndex(p => p == threshold) - 1].Temperature + 0.0001,
                            end: threshold.Temperature,
                            value: thresholds[thresholds.FindIndex(p => p == threshold) - 1].Value));
                }

                var temeratureCheck = await RunProcessAsync("cat", "/sys/class/thermal/thermal_zone0/temp");

                if (double.TryParse(temeratureCheck, out double temperature))
                {
                    if (temperature > 0)
                        temperature = temperature / 1000;

                    var value = ranges.SingleOrDefault(r => temperature >= r.Start && temperature <= r.End)?.Value;

                    value = value ?? 0;

                    if (previousValue != value)
                    {
                        previousValue = value.Value;

                        Console.WriteLine($"{temperature} => {value}");

                        await RunProcessAsync("gpio", "pwm", WPI.ToString(), value.ToString());
                    }
                }
            }

            Task.Delay(TimeSpan.FromSeconds(1)).Wait();
        }
    }
}
