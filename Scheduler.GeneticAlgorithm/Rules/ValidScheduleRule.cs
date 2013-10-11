using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler.Domain;

namespace Scheduler.GeneticAlgorithm.Rules
{
    /// <summary>
    /// A rule that penalizes any season that contains one or more invalid schedules.
    /// 
    /// An invalid schedule is on in which a team is scheduled to play twice.
    /// </summary>
    public class ValidScheduleRule : IRule
    {
        private const int Penalty = -1000;

        public void Initialize(League league)
        {
            // no-op
        }

        public int Apply(Season season)
        {
            foreach (var schedule in season.Weeks)
            {
                if (!IsValid(schedule)) return Penalty;
            }

            return 0;
        }

        private bool IsValid(Schedule schedule)
        {
            var seenTeams = new HashSet<string>();
            foreach (var game in schedule.Games)
            {
                if (seenTeams.Contains(game.Home.Name)) return false;
                seenTeams.Add(game.Home.Name);

                if (seenTeams.Contains(game.Away.Name)) return false;
                seenTeams.Add(game.Away.Name);
            }

            return true;
        }
    }
}
