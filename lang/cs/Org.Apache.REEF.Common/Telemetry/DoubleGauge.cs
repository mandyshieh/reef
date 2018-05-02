using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Org.Apache.REEF.Utilities.Attributes;

namespace Org.Apache.REEF.Common.Telemetry
{
    class DoubleGauge : MetricBase<double>
    {
        public DoubleGauge(string name, string description)
            : base(name, description)
        {
        }

        [JsonConstructor]
        internal DoubleGauge(string name, string description, long timeStamp, double value)
            : base(name, description, timeStamp, value)
        {
        }

        public override void Update(IMetric me)
        {
            _typedValue = Convert.ToDouble(me.ValueUntyped);
            _timestamp = DateTime.Now.Ticks;
        }

        public override void Update(object val)
        {
            _typedValue = Convert.ToDouble(val);
            _timestamp = DateTime.Now.Ticks;
        }

        public override IMetric Copy()
        {
            return new DoubleGauge(Name, Description, _timestamp, _typedValue);
        }
    }
}
