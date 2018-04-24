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
using Org.Apache.REEF.Common.Telemetry;
using Org.Apache.REEF.Tang.Implementations.Tang;
using Xunit;

namespace Org.Apache.REEF.Common.Tests.Telemetry
{
    public class CounterTests
    {
        /// <summary>
        /// Test ICounters and IEvaluatorMetrics API.
        /// </summary>
        [Fact]
        public void TestEvaluatorMetrics()
        {
            var evalMetrics = TangFactory.GetTang().NewInjector().GetInstance<IEvaluatorMetrics>();
            var metrics = evalMetrics.GetMetrics();
            metrics.TryRegisterMetric(new Counter("counter1", "counter1 description"));
            metrics.TryRegisterMetric(new Counter("counter2", "counter2 description"));
            
            //// ValidateCounter(metrics, "counter1", 0);
            //// ValidateCounter(metrics, "counter2", 0);

            metrics.IncrementCounter("counter1", 3);
            metrics.IncrementCounter("counter1", 1);
            metrics.IncrementCounter("counter2", 2);
            metrics.IncrementCounter("counter2", 3);

            //// ValidateCounter(metrics, "counter1", 4);
            //// ValidateCounter(metrics, "counter2", 5);
            var counterStr = evalMetrics.Serialize();

            var metrics2 = new EvaluatorMetrics(counterStr);
            var counters2 = metrics2.GetMetrics();
            ValidateCounter(counters2, "counter1", 4);
            ValidateCounter(counters2, "counter2", 5);
        }

        /// <summary>
        /// Test TryRegisterCounter with a duplicated counter name
        /// </summary>
        [Fact]
        public void TestDuplicatedCounters()
        {
            var counters = CreateMetrics();
            counters.TryRegisterMetric(new Counter("counter1", "counter1 description"));
            Assert.False(counters.TryRegisterMetric(new Counter("counter1", "counter1 description")));
        }

        /// <summary>
        /// Test Increment for a non-registered counter.
        /// </summary>
        [Fact]
        public void TestNoExistCounter()
        {
            var metricSet = CreateMetrics();

            // Action increment = () => metricSet.Increment("counter1", 2);
            // Assert.Throws<ApplicationException>(increment);
        }

        private static void ValidateCounter(IMetrics metricSet, string name, int expectedValue)
        {
            metricSet.TryGetValue(name, out MetricBase c1);
            Assert.Equal(c1.Type, MetricType.Counter);
            Assert.Equal(expectedValue, Convert.ToInt32(c1.Value));
        }

        private static IMetrics CreateMetrics()
        {
            var m = TangFactory.GetTang().NewInjector().GetInstance<IEvaluatorMetrics>();
            var c = m.GetMetrics();
            return c;
        }
    }
}
