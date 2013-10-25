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
        private const int Reward = 5000;

        public void Initialize(League league)
        {
            // no-op
        }

        public int Apply(Season season)
        {
            return season.Weeks.All(s => this.IsValid(s)) ? Reward : 0;
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
        
        public List<RuleMessage> Report(Season season)
        {
            var messages = new List<RuleMessage>();

            if (this.Apply(season) > 0)
            {
                messages.Add(new RuleMessage(Reward, "Schedule is valid."));
            }

            return messages;
        }
    }
}
