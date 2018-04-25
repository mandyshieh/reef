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

using System.Threading;
using Org.Apache.REEF.Common.Tasks;
using Org.Apache.REEF.Common.Telemetry;
using Org.Apache.REEF.Tang.Annotations;
using Org.Apache.REEF.Utilities.Logging;

namespace Org.Apache.REEF.Tests.Functional.Telemetry
{
    /// <summary>
    /// A test task that is to add counter information during the task execution.
    /// </summary>
    public class MetricsTask : ITask
    {
        private static readonly Logger Logger = Logger.GetLogger(typeof(MetricsTask));

        public const string TestMetric1 = "TestCounter";
        public const string TestMetric2 = "TestDouble";

        private readonly IMetrics _metrics;

        [Inject]
        private MetricsTask(IEvaluatorMetrics evaluatorMetrics)
        {
            _metrics = evaluatorMetrics.GetMetrics();
            _metrics.TryRegisterMetric(new Counter(TestMetric1, "This is " + TestMetric1));
            _metrics.TryRegisterMetric(new DoubleGauge(TestMetric2, "This is " + TestMetric2));
        }

        public byte[] Call(byte[] memento)
        {
            for (int i = 0; i < 100; i++)
            {
                _metrics.IncrementCounter(TestMetric1, 1);
                _metrics.TryGetValue(TestMetric2, out MetricBase me);
                me.Value = (double)me.Value + 2.0;
                Thread.Sleep(100);
            }
            return null;
        }

        public void Dispose()
        {
        }
    }
}