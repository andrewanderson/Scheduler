using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler.GeneticAlgorithm.Rules
{
    public class RuleMessage
    {
        public string Message { get; private set; }
        public int Points { get; private set; }

        public RuleMessage(int points, string message)
        {
            this.Points = points;
            this.Message = message;
        }
    }
}
