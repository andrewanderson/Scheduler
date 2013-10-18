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
    public class GameShuffleMutation : IMutationAlgorithm
    {       
        public void Mutate(Season season)
        {
            int weekToShuffle = RandomProvider.Next(season.Weeks.Count);

            var week = season.Weeks[weekToShuffle];
            FisherYatesShuffleAlgorithm.ShuffleDestructive(week.Games);
        }

    }
}
