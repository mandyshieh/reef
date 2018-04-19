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
using Org.Apache.REEF.Tang.Annotations;
using Org.Apache.REEF.Utilities.Attributes;

namespace Org.Apache.REEF.Common.Telemetry
{
    [Unstable("0.16", "This is to build a collection of counters for evaluator metrics.")]
    [DefaultImplementation(typeof(MetricsImpl))]
    public interface IMetrics
    {
        /// <summary>
        /// Register a new counter with a specified name.
        /// If name does not exist, the counter will be added and true will be returned
        /// Otherwise the counter will be not added and false will be returned. 
        /// </summary>
        /// <param name="name">Name of the counter to be registered.</param>
        /// <param name="description">Description of the counter to be registered.</param>
        /// <returns>Returns a boolean to indicate if the counter is added.</returns>
        bool TryRegisterMetric(string name, string description);

        /// <summary>
        /// Get counter value for a given counter name
        /// </summary>
        /// <param name="name">Name of the counter</param>
        /// <param name="counter">The counter object returned</param>
        /// <returns>Returns a boolean to indicate if the value is found.</returns>
        bool TryGetValue(string name, out IMetricBase metric);

        /// <summary>
        /// Returns all the counters
        /// </summary>
        /// <returns></returns>
        IEnumerable<IMetricBase> GetMetrics();

        /// <summary>
        /// Serialize the metrics into strings
        /// </summary>
        /// <returns>Returns serialized string of the metrics.</returns>
        string Serialize();
    }
}