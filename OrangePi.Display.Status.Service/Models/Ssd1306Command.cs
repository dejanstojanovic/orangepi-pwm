using Iot.Device.Ssd13xx.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangePi.Display.Status.Service.Models
{
    internal class Ssd1306Command : ISsd1306Command
    {
        public Ssd1306Command(byte id)
        {
            Id = id;
        }
        public byte Id { get; init; }

        public byte[] GetBytes()
        {
            return [Id];
        }
    }
}
