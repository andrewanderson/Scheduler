using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler.Domain;

namespace Scheduler.GeneticAlgorithm.Rules
{
    /// <summary>
    /// Provides a configurable bonus if a team is granted a specific game slot 
    /// on the desired week.
    /// </summary>
    public class SpecificGameslotRule : IRule
    {
        private const int Reward = 100;
        private readonly Team targetTeam;
        private readonly GameSlot targetSlot;
        private readonly int targetWeekIndex;

        public SpecificGameslotRule(Team team, GameSlot slot, int weekIndex)
        {
            this.targetTeam = team;
            this.targetSlot = slot;
            this.targetWeekIndex = weekIndex;
        }
        
        public void Initialize(League league)
        {
            if (targetWeekIndex >= league.Duration) throw new ArgumentException("Specified league can never satisfy rule", "league");
        }

        public int Apply(Season season)
        {
            var desiredWeek = season.Weeks[this.targetWeekIndex];
            foreach (var game in desiredWeek.Games)
            {
                if (game.Slot.Id == this.targetSlot.Id && (game.Home.Name == this.targetTeam.Name || game.Away.Name == this.targetTeam.Name)) 
                {
                    return Reward;
                }
                
            }
            return 0;
        }

        public List<RuleMessage> Report(Season season)
        {
            var messages = new List<RuleMessage>();

            if (this.Apply(season) > 0)
            {
                messages.Add(new RuleMessage(Reward, string.Format("{0} plays at {1} on week {2}", targetTeam.Name, targetSlot.StartTime, targetWeekIndex + 1)));
            }

            return messages;
        }
    }
}
