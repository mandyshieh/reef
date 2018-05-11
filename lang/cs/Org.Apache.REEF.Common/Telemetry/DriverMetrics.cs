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

namespace Org.Apache.REEF.Common.Telemetry
{
    /// <summary>
    /// A simple driver metrics.
    /// It contains system state for now.
    /// It can be extended later to include more driver metrics data.
    /// </summary>
    public sealed class DriverMetrics : IDriverMetrics
    {
        private string _stateMetricName = "DriverState";

        public IMetric SystemState
        {
            get;
        }

        public DriverMetrics(string systemState, DateTime timeUpdated)
        {
            SystemState = new DriverSystemState(_stateMetricName, "driver state.", timeUpdated.Ticks, systemState);
        }
    }
}
