using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler.Domain;

namespace Scheduler.GeneticAlgorithm.Rules
{
    /// <summary>
    /// A heuristic that can be applied to a season to measure one part of its fitness.
    /// </summary>
    public interface IRule
    {
        /// <summary>
        /// Perform any start-up calculations that are needed when applying this rule.
        /// </summary>
        /// <remarks>
        /// - Should be called exactly once.
        /// - Added for performance reasons.
        /// </remarks>
        void Initialize(League league);

        /// <summary>
        /// Calculate the component of the Season's fitness that applies to this rule.
        /// </summary>
        int Apply(Season season);
    }
}
