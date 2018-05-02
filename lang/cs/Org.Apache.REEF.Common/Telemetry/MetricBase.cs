using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Org.Apache.REEF.Common.Telemetry
{
    abstract class MetricBase<T> : IMetric<T>
    {
        protected T _typedValue;
        protected long _timestamp;

        public string Name { get; }

        public string Description { get; }

        public virtual long Timestamp
        {
            get { return _timestamp; }
        }

        public virtual object ValueUntyped
        {
            get { return _typedValue; }
        }

        public virtual T Value
        {
            get
            {
                return _typedValue;
            }
        }

        public abstract bool IsImmutable { get; }

        public MetricBase(string name, string description)
        {
            Name = name;
            Description = description;
            _timestamp = DateTime.Now.Ticks;
            _typedValue = default(T);
        }

        public MetricBase(string name, string description, long timeStamp, T value)
        {
            Name = name;
            Description = description;
            _timestamp = timeStamp;
            _typedValue = value;
        }

        public abstract IMetric Copy();

        public abstract void Update(IMetric me);

        public abstract void Update(object val);
    }
}
