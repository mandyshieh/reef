using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Org.Apache.REEF.Utilities.Attributes;

namespace Org.Apache.REEF.Common.Telemetry
{
    class DoubleGauge : MetricBase
    {
        private double _typedValue;

        public override object Value
        {
            get
            {
                return _typedValue;
            }
            set
            {
                _value = value;
                _typedValue = (double)value;
                _timestamp = DateTime.Now.Ticks;
            }
        }

        public DoubleGauge(string name, string description) :
            base(name, description)
        {
            _type = MetricType.Double;
        }

        [JsonConstructor]
        internal DoubleGauge(string name, string description, long timeStamp, double value) :
            base(name, description, value, timeStamp, MetricType.Double)
        {
            _typedValue = value;
        }
    }
}
