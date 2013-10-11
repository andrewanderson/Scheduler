using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler.Domain;

namespace Scheduler.GeneticAlgorithm.Rules
{
    /// <summary>
    /// A rule that penalizes seasons where teams repeat games prior to playing 
    /// all other opponents.
    /// 
    /// TODO: define the scale of the penalty
    /// </summary>
    public class GameSpacingRule : IRule
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
