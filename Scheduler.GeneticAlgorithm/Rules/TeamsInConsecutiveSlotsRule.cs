using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler.Domain;

namespace Scheduler.GeneticAlgorithm.Rules
{
    /// <summary>
    /// Rewards schedules where the specified teams play in consecutive game slots (when not
    /// playing against each other).
    /// 
    /// There is a bonus on 10 points per week that meets this rule.
    /// </summary>
    public class TeamsInConsecutiveSlotsRule : IRule
    {
        private const int Reward = 5;
        private readonly Team team1;
        private readonly Team team2;

        public TeamsInConsecutiveSlotsRule(Team team1, Team team2)
        {
            this.team1 = team1;
            this.team2 = team2;
        }

        public void Initialize(League league)
        {
            // no-op
        }

        public int Apply(Season season)
        {
            int totalReward = 0;

            foreach (var week in season.Weeks)
            {
                int i = this.IndexOfTeam(week, this.team1);
                int j = this.IndexOfTeam(week, this.team2);

                if (Math.Abs(i - j) == 1)
                {
                    totalReward += Reward;
                }
            }

            return totalReward;
        }

        private int IndexOfTeam(Schedule week, Team team)
        {
            for (int i = 0; i < week.Games.Count; i++)
            {
                var matchup = week.Games[i];
                if (matchup.Home.Name == team.Name || matchup.Away.Name == team.Name)
                {
                    return i;
                }
            }
            return 0; // wtf
        }

    }
}
