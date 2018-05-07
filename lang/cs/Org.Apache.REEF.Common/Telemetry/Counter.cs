// to you under the Apache License, Version 2.0 (the
// "License"); you may not use this file except in compliance
// with the License.  You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.

using System;
using Newtonsoft.Json;
using Org.Apache.REEF.Utilities.Attributes;

namespace Org.Apache.REEF.Common.Telemetry
{
    /// <summary>
    /// Counter implementation
    /// The properties that need to be serialized will be revisited later. We should only serialize minimum data to reduce the network load
    /// For example, the name can be mapped to a unique number (byte) and description should not be serialized.
    /// </summary>
    internal sealed class Counter : MetricBase<int>, ICounter
    {
        public override bool IsImmutable
        {
            get { return false; }
        }

        public Counter(string name, string description)
            : base(name, description)
        {
            _timestamp = DateTime.Now.Ticks;
            _typedValue = default(int);
        }

        [JsonConstructor]
        internal Counter(string name, string description, long timeStamp, int value)
            : base(name, description)
        {
            _timestamp = timeStamp;
            _typedValue = value;
        }

        public override IMetric CreateInstanceWithNewValue(object val)
        {
            return new Counter(Name, Description, DateTime.Now.Ticks, (int)val);
        }

        /// <summary>
        /// Increase the counter value and update the time stamp.
        /// </summary>
        /// <param name="number"></param>
        public void Increment(int number = 1)
        {
            _typedValue += number;
            _timestamp = DateTime.Now.Ticks;
        }

        public void Decrement(int number = 1)
        {
            _typedValue -= number;
            _timestamp = DateTime.Now.Ticks;
        }
    }
}
