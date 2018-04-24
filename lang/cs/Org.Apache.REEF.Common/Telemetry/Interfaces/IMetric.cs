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
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Org.Apache.REEF.Utilities.Attributes;

namespace Org.Apache.REEF.Common.Telemetry
{
    public class MetricBase
    {
        protected string _name;
        protected string _description;
        protected long _timestamp;
        protected object _value;
        protected MetricType _type = MetricType.NA;

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
        }

        public long Timestamp
        {
            get
            {
                return _timestamp;
            }
        }

        public virtual object Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                _timestamp = DateTime.Now.Ticks;
            }
        }

        public MetricType Type
        {
            get
            {
                return _type;
            }
        }

        public MetricBase(string name, string description)
        {
            _name = name;
            _description = description;
            _timestamp = DateTime.Now.Ticks;
        }

        [JsonConstructor]
        public MetricBase(string name, string description, object value, long timeStamp, MetricType type)
        {
            _name = name;
            _description = description;
            _value = value;
            _timestamp = timeStamp;
            _type = type;

            switch (type)
            {
                case MetricType.Counter:
                case MetricType.Integer: _value = Convert.ToInt32(_value);
                    break;
                case MetricType.Double: _value = Convert.ToDouble(_value);
                    break;
                case MetricType.Long: _value = Convert.ToInt64(_value);
                    break;
                default: _value = value;
                    break;
            }
        }
    }

    public enum MetricType
    {
        Counter,
        Integer,
        Double,
        Long,
        NA
    }
}
