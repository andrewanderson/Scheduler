using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler.GeneticAlgorithm
{
    public static class ListExtensions
    {
        public static List<int> FindIndecies<T>(this List<T> list, T value)
        {
            var indecies = new List<int>();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Equals(value)) indecies.Add(i);
            }
            return indecies;
        }
    }
}
