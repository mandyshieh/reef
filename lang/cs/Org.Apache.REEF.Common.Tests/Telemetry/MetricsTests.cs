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
            var evalMetrics = TangFactory.GetTang().NewInjector().GetInstance<IEvaluatorMetrics>();
            var metrics = evalMetrics.GetMetrics();
            metrics.TryRegisterMetric(new Counter("counter1", "counter1 description"));
            metrics.TryRegisterMetric(new Counter("counter2", "counter2 description"));
            ValidateMetric(metrics, "counter1", MetricType.Counter, 0);
            ValidateMetric(metrics, "counter2", MetricType.Counter, 0);

            metrics.IncrementCounter("counter1", 3);
            metrics.IncrementCounter("counter1", 1);
            metrics.IncrementCounter("counter2", 2);
            metrics.IncrementCounter("counter2", 3);
            ValidateMetric(metrics, "counter1", MetricType.Counter, 4);
            ValidateMetric(metrics, "counter2", MetricType.Counter, 5);
            var counterStr = evalMetrics.Serialize();

            var metrics2 = new EvaluatorMetrics(counterStr);
            var counters2 = metrics2.GetMetrics();
            ValidateMetric(counters2, "counter1", MetricType.Counter, 4);
            ValidateMetric(counters2, "counter2", MetricType.Counter, 5);
        }

        [Fact]
        public void TestEvaluatorMetricsMixed()
        {
            var evalMetrics = TangFactory.GetTang().NewInjector().GetInstance<IEvaluatorMetrics>();
            var metrics = evalMetrics.GetMetrics();
            metrics.TryRegisterMetric(new Counter("counter", "counter description"));
            metrics.TryRegisterMetric(new DoubleGauge("double", "double description", DateTime.Now.Ticks, 3.14));

            ValidateMetric(metrics, "counter", MetricType.Counter, 0);
            ValidateMetric(metrics, "double", MetricType.Double, 3.14);

            metrics.IncrementCounter("counter", 3);
            metrics.IncrementCounter("counter", 1);
            ValidateMetric(metrics, "counter", MetricType.Counter, 4);
            metrics.TryGetValue("double", out MetricBase doubleMetric);
            doubleMetric.Value = 4.13;
            ValidateMetric(metrics, "double", MetricType.Double, 4.13);

            var metricsStr = evalMetrics.Serialize();
            var evalMetrics2 = new EvaluatorMetrics(metricsStr);
            var metrics2 = evalMetrics2.GetMetrics();
            ValidateMetric(metrics2, "counter", MetricType.Counter, Convert.ToInt32(4));
            ValidateMetric(metrics2, "double", MetricType.Double, 4.13);
        }

        /// <summary>
        /// Tests updating metric value.
        /// </summary>
        [Fact]
        public void TestMetricSetValue()
        {
            var metrics = CreateMetrics();
            metrics.TryRegisterMetric(new DoubleGauge("dou1", "metric of type double", DateTime.Now.Ticks, default(double)));
            metrics.TryRegisterMetric(new DoubleGauge("dou2", "metric of type double", DateTime.Now.Ticks, default(double)));
            ValidateMetric(metrics, "dou1", MetricType.Double, default(double));
            ValidateMetric(metrics, "dou2", MetricType.Double, default(double));

            metrics.TryGetValue("dou1", out MetricBase me);
            me.Value = 3.14;
            ((MetricsImpl)metrics).SetDoubleToPi("dou2");
            ValidateMetric(metrics, "dou1", MetricType.Double, 3.14);
            ValidateMetric(metrics, "dou2", MetricType.Double, Math.PI);
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

        /// <summary>
        /// Test Increment for a non-registered counter.
        /// </summary>
        [Fact]
        public void TestNoExistCounter()
        {
            var metricSet = CreateMetrics();

            Action increment = () => metricSet.IncrementCounter("counter1", 2);
            Assert.Throws<ApplicationException>(increment);
        }

        private static void ValidateMetric(IMetrics metricSet, string name, MetricType expectedType, object expectedValue)
        {
            metricSet.TryGetValue(name, out MetricBase metric);
            Assert.Equal(metric.Type, expectedType);
            Assert.Equal(expectedValue, metric.Value);
        }

        private static IMetrics CreateMetrics()
        {
            var m = TangFactory.GetTang().NewInjector().GetInstance<IEvaluatorMetrics>();
            var c = m.GetMetrics();
            return c;
        }
    }
}
