using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OrangePi.PWM
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            const int WPI = 2;
            (int Temperature, int Speed)[] ranges = new[] {
                (35, 100), (40, 300), (50, 400), (60, 500), (70, 800), (80, 1000)
            };

            Process getProcess(string command, params string[] args)
            {
                Process process = new Process();
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.FileName = command;
                foreach (var arg in args)
                {
                    process.StartInfo.ArgumentList.Add(arg);
                }
                return process;
            }

            int previousSpeed = 0;

            using (var pwmPinSet = getProcess("gpio", "mode", WPI.ToString(), "pwm"))
            {
                pwmPinSet.Start();
                await pwmPinSet.WaitForExitAsync();
            }

            while (true)
            {
                using (var temeratureCheck = getProcess("cat", "/sys/class/thermal/thermal_zone0/temp"))
                {
                    temeratureCheck.Start();
                    string temeratureCheckOutput = temeratureCheck.StandardOutput.ReadToEnd();
                    await temeratureCheck.WaitForExitAsync();

                    if (double.TryParse(temeratureCheckOutput, out double temperature))
                    {
                        if (temperature > 0)
                            temperature = temperature / 1000;

                        var speed = ranges.OrderBy(r => r.Temperature).Where(r => r.Temperature <= temperature).Last().Speed;
                        if (previousSpeed != speed)
                        {
                            previousSpeed = speed;

                            Console.WriteLine($"{temperature} => {speed}");

                            using (var pwmSet = getProcess("gpio", "pwm", WPI.ToString(), speed.ToString()))
                            {
                                pwmSet.Start();
                                await pwmSet.WaitForExitAsync();
                            }
                        }
                    }
                }

                Task.Delay(TimeSpan.FromSeconds(1)).Wait();
            }
        }
    }
}