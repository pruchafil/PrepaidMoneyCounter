using System;

namespace PrepaidMoneyCounter.Model
{
    public class Record
    {
        public Guid Guid { get; set; } = Guid.NewGuid();

        public RecordType RecordType { get; set; }

        public DateTime DateTime { get; set; } = DateTime.Now;

        public decimal Cost { get; set; } = 0.0m;

        public string Message { get; set; } = string.Empty;

        public Record()
        {

        }
    }
}

