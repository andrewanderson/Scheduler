using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler.Domain;

namespace Scheduler.GeneticAlgorithm.Reproduction
{
    /// <summary>
    /// Rearrange all of the matchups within a single week of the season
    /// </summary>
    public class MatchupShuffleMutation : IMutationAlgorithm
    {
        private readonly List<Team> teams;
        private readonly List<GameSlot> slots;

        public MatchupShuffleMutation(League league)
        {
            this.teams = league.Teams;
            this.slots = league.GameSlots;
        }

        public void Mutate(Season season)
        {
            int weekToShuffle = RandomProvider.Next(season.Weeks.Count);

            var week = season.Weeks[weekToShuffle];
            week.Games.Clear();
            
            var shuffledTeams = FisherYatesShuffleAlgorithm.Shuffle(this.teams);
            for (int i = 0; i < shuffledTeams.Count / 2; i++)
            {
                week.Games.Add(new Matchup { Home = shuffledTeams[i * 2], Away = shuffledTeams[i * 2 + 1], Slot = this.slots[i] });
            }
        }
    }
}
