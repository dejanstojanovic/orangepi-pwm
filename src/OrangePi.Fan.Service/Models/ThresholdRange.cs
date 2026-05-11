using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangePi.Fan.Service.Models
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
}
