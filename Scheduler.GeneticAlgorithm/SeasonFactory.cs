using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler.Domain;

namespace Scheduler.GeneticAlgorithm
{
    public class SeasonFactory
    {
        private readonly League league;

        public SeasonFactory(League league)
        {
            // Sanity checks on the shape of the league
            if (league.Teams.Count % 2 > 0) throw new ArgumentException("League must contain an even number of teams.");
            if (league.Teams.Count / 2 != league.GameSlots.Count) throw new ArgumentException("League must contain enough game slots to host all teams.");

            this.league = league;
        }

        public Season GenerateRandom()
        {
            var season = new Season();
            for (int i = 0; i < this.league.Duration; i++)
            {
                season.Weeks.Add(this.GenerateSchedule());
            }

            return season;
        }

        /// <summary>
        /// Generate a random schedule from the list of teams in a league.
        /// 
        /// All teams are guaranteed to play exactly once per week.
        /// </summary>
        private Schedule GenerateSchedule()
        {
            var teams = FisherYatesShuffleAlgorithm.Shuffle(this.league.Teams);

            var schedule = new Schedule();
            for (int i = 0; i < (teams.Count / 2); i++)
            {
                schedule.Games.Add(new Matchup { Home = teams[i * 2], Away = teams[i * 2 + 1], Slot = this.league.GameSlots[i] });
            }

            return schedule;
        }

    }
}
