using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler.Domain;

namespace Scheduler.GeneticAlgorithm
{
    public class GenerationEventArgs : EventArgs
    {
        public int Generation { get; private set; }
        public Season MostFitSeason { get; private set; }

        public GenerationEventArgs(int generation, Season mostFitSeason)
        {
            this.Generation = generation;
            this.MostFitSeason = mostFitSeason;
        }
    }
}
