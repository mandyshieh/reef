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
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Org.Apache.REEF.Utilities.Attributes;

namespace Org.Apache.REEF.Common.Telemetry
{
    /// <summary>
    /// Counter implementation
    /// The properties that need to be serialized will be revisited later. We should only serialize minimum data to reduce the network load
    /// For example, the name can be mapped to a unique number (byte) and description should not be serialized.
    /// </summary>
    [Unstable("0.16", "This is a simple counter for evaluator metrics.")]
    internal sealed class Counter : ICounter
    {
        private string _name;
        private string _description;
        private int _typedValue;
        private long _timestamp;

        public string Name => _name;

        public string Description => _description;

        public object ValueUntyped => _typedValue;

        public long Timestamp => _timestamp;

        public int Value { get => _typedValue; set => _typedValue = value; }

        public Counter(string name, string description)
        {
            _name = name;
            _description = description;
            _timestamp = DateTime.Now.Ticks;
            _typedValue = default(int);
        }

        [JsonConstructor]
        internal Counter(string name, string description, long timeStamp, int value)
        {
            _name = name;
            _description = description;
            _timestamp = timeStamp;
            _typedValue = value;
        }

        /// <summary>
        /// Increase the counter value and update the time stamp.
        /// </summary>
        /// <param name="number"></param>
        public void Increment(int number)
        {
            _typedValue += number;
            _timestamp = DateTime.Now.Ticks;
        }

        public void Increment()
        {
            _typedValue += 1;
            _timestamp = DateTime.Now.Ticks;
        }

        public void Decrement()
        {
            _typedValue -= 1;
            _timestamp = DateTime.Now.Ticks;
        }

        public void Decrement(int number)
        {
            _typedValue -= number;
            _timestamp = DateTime.Now.Ticks;
        }
    }
}
