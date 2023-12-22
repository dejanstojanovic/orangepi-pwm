namespace OrangePi.Display.Status.Service.Models
{
    public class StatusValue
    {
        public StatusValue(double value, string valueText)
        {
            ValueText = valueText;
            Value = value;
        }

        public StatusValue(double value, string valueText, string? note) : this(value, valueText)
        {
            this.Note = note;
        }
        public string ValueText { get; set; }
        public double Value { get; set; }
        public string? Note { get; set; }
    }
}
