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
using Newtonsoft.Json;

namespace Org.Apache.REEF.Common.Telemetry
{
    /// <summary>
    /// This class maintains a collection of the data for all the metrics for metrics service. 
    /// When new metric data is received, the data in the collection will be updated.
    /// After the data is processed, the changes since last process will be reset.
    /// </summary>
    public sealed class MetricsData : IMetrics
    {
        private static readonly Logger Logger = Logger.GetLogger(typeof(MetricsData));

        JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All
        };

        /// <summary>
        /// Registration of metrics
        /// </summary>
        private IDictionary<string, MetricData> _metricsMap = new ConcurrentDictionary<string, MetricData>();

        /// <summary>
        /// The lock for metrics
        /// </summary>
        private readonly object _metricLock = new object();

        [Inject]
        private MetricsData()
        {
        }

        /// <summary>
        /// Deserialization.
        /// </summary>
        /// <param name="serializedMetricsString"></param>
        [JsonConstructor]
        internal MetricsData(string serializedMetricsString)
        {
            var metrics = JsonConvert.DeserializeObject<IList<MetricData>>(serializedMetricsString, settings);
            foreach (var m in metrics)
            {
                _metricsMap.Add(m.GetMetric().Name, m);
            }
        }

        internal MetricsData(IMetrics metrics)
        {
            foreach (var me in metrics.GetMetrics())
            {
                _metricsMap.Add(me.GetMetric().Name, new MetricData(me.GetMetric()));
            }
        }

        public bool TryRegisterMetric(IMetric metric)
        {
            lock (_metricLock)
            {
                if (_metricsMap.ContainsKey(metric.Name))
                {
                    Logger.Log(Level.Warning, "The metric [{0}] already exists.", metric.Name);
                    return false;
                }
                _metricsMap.Add(metric.Name, new MetricData(metric));
            }
            return true;
        }

        public bool TryGetValue(string name, out IMetric me)
        {
            lock (_metricLock)
            {
                if (!_metricsMap.TryGetValue(name, out MetricData md))
                {
                    me = null;
                    return false;
                }
                me = md.GetMetric();
            }
            return true;
        }

        public IEnumerable<MetricData> GetMetrics()
        {
            return _metricsMap.Values;
        }

        /// <summary>
        /// Update metrics 
        /// </summary>
        /// <param name="metrics"></param>
        internal void Update(MetricsData metrics)
        {
            foreach (var metric in metrics.GetMetrics())
            {
                var me = metric.GetMetric();
                if (_metricsMap.TryGetValue(me.Name, out MetricData metricData))
                {
                    metricData.UpdateMetric(metric);
                }
                else
                {
                    _metricsMap.Add(me.Name, metric);
                }
            }
        }

        internal void Update(IMetric me)
        {
            lock (_metricLock)
            {
                if (_metricsMap.TryGetValue(me.Name, out MetricData metricData))
                {
                    metricData.UpdateMetric(me);
                }
                else
                {
                    _metricsMap.Add(me.Name, new MetricData(me));
                }
            }
        }

        internal void Update(string name, object val)
        {
            lock (_metricLock)
            {
                if (_metricsMap.TryGetValue(name, out MetricData me))
                {
                    me.UpdateMetric(name, val);
                }
                else
                {
                    Logger.Log(Level.Error, "Metric {0} needs to be registered before it can be updated with value {1}.", name, val);
                    throw new Exception("Metric " + name + " has not been registered.");
                }
            }
        }

        /// <summary>
        /// Reset changed since last sink for each metric
        /// </summary>
        internal void Reset()
        {
            lock (_metricLock)
            {
                foreach (var c in _metricsMap.Values)
                {
                    c.ResetChangeSinceLastSink();
                }
            }
        }

        /// <summary>
        /// Convert the metric data into ISet for sink
        /// </summary>
        /// <returns>Key value pairs for all the metrics on record and their value.</returns>
        internal IEnumerable<KeyValuePair<string, string>> GetMetricPairs()
        {
            return _metricsMap.Select(metric => metric.Value.GetKeyValuePair()).SelectMany(m => m);
        }

        /// <summary>
        /// The condition that triggers the sink. The condition can be modified later.
        /// </summary>
        /// <returns></returns>
        internal bool TriggerSink(int metricSinkThreshold)
        {
            return _metricsMap.Values.Sum(e => e.ChangesSinceLastSink) > metricSinkThreshold;
        }

        public string Serialize()
        {
            lock (_metricLock)
            {
                if (_metricsMap.Count > 0)
                {
                    return JsonConvert.SerializeObject(_metricsMap.Values.Where(me => me.ChangesSinceLastSink > 0).ToList(), settings);
                }
            }
            return null;
        }
    }
}
