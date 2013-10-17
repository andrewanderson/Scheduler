using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler.GeneticAlgorithm
{
    /// <summary>
    /// Rearranges the elements in a list randomly.
    /// </summary>
    /// <remarks>
    /// http://www.dotnetperls.com/fisher-yates-shuffle
    /// </remarks>
    public class FisherYatesShuffleAlgorithm
    {
        public static void ShuffleDestructive<T>(List<T> listToShuffle)
        {
            for (int i = listToShuffle.Count; i > 1; i--)
            {
                // Pick random element to swap.
                int j = RandomProvider.Next(i); // 0 <= j <= i-1
                // Swap.
                T tmp = listToShuffle[j];
                listToShuffle[j] = listToShuffle[i - 1];
                listToShuffle[i - 1] = tmp;
            }
        }

        public static List<T> Shuffle<T>(List<T> listToShuffle)
        {
            var newList = new List<T>(listToShuffle);

            for (int i = newList.Count; i > 1; i--)
            {
                // Pick random element to swap.
                int j = RandomProvider.Next(i); // 0 <= j <= i-1
                // Swap.
                T tmp = newList[j];
                newList[j] = newList[i - 1];
                newList[i - 1] = tmp;
            }

            return newList;
        }

    }
}
