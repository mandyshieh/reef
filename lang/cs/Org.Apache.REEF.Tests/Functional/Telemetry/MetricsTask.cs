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

        public const string TestCounter1 = "TestCounter1";
        public const string TestCounter2 = "TestCounter2";

        private readonly IMetrics _metricSet;

        [Inject]
        private MetricsTask(IEvaluatorMetrics evaluatorMetrics)
        {
            _metricSet = evaluatorMetrics.GetMetrics();
            _metricSet.TryRegisterMetric(new Counter(TestCounter1, "This is " + TestCounter1));
            _metricSet.TryRegisterMetric(new Counter(TestCounter2, "This is " + TestCounter2));
        }

        public byte[] Call(byte[] memento)
        {
            for (int i = 0; i < 100; i++)
            {
                _metricSet.TryGetValue(TestCounter1, out IMetric me1);
                ((Counter)me1).Value += 1;
                _metricSet.TryGetValue(TestCounter2, out IMetric me2);
                ((Counter)me2).Value += 2;
                Thread.Sleep(100);
            }
            return null;
        }

        public void Dispose()
        {
        }
    }
}
