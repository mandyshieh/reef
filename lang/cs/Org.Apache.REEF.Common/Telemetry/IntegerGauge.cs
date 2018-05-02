using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Org.Apache.REEF.Utilities.Attributes;

namespace Org.Apache.REEF.Common.Telemetry
{
    class IntegerGauge : MetricBase<int>
    {
        public override bool IsImmutable
        {
            get { return true; }
        }

        public IntegerGauge(string name, string description)
            : base(name, description)
        {
        }

        [JsonConstructor]
        internal IntegerGauge(string name, string description, long timeStamp, int value)
            : base(name, description, timeStamp, value)
        {
        }

        public override void Update(IMetric me)
        {
            _typedValue = Convert.ToInt32(me.ValueUntyped);
            _timestamp = DateTime.Now.Ticks;
        }

        public override void Update(object val)
        {
            _typedValue = Convert.ToInt32(val);
            _timestamp = DateTime.Now.Ticks;
        }

        public override IMetric Copy()
        {
            return new IntegerGauge(Name, Description, _timestamp, _typedValue);
        }
    }
}
