using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangePi.Display.Status.Service.Models
{
    internal class StatusValue
    {
        public StatusValue(string text, double value)
        {
            Text = text;
            Value = value;
        }
        public string Text { get; set; }
        public double Value { get; set; }
    }
}
