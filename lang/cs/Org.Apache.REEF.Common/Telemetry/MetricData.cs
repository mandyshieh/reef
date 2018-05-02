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

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Org.Apache.REEF.Utilities.Logging;

namespace Org.Apache.REEF.Common.Telemetry
{
    /// <summary>
    /// This class wraps a metric object, the record and counts of updates since last sink.
    /// </summary>
    [JsonObject]
    public sealed class MetricData
    {
        private static readonly Logger Logger = Logger.GetLogger(typeof(MetricsData));

        /// <summary>
        /// Metric object
        /// </summary>
        [JsonProperty]
        private IMetric _metric;

        [JsonProperty]
        private IList<IMetric> _records;

        ///// <summary>
        ///// Whether metric has been updated since last sink.
        ///// </summary>
        [JsonProperty]
        internal int ChangesSinceLastSink;

        private bool _keepUpdateRecord;

        /// <summary>
        /// Constructor for metricData
        /// </summary>
        /// <param name="metric"></param>
        /// <param name="initialValue"></param>
        internal MetricData(IMetric metric)
        {
            _metric = metric;
            ChangesSinceLastSink = 0;
            _records = new List<IMetric>();
            _records.Add(_metric.Copy());
        }

        [JsonConstructor]
        internal MetricData(IMetric metric, IList<IMetric> records, int changes, bool record)
        {
            _metric = metric;
            _records = records;
            ChangesSinceLastSink = changes;
            _keepUpdateRecord = record;
        }

        /// <summary>
        /// clear the increment since last sink
        /// </summary>
        internal void ResetChangeSinceLastSink()
        {
            ChangesSinceLastSink = 0;
            _records.Clear();
        }

        /// <summary>
        /// When new metric data is received, update the value and records so it reflects the new data.
        /// </summary>
        /// <param name="metric">Metric data received.</param>
        internal void UpdateMetric(MetricData metric)
        {
            if (metric.GetMetric().IsImmutable && metric.ChangesSinceLastSink > 0)
            {
                foreach (var r in metric._records)
                {
                    _records.Add(r);
                }
            }
            ChangesSinceLastSink += metric.ChangesSinceLastSink;
            _metric.Update(metric.GetMetric()); // update current metric value
        }

        internal void UpdateMetric(IMetric me)
        {
            if (me.GetType() != _metric.GetType())
            {
                throw new ApplicationException("Trying to update metric of type " + _metric.GetType() + " with type " + me.GetType());
            }
            if (me.IsImmutable)
            {
                _records.Add(me);
            }
            ChangesSinceLastSink++;
            _metric.Update(me); // update current metric value
        }

        internal void UpdateMetric(string name, object val)
        {
            var tmp = _metric.Copy();
            _metric.Update(val);
            if (_metric.IsImmutable)
            {
                _records.Add(_metric.Copy());
            }
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

            if (_metric.IsImmutable)
            {
                foreach (var r in _records)
                {
                    values.Add(new KeyValuePair<string, string>(_metric.Name, r.ValueUntyped.ToString()));
                }
            }
            else
            {
                values.Add(new KeyValuePair<string, string>(_metric.Name, _metric.ValueUntyped.ToString()));
            }
            return values;
        }
    }
}
