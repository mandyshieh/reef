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
    /// Base implementation of a metric object. All metrics of a specific type should derive from this base class.
    /// </summary>
    /// <typeparam name="T">Metric type.</typeparam>
    abstract class MetricBase<T> : IMetric<T>
    {
        protected T _typedValue;

        protected long _timestamp;

        public string Name { get; }

        public string Description { get; }

        public virtual long Timestamp
        {
            get { return _timestamp; }
        }

        public virtual object ValueUntyped
        {
            get { return _typedValue; }
        }

        public virtual T Value
        {
            get
            {
                return _typedValue;
            }
        }

        public abstract bool IsImmutable { get; }

        public MetricBase(string name, string description)
        {
            Name = name;
            Description = description;
            _timestamp = DateTime.Now.Ticks;
            _typedValue = default(T);
        }

        public MetricBase(string name, string description, long timeStamp, T value)
        {
            Name = name;
            Description = description;
            _timestamp = timeStamp;
            _typedValue = value;
        }

        public abstract IMetric CreateInstanceWithNewValue(object val);
    }
}
