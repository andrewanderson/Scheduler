using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler.Domain;

namespace Scheduler.GeneticAlgorithm.Reproduction
{
    /// <summary>
    /// Cross two Seasons at a single point to create two new offspring.
    /// </summary>
    public class SinglePointCrossover : ICrossoverAlgorithm
    {
        public List<Season> Breed(Season parent1, Season parent2)
        {
            // child1 will inherit all weeks before and including this point from parent1, and all weeks after this point from parent2
            // child2 will inherit the exact opposite set.
            int crossIndex = RandomProvider.Next(parent1.Weeks.Count);

            var child1 = new Season();
            var child2 = new Season();
            for (int i = 0; i < parent1.Weeks.Count; i++)
            {
                if (i <= crossIndex)
                {
                    child1.Weeks.Add(parent1.Weeks[i]);
                    child2.Weeks.Add(parent2.Weeks[i]);
                }
                else
                {
                    child1.Weeks.Add(parent2.Weeks[i]);
                    child2.Weeks.Add(parent1.Weeks[i]);
                }
            }

            return new List<Season> { child1, child2 };
        }
    }
}
