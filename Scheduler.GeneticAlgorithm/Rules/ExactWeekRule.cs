using Scheduler.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler.GeneticAlgorithm.Rules
{
    /// <summary>
    /// A rule that applies a hefty bonus for matching a week exactly.
    /// 
    /// This rule is meant to be used when one week is already set in stone.
    /// </summary>
    public class ExactWeekRule : IRule
    {
        private const int Reward = 1000;
        private readonly Schedule targetSchedule;
        private readonly int targetWeekIndex;

        public ExactWeekRule(Schedule targetSchedule, int weekIndex)
        {
            this.targetSchedule = targetSchedule;
            this.targetWeekIndex = weekIndex;
        }
        
        public void Initialize(League league)
        {
            if (targetWeekIndex >= league.Duration) throw new ArgumentException("Specified league can never satisfy rule", "league");
        }

        public int Apply(Season season)
        {
            var scheduledWeek = season.Weeks[this.targetWeekIndex];
            foreach (var scheduledGame in scheduledWeek.Games)
            {
                // Match the week to the target, ignoring home/away
                var targetGame = this.targetSchedule.Games.FirstOrDefault(g => g.Slot.Id == scheduledGame.Slot.Id);
                if (targetGame == null) return 0;
                if (targetGame.Home.Name != scheduledGame.Home.Name || targetGame.Home.Name != scheduledGame.Away.Name) return 0;
                if (targetGame.Away.Name != scheduledGame.Home.Name || targetGame.Away.Name != scheduledGame.Away.Name) return 0;               
            }
            return Reward;
        }

        public List<RuleMessage> Report(Season season)
        {
            var messages = new List<RuleMessage>();

            if (this.Apply(season) > 0)
            {
                messages.Add(new RuleMessage(Reward, string.Format("Week {0} matches the desired schedule", targetWeekIndex + 1)));
            }

            return messages;
        }
    }
}
