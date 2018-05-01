using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Org.Apache.REEF.Utilities.Attributes;

namespace Org.Apache.REEF.Common.Telemetry
{
    class DoubleGauge : IMetric<double>
    {
        private string _name;
        private string _description;
        private double _typedValue;
        private long _timestamp;

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
        }

        public object ValueUntyped
        {
            get
            {
                return _typedValue;
            }
            set
            {
                _typedValue = Convert.ToDouble(value);
            }
        }

        public long Timestamp
        {
            get
            {
                return _timestamp;
            }
        }

        public double Value
        {
            get
            {
                return _typedValue;
            }
            set
            {
                _typedValue = Convert.ToDouble(value);
            }
        }

        public DoubleGauge(string name, string description)
        {
            _name = name;
            _description = description;
            _timestamp = DateTime.Now.Ticks;
            _typedValue = default(double);
        }

        [JsonConstructor]
        internal DoubleGauge(string name, string description, long timeStamp, double value)
        {
            _name = name;
            _description = description;
            _timestamp = timeStamp;
            _typedValue = value;
        }

        public IMetric Copy()
        {
            return new DoubleGauge(_name, _description, _timestamp, _typedValue);
        }
    }
}
