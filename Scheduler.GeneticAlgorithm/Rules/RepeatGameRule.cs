using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler.Domain;

namespace Scheduler.GeneticAlgorithm.Rules
{
    /// <summary>
    /// A rule that penalizes seasons where teams repeat games more often
    /// than if there was a perfectly even distribution.
    /// 
    /// TODO: define the scale of the penalty
    /// </summary>
    public class RepeatGameRule : IRule
    {
        public void Initialize(League league)
        {
            throw new NotImplementedException();
        }
                
        public int Apply(Season season)
        {
            throw new NotImplementedException();
        }
    }
}
