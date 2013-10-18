using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler.Domain;

namespace Scheduler.GeneticAlgorithm.Reproduction
{
    /// <summary>
    /// Rearrange all of the matchups within roughly a tenth of the weeks of the season
    /// </summary>
    public class MatchupShuffleMutation : IMutationAlgorithm
    {
        private const double shuffleChance = 0.1;

        private readonly List<Team> teams;
        private readonly List<GameSlot> slots;

        public MatchupShuffleMutation(League league)
        {
            this.teams = league.Teams;
            this.slots = league.GameSlots;
        }

        public void Mutate(Season season)
        {
            foreach (var week in season.Weeks)
            {
                if (RandomProvider.NextDouble() < shuffleChance)
                {
                    week.Games.Clear();
                    var shuffledTeams = FisherYatesShuffleAlgorithm.Shuffle(this.teams);
                    for (int i = 0; i < shuffledTeams.Count / 2; i++)
                    {
                        week.Games.Add(new Matchup { Home = shuffledTeams[i * 2], Away = shuffledTeams[i * 2 + 1], Slot = this.slots[i] });
                    }
                }
            }
        }
    }
}
