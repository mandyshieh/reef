using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Org.Apache.REEF.Common.Telemetry.Interfaces
{
    interface IDoubleGauge : IMetric<double>
    {
        /// <summary>
        /// Increments the gauge by 1.
        /// </summary>
        void Increment();

        /// <summary>
        /// Increments the gauge by delta.
        /// </summary>
        /// <param name="delta">Value with which to increment.</param>
        void Increment(double delta);

        /// <summary>
        /// Decrements the gauge by 1.
        /// </summary>
        void Decrement();

        /// <summary>
        /// Decrements the gauge by delta.
        /// </summary>
        /// <param name="delta">Value with which to increment.</param>
        void Decrement(double delta);

        /// <summary>
        /// Resets the gauge.
        /// </summary>
        /// <param name="value">Value to which the gauge should be reset.</param>
        void Reset(double value);
    }
}
