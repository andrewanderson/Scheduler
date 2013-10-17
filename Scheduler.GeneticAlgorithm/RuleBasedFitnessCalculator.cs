using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler.Domain;
using Scheduler.GeneticAlgorithm.Rules;

namespace Scheduler.GeneticAlgorithm
{
    /// <summary>
    /// Determines the fitness of a Season by using a configurable set of rules.
    /// 
    /// Zero is the lowest fitness that a Season can have under this algorithm.
    /// </summary>
    public class RuleBasedFitnessCalculator
    {
        private readonly List<IRule> rules = new List<IRule>();

        public RuleBasedFitnessCalculator(League league, IEnumerable<IRule> rules)
        {
            if (league == null) throw new ArgumentNullException("league");
            if (rules == null) throw new ArgumentNullException("rules");

            this.rules.AddRange(rules);
            this.rules.ForEach(r => r.Initialize(league));
        }
        
        public int Calculate(Season season)
        {
            return Math.Max(0, rules.Sum(r => r.Apply(season)));
        }
    }
}
