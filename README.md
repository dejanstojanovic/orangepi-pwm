![Connection diagram](https://raw.githubusercontent.com/dejanstojanovic/orangepi-pwm/main/Diagram/diagram.png)

# OrangePi PWM fan control
The purpose of this project is to manage to control fan speed based on the temperature of the SOC for **Orange Pi** SBC. 

The solution is tested and currently running on OrangePi5 but you should not have any issues runnig it on older **Orange Pi 4** or never **Orange Pi 5B** or **Orange Pi 5 Plus**.

### Prerequisites
For start you need to use any PWM enabled fan. In my case I am using [Noctua NF-A4x10 5V PWM](https://noctua.at/en/nf-a4x10-5v-pwm) but you should be good with pretty much any fan that supports PWM.
Connect fan to Orange Pi 5 GND and 5V pins GPIO pins. PWM should be connected to GPIO7 which you will use to control the value.

## OrangePi.PWM
This is simple console application that you can use with simple ```dotnet run OrangePi.PWM/OrangePi.PWM.csproj```. Alternatively you can also publish it and run it's binary, but every code change will require you to rebuild it

All configurations are in the code and in order to change any of the pramaters will requre you to stop and start the process.

To ensure the process is running even after the reboot, you can register it as **systemd** service
```ini
[Unit]
Description=Run PWM control

[Service]
ExecStart=%h/.dotnet/dotnet run --project /etc/orangepi-pwm/OrangePi.PWM.csproj --property:Configuration=Release
User=root
Group=0
Type=simple
Restart=no

[Install]
WantedBy=multi-user.target
```
## OrangePi.PWM.Service
This project is a proper service library. It is recommended to run it as compiled binary.

All configurations are in **appsettings.json** configuration file. Once peoject is published, in order to change any or the paramaters you do not need to stop and start tr process.
Confiuration values will be automatically reloded once configuration/setting file changes are saved.
To run it as a **systemd** service use the followin service configuration
```ini
[Unit]
Description=Run PWM control

[Service]
ExecStart=%h/.dotnet/dotnet /etc/orangepi-pwm/OrangePi.PWM.Service.dll
WorkingDirectory=/etc/orangepi-pwm
User=root
Group=0
Type=simple
Restart=no

[Install]
WantedBy=multi-user.target
```

To setup and start the service run the following commands
```
systemctl daemon-reload
systemctl enable orangepi-pwm.service
systemctl start orangepi-pwm.service
```

While service is runnig you can check recent logs and service status 
```
systemctl status orangepi-pwm.service
```
### Configuring temperature thresholds
Predefined temperature thresholds and PWM values for each threshold are configured in ```appsettings.json``` file.
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "ServiceConfiguration": {
    "wPi": 2,
    "IntervalSeconds": 1,
    "TemperatureConfigurations": [
      {
        "Temperature": 0,
        "Value": 0
      },
      {
        "Temperature": 30,
        "Value": 100
      },
      {
        "Temperature": 40,
        "Value": 300
      },
      {
        "Temperature": 50,
        "Value": 400
      },
      {
        "Temperature": 60,
        "Value": 500
      },
      {
        "Temperature": 70,
        "Value": 800
      },
      {
        "Temperature": 80,
        "Value": 1000
      }
    ]
  }
}
```
If you want to update thresholds or PWM values you just need to update values in ```appsettings.json``` file without need to restart the service. New values will be picked up automatically once you save the file.
