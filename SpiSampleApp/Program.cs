// See https://aka.ms/new-console-template for more information


using System.Device.Spi;

using var spiDevice = SpiDevice.Create(new SpiConnectionSettings(busId: 4, chipSelectLine: 52 /*4*/));


