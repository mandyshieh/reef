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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Org.Apache.REEF.Tang.Annotations;
using Org.Apache.REEF.Utilities.Logging;

namespace Org.Apache.REEF.Common.Telemetry
{
    /// <summary>
    /// This class maintains a collection of the data for all the metrics for metrics service. 
    /// When new metric data is received, the data in the collection will be updated.
    /// After the data is processed, the increment since last process will be reset.
    /// </summary>
    internal sealed class MetricsData
    {
        private static readonly Logger Logger = Logger.GetLogger(typeof(MetricsData));

        /// <summary>
        /// Registration of metrics
        /// </summary>
        private readonly IDictionary<string, MetricData> _metricMap = new ConcurrentDictionary<string, MetricData>();

        [Inject]
        private MetricsData()
        {
        }

        /// <summary>
        /// Update metrics 
        /// </summary>
        /// <param name="metrics"></param>
        internal void Update(IMetrics metrics)
        {
            foreach (var metric in metrics.GetMetrics())
            {
                if (_metricMap.TryGetValue(metric.Name, out MetricData metricData))
                {
                    metricData.UpdateMetric(metric);
                }
                else
                {
                    _metricMap.Add(metric.Name, new MetricData(metric));
                }

                Logger.Log(Level.Verbose, "Metric name: {0}, value: {1}, description: {2}, time: {3},  changed since last sink: {4}.",
                    metric.Name, metric.Value, metric.Description, new DateTime(metric.Timestamp), _metricMap[metric.Name].ChangesSinceLastSink);
            }
        }

        /// <summary>
        /// Reset changed since last sink for each metric
        /// </summary>
        internal void Reset()
        {
            foreach (var c in _metricMap.Values)
            {
                c.ResetChangeSinceLastSink();
            }
        }

        /// <summary>
        /// Convert the metric data into ISet for sink
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<KeyValuePair<string, string>> GetMetricData()
        {
            return _metricMap.Select(metric => metric.Value.GetKeyValuePair()).SelectMany(m => m);
        }

        /// TODO
        /// <summary>
        /// The condition that triggers the sink. The condition can be modified later.
        /// </summary>
        /// <returns></returns>
        internal bool TriggerSink(int metricSinkThreshold)
        {
            return _metricMap.Values.Sum(e => e.ChangesSinceLastSink) > metricSinkThreshold;
        }
    }
}
