﻿// Licensed to the Apache Software Foundation (ASF) under one
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
using System.Linq;
using Newtonsoft.Json;
using Org.Apache.REEF.Utilities.Logging;

namespace Org.Apache.REEF.Common.Telemetry
{
    /// <summary>
    /// MetricData class maintains the current value of a single metric and keeps count of the number
    /// of times this metric has been updated. If the metric is immutable, it keeps a record of updates.
    /// Once the data has been processed, the records and count will reset.
    /// </summary>
    [JsonObject]
    public sealed class MetricData : IObserver<IMetric>
    {
        private static readonly Logger Logger = Logger.GetLogger(typeof(MetricsData));

        private bool _keepUpdateHistory;

        [JsonProperty]
        private IMetric _mirror;

        /// <summary>
        /// List of the history of values this metric has held. If _keepUpdateHistory is false, only holds current value.
        /// </summary>
        [JsonProperty]
        private IList<MetricRecord> _records;

        /// <summary>
        /// Number of times metric has been updated since last processed.
        /// </summary>
        [JsonProperty]
        internal int ChangesSinceLastSink;

        /// <summary>
        /// Constructor for metricData
        /// </summary>
        /// <param name="metric"></param>
        /// <param name="initialValue"></param>
        internal MetricData(IMetric metric)
        {
            Subscribe(metric);
            _mirror = metric;
            ChangesSinceLastSink = 0;
            _keepUpdateHistory = metric.IsImmutable;
            _records = new List<MetricRecord>();
            _records.Add(CreateMetricRecord());
        }

        [JsonConstructor]
        internal MetricData(IList<MetricRecord> records, int changes)
        {
            _records = records;
            ChangesSinceLastSink = changes;
        }

        /// <summary>
        /// Reset records.
        /// </summary>
        internal void ResetChangesSinceLastSink()
        {
            ChangesSinceLastSink = 0;
            _records.Clear();
        }

        /// <summary>
        /// When new metric data is received, update the value and records so it reflects the new data.
        /// </summary>
        /// <param name="metric">Metric data received.</param>
        internal MetricData UpdateMetric(MetricData metric)
        {
            _mirror = metric.GetMetric();
            if (metric.ChangesSinceLastSink > 0)
            {
                if (_keepUpdateHistory)
                {
                    _records.Concat(metric._records);
                }
                else
                {
                    _records = metric.GetMetricRecords().ToList();
                }
            }
            ChangesSinceLastSink += metric.ChangesSinceLastSink;
            return this;
        }

        /// <summary>
        /// Updates metric value with metric object received.
        /// </summary>
        /// <param name="me">New metric.</param>
        internal void UpdateMetric(IMetric me)
        {
            ChangesSinceLastSink++;
            _mirror = me;
            UpdateRecords();
        }

        /// <summary>
        /// Updates metric value given its name.
        /// </summary>
        /// <param name="name">Name of the metric to update.</param>
        /// <param name="val">New value.</param>
        internal void UpdateMetric(string name, object val)
        {
            ChangesSinceLastSink++;
            _mirror.AssignNewValue(val);
            UpdateRecords();
        }

        private void UpdateRecords()
        {
            var newRecord = CreateMetricRecord();
            if (_keepUpdateHistory)
            {
                _records.Add(newRecord);
            }
            else
            {
                _records[0] = newRecord;
            }
        }

        /// <summary>
        /// Get the metric with its most recent value.
        /// </summary>
        /// <returns></returns>
        internal IMetric GetMetric()
        {
            return _mirror;
        }

        /// <summary>
        /// Get all the metric records.
        /// </summary>
        /// <returns>The history of the metric values.</returns>
        internal IEnumerable<MetricRecord> GetMetricRecords()
        {
            return _records;
        }

        public void Subscribe(IMetric provider)
        {
            _mirror = provider;
            provider.Subscribe(this);
        }

        public void OnNext(IMetric metric)
        {
            UpdateMetric(metric);
        }

        public void OnError(Exception error)
        {
        }

        public void OnCompleted()
        {
        }

        private MetricRecord CreateMetricRecord()
        {
            return new MetricRecord(this);
        }

        [JsonObject]
        public struct MetricRecord
        {
            [JsonProperty]
            public object Value { get; }

            [JsonProperty]
            public long Timestamp { get; }

            [JsonConstructor]
            public MetricRecord(object value, long timestamp)
            {
                Value = value;
                Timestamp = timestamp;
            }

            public MetricRecord(MetricData metricData)
            {
                Timestamp = metricData._mirror.Timestamp;
                Value = metricData._mirror.ValueUntyped;
            }
        }
    }
}
