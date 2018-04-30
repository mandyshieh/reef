// Licensed to the Apache Software Foundation (ASF) under one
// or more contributor license agreements.  See the NOTICE file
// distributed with this work for additional information
// regarding copyright ownership.  The ASF licenses this file
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

using System.Collections.Generic;

namespace Org.Apache.REEF.Common.Telemetry
{
    /// <summary>
    /// This class wraps a Counter object and the increment value since last sink
    /// </summary>
    internal sealed class CounterData
    {
        /// <summary>
        /// Counter object
        /// </summary>
        private ICounter _counter;

        /// <summary>
        /// Counter increment value since last sink
        /// </summary>
        internal int IncrementSinceLastSink { get; private set; }

        /// <summary>
        /// Constructor for CounterData
        /// </summary>
        /// <param name="counter"></param>
        /// <param name="initialValue"></param>
        internal CounterData(ICounter counter, int initialValue)
        {
            _counter = counter;
            IncrementSinceLastSink = initialValue;
        }

        /// <summary>
        /// clear the increment since last sink
        /// </summary>
        internal void ResetSinceLastSink()
        {
            IncrementSinceLastSink = 0;
        }

        /// <summary>
        /// When new metric data is received, update the value and records so it reflects the new data.
        /// </summary>
        /// <param name="metric">Metric data received.</param>
        internal void UpdateMetric(MetricData metric)
        {
            if (!(metric.GetMetric() is ICounter))
            {
                foreach (var r in metric._records)
                {
                    _records.Add(r);
                }
                _records.Add(_metric);
            }
            ChangesSinceLastSink += metric.ChangesSinceLastSink;
            _metric = metric.GetMetric(); // update current metric value
        }

        internal void UpdateMetric(IMetric me)
        {
            if (me.GetType() != _metric.GetType())
            {
                throw new ApplicationException("Trying to update metric of type " + _metric.GetType() + " with type " + me.GetType());
            }
            if (!(me is ICounter))
            {
                _records.Add(_metric);
            }
            ChangesSinceLastSink++;
            _metric = me; // update current metric value
        }

        internal void UpdateMetric(string name, object val)
        {
            var tmp = _metric.Copy();
            if (!(_metric is ICounter))
            {
                _records.Add(tmp);
            }
            _metric.ValueUntyped = val;
            ChangesSinceLastSink++;
        }

        internal IMetric GetMetric()
        {
            return _metric;
        }

        /// <summary>
        /// Get KeyValuePair for every record and current metric value.
        /// </summary>
        /// <returns>This metric's values.</returns>
        internal IEnumerable<KeyValuePair<string, string>> GetKeyValuePair()
        {
            var values = new List<KeyValuePair<string, string>>();
            foreach (var r in _records)
            {
                values.Add(new KeyValuePair<string, string>(_metric.Name, r.ValueUntyped.ToString()));
            }
            values.Add(new KeyValuePair<string, string>(_metric.Name, _metric.ValueUntyped.ToString()));
            return values;
        }
    }
}
