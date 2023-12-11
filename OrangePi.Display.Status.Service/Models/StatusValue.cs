using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangePi.Display.Status.Service.Models
{
    internal class StatusValue
    {
        public StatusValue(string label, double value, string valueText)
        {
            ValueText = valueText;
            Value = value;
            Label = label;
        }
        public string Label { get; set; }
        public string ValueText { get; set; }
        public double Value { get; set; }
    }
}
