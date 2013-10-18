using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler.Domain;

namespace Scheduler.GeneticAlgorithm.Reproduction
{
    /// <summary>
    /// Mutate a Season by shuffling the position of one or more Schedules.
    /// </summary>
    public class ScheduleShuffleMutation : IMutationAlgorithm
    {
        private const int MaximumShuffles = 3;

        public void Mutate(Season season)
        {
            int shuffles = RandomProvider.Next(MaximumShuffles + 1); // could be 0
            for (int i = 0; i < shuffles; i++)
            {
                // Perform a swap
                int x = RandomProvider.Next(season.Weeks.Count);
                int y = RandomProvider.Next(season.Weeks.Count);

                var temp = season.Weeks[x];
                season.Weeks[x] = season.Weeks[y];
                season.Weeks[y] = temp;
            }
        }
    }
}
