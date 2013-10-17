using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler.Domain;

namespace Scheduler.GeneticAlgorithm.Reproduction
{
    /// <summary>
    /// Mutate a Season by shuffling individual games within a single Schedule
    /// </summary>
    public class GameShuffleMutation
    {       
        public Season Mutate(Season originalSeason)
        {
            var newSeason = originalSeason.DeepCopy();

            int weekToShuffle = RandomProvider.Next(newSeason.Weeks.Count);

            var week = newSeason.Weeks[weekToShuffle];
            FisherYatesShuffleAlgorithm.ShuffleDestructive(week.Games);

            return newSeason;
        }

    }
}
