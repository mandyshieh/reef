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
    /// This class wraps a metric object and the increment value since last sink
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
        }

        [JsonConstructor]
        internal MetricData(IMetric metric, IList<IMetric> records, int changes)
        {
            _metric = metric;
            _records = records;
            ChangesSinceLastSink = changes;
        }

        /// <summary>
        /// clear the increment since last sink
        /// </summary>
        internal void ResetChangeSinceLastSink()
        {
            ChangesSinceLastSink = 0;
            _records.Clear();
        }

        internal void UpdateMetric(MetricData metric)
        {
            if (metric.GetMetric().GetType() == _metric.GetType())
            {
                if (metric.GetMetric().ValueUntyped != _metric.ValueUntyped || metric._records.Count > 0)
                {
                    ChangesSinceLastSink += metric.ChangesSinceLastSink;
                    IMetric tmp = _metric;
                    foreach (var r in metric._records)
                    {
                        _records.Add(r);
                    }
                    _records.Add(tmp);

                    //// TODO: [REEF-1748] The following cases need to be considered in determine how to update the metric:
                    //// if evaluator contains the aggregated values, the value will override existing value
                    //// if evaluator only keep delta, the value should be added at here. But the value in the evaluator should be reset after message is sent
                    //// For the metrics from multiple evaluators with the same metric name, the value should be aggregated here
                    //// We also need to consider failure cases.  
                    _metric = metric.GetMetric();
                }
                else
                {
                    Logger.Log(Level.Info, "Metric {0} not updated because value has not changed.", _metric.Name);
                }
            }
            else
            {
                Logger.Log(Level.Error, "Trying to update metric {0} of type {1} with type {2}", _metric.Name, _metric.GetType(), metric.GetType());
                throw new Exception("Trying to update metric of type" + _metric.GetType() + " with type " + metric.GetType());
            }
        }

        internal void UpdateMetric(string name, object val)
        {
            ChangesSinceLastSink++;
            var copy = _metric.Copy();
            copy.ValueUntyped = val;

            IMetric tmp = _metric;
            _records.Add(tmp);

            //// TODO: [REEF-1748] The following cases need to be considered in determine how to update the metric:
            //// if evaluator contains the aggregated values, the value will override existing value
            //// if evaluator only keep delta, the value should be added at here. But the value in the evaluator should be reset after message is sent
            //// For the metrics from multiple evaluators with the same metric name, the value should be aggregated here
            //// We also need to consider failure cases.
            _metric = copy;
        }

        internal IMetric GetMetric()
        {
            return _metric;
        }

        /// <summary>
        /// Get count name and value as KeyValuePair
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<KeyValuePair<string, string>> GetKeyValuePair()
        {
            // return new KeyValuePair<string, string>(_metric.Name, _metric.Value.ToString());
            Logger.Log(Level.Info, "Getting KeyValuPair in MetricData.");
            var values = new List<KeyValuePair<string, string>>();
            foreach (var r in _records)
            {
                values.Add(new KeyValuePair<string, string>(_metric.Name, r.ValueUntyped.ToString()));
            }
            Logger.Log(Level.Info, "Got MetricData: {0}", JsonConvert.SerializeObject(values));
            return values;
        }
    }
}
