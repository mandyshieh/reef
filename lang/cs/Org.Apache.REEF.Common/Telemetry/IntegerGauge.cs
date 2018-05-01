using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Org.Apache.REEF.Utilities.Attributes;

namespace Org.Apache.REEF.Common.Telemetry
{
    class IntegerGauge : IMetric<int>
    {
        private string _name;
        private string _description;
        private int _typedValue;
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
                _typedValue = Convert.ToInt32(value);
            }
        }

        public long Timestamp
        {
            get
            {
                return _timestamp;
            }
        }

        public int Value
        {
            get
            {
                return _typedValue;
            }
            set
            {
                _typedValue = Convert.ToInt32(value);
            }
        }

        public IntegerGauge(string name, string description)
        {
            _name = name;
            _description = description;
            _timestamp = DateTime.Now.Ticks;
            _typedValue = default(int);
        }

        [JsonConstructor]
        internal IntegerGauge(string name, string description, long timeStamp, int value)
        {
            _name = name;
            _description = description;
            _timestamp = timeStamp;
            _typedValue = value;
        }

        public IMetric Copy()
        {
            return new IntegerGauge(_name, _description, _timestamp, _typedValue);
        }
    }
}
