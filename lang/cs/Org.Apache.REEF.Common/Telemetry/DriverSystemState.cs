using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Org.Apache.REEF.Common.Telemetry
{
    class DriverSystemState : MetricBase<string>
    {
        public override bool IsImmutable
        {
            get { return false; }
        }

        public DriverSystemState(string name, string description)
            : base(name, description)
        {
        }

        internal DriverSystemState(string name, string description, long timeStamp, string value)
            : base(name, description, timeStamp, value)
        {
        }

        public override void Update(IMetric me)
        {
            _typedValue = Convert.ToString(me.ValueUntyped);
            _timestamp = DateTime.Now.Ticks;
        }

        public override void Update(object val)
        {
            _typedValue = Convert.ToString(val);
            _timestamp = DateTime.Now.Ticks;
        }

        public override IMetric Copy()
        {
            return new DriverSystemState(Name, Description, _timestamp, _typedValue);
        }
    }
}
