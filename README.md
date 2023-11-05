# OrangePi PWM fan control
The purpose of this project is to manage to control fan speed based on the temperature of the SOC for **Orange Pi** SBC. 

The solution is tested and currently running on OrangePi5 but you should not have any issues runnig it on older **Orange Pi 4** or never **Orange Pi 5B** or **Orange Pi 5 Plus**.

## Prerequisites
For start you need to use any PWM enabled fan. In my case I am using [Noctua NF-A4x10 5V PWM](https://noctua.at/en/nf-a4x10-5v-pwm) but you should be good with pretty much any fan that supports PWM.
Connect fan to Orange Pi 5 GND and 5V pins GPIO pins. PWM should be connected to GPIO7 which you will use to control the value.

