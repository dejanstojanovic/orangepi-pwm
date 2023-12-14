namespace I2CSample
{
    internal class StatusValue
    {
        public StatusValue(string label,double value,string valueText)
        {
            ValueText = valueText;
            Value = value;
            Label = label;
        }

        public StatusValue(string label, double value, string valueText, string? note):this(label,value,valueText)
        {
           this.Note = note;
        }
        public string Label { get; set; }
        public string ValueText { get; set; }
        public double Value { get; set; }
        public string? Note { get; set; }
    }
}
