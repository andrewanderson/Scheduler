using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler.Domain;

namespace Scheduler.GeneticAlgorithm.Reproduction
{
    /// <summary>
    /// Mutate a Season by shuffling individual games within a random percentage of the Schedules
    /// </summary>
    public class GameShuffleMutation : IMutationAlgorithm
    {
        private const double shuffleChance = 0.5;

        public void Mutate(Season season)
        {
            foreach (var week in season.Weeks)
            {
                if (RandomProvider.NextDouble() <= shuffleChance)
                    FisherYatesShuffleAlgorithm.ShuffleDestructive(week.Games);
            }
        }

    }
}
