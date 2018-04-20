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
using Org.Apache.REEF.Tang.Annotations;
using Org.Apache.REEF.Utilities.Attributes;
using Org.Apache.REEF.Utilities.Logging;

namespace Org.Apache.REEF.Common.Telemetry
{
    [Unstable("0.16", "This is to build a collection of metrics for evaluator metrics.")]
    internal sealed class MetricsImpl : IMetrics
    {
        private static readonly Logger Logger = Logger.GetLogger(typeof(MetricsImpl));

        /// <summary>
        /// It contains name and count pairs
        /// </summary>
        private readonly IDictionary<string, IMetricBase> _metricsDict = new Dictionary<string, IMetricBase>();

        /// <summary>
        /// The lock for metrics
        /// </summary>
        private readonly object _metricLock = new object();

        [Inject]
        private MetricsImpl()
        {
        }

        /// <summary>
        /// Deserialize a metrics serialized string into a metrics object
        /// </summary>
        /// <param name="serializedMetricsString"></param>
        internal MetricsImpl(string serializedMetricsString)
        {
            var metrics = JsonConvert.DeserializeObject<IEnumerable<IMetricBase>>(serializedMetricsString);
            foreach (var m in metrics)
            {
                _metricsDict.Add(m.Name, m);
            }
        }

        public IEnumerable<IMetricBase> GetMetrics()
        {
            return _metricsDict.Values;
        }

        /// <summary>
        /// Register a new metric with a specified name.
        /// If name does not exist, the metric will be added and true will be returned
        /// Otherwise the metric will be not added and false will be returned. 
        /// </summary>
        /// <param name="name">Metric name</param>
        /// <param name="description">Metric description</param>
        /// <returns>Returns a boolean to indicate if the metric is added.</returns>
        public bool TryRegisterMetric(IMetricBase metric)
        {
            lock (_metricLock)
            {
                if (_metricsDict.ContainsKey(metric.Name))
                {
                    Logger.Log(Level.Warning, "The metric [{0}] already exists.", metric.Name);
                    return false;
                }
                _metricsDict.Add(metric.Name, metric);
            }
            return true;
        }

        /// <summary>
        /// Get metric for a given name
        /// return false if the metric isn't registered.
        /// </summary>
        /// <param name="name">Name of the metric</param>
        /// <param name="registeredMetric">Value of the metric returned</param>
        /// <returns>Returns a boolean to indicate if the value is found.</returns>
        public bool TryGetValue(string name, out IMetricBase registeredMetric)
        {
            lock (_metricLock)
            {
                return _metricsDict.TryGetValue(name, out registeredMetric);
            }
        }

        /// <summary>
        /// return serialized string of metric data
        /// TODO: [REEF-] use an unique number for the metric name mapping to reduce the data transfer over the wire
        /// TODO: [REEF-] use Avro schema if that can make the serialized string more compact
        /// </summary>
        /// <returns>Returns serialized string of the metrics.</returns>
        public string Serialize()
        {
            lock (_metricLock)
            {
                if (_metricsDict.Count > 0)
                {
                    return JsonConvert.SerializeObject(_metricsDict.Values);
                }
                return null;
            }
        }
    }
}
