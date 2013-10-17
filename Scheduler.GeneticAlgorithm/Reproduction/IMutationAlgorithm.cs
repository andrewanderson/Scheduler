using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler.Domain;

namespace Scheduler.GeneticAlgorithm.Reproduction
{
    public interface IMutationAlgorithm
    {
        Season Mutate(Season originalSeason);
    }
}
