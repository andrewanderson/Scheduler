using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler.Domain;

namespace Scheduler.GeneticAlgorithm.Rules
{
    /// <summary>
    /// A heuristic that can be applied to a schedule to measure its fitness.
    /// </summary>
    public interface IRule
    {
        int Apply(Schedule schedule);
    }
}
