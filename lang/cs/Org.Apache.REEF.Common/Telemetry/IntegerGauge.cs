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
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Org.Apache.REEF.Utilities.Attributes;

namespace Org.Apache.REEF.Common.Telemetry
{
    class IntegerGauge : MetricBase<int>
    {
        public override bool IsImmutable
        {
            get { return true; }
        }

        public IntegerGauge(string name, string description)
            : base(name, description)
        {
        }

        [JsonConstructor]
        internal IntegerGauge(string name, string description, long timeStamp, int value)
            : base(name, description, timeStamp, value)
        {
        }

        public override void Update(IMetric me)
        {
            _typedValue = Convert.ToInt32(me.ValueUntyped);
            _timestamp = DateTime.Now.Ticks;
        }

        public override void Update(object val)
        {
            _typedValue = Convert.ToInt32(val);
            _timestamp = DateTime.Now.Ticks;
        }

        public override IMetric Copy()
        {
            return new IntegerGauge(Name, Description, _timestamp, _typedValue);
        }
    }
}
