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
using Org.Apache.REEF.Common.Telemetry;
using Org.Apache.REEF.Tang.Implementations.Tang;
using Xunit;

namespace Org.Apache.REEF.Common.Tests.Telemetry
{
    public class MetricsTests
    {
        /// <summary>
        /// Test IMetrics, ICounters and IEvaluatorMetrics API.
        /// </summary>
        [Fact]
        public void TestEvaluatorMetricsCountersOnly()
        {
            var evalMetrics1 = TangFactory.GetTang().NewInjector().GetInstance<IEvaluatorMetrics>();
            var metrics1 = evalMetrics1.GetMetrics();
            metrics1.TryRegisterMetric(new Counter("counter1", "counter1 description"));
            metrics1.TryRegisterMetric(new Counter("counter2", "counter2 description"));
            ValidateMetric(metrics1, "counter1", 0);
            ValidateMetric(metrics1, "counter2", 0);
            metrics1.TryGetValue("counter1", out IMetric me);
            var co1 = (Counter)me;
            co1.Increment();
            co1.Increment(3);
            co1.Decrement(2);
            Assert.Equal(me.ValueUntyped, 2);
            var counterStr = metrics1.Serialize();

            var evalMetrics2 = new EvaluatorMetrics(counterStr);
            var metrics2 = evalMetrics2.GetMetrics();
            ValidateMetric(metrics2, "counter1", 2);
            ValidateMetric(metrics2, "counter2", 0);
        }

        /// <summary>
        /// Tests updating metric value.
        /// </summary>
        [Fact]
        public void TestMetricSetValue()
        {
            var metrics = CreateMetrics();
            metrics.TryRegisterMetric(new DoubleGauge("dou1", "metric of type double", DateTime.Now.Ticks, 0));
            metrics.TryRegisterMetric(new DoubleGauge("dou2", "metric of type double", DateTime.Now.Ticks, 0));
            ValidateMetric(metrics, "dou1", default(double));
            ValidateMetric(metrics, "dou2", default(double));

            metrics.TryGetValue("dou1", out IMetric me);
            ((DoubleGauge)me).Value = 3.14;

            ValidateMetric(metrics, "dou1", 3.14);
            ValidateMetric(metrics, "dou2", 0);
        }

        /// <summary>
        /// Test TryRegisterCounter with a duplicated counter name
        /// </summary>
        [Fact]
        public void TestDuplicatedCounters()
        {
            var counters = CreateMetrics();
            counters.TryRegisterMetric(new Counter("metric1", "metric description"));
            Assert.False(counters.TryRegisterMetric(new DoubleGauge("metric1", "duplicate name")));
        }

        private static void ValidateMetric(IMetrics metricSet, string name, object expectedValue)
        {
            metricSet.TryGetValue(name, out IMetric metric);
            Assert.Equal(expectedValue, metric.ValueUntyped);
        }

        private static IMetrics CreateMetrics()
        {
            var m = TangFactory.GetTang().NewInjector().GetInstance<IEvaluatorMetrics>();
            var c = m.GetMetrics();
            return c;
        }
    }
}
