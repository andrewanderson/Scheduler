using Scheduler.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler.GeneticAlgorithm.Rules
{
    /// <summary>
    /// A rule that penalizes seasons where teams play in the same game slot back-to-back
    /// </summary>
    public class GameslotSpacingRule : IRule
    {
        private const int PenaltyPerInfraction = -10;
        
        private readonly List<string> teamNames = new List<string>();
        private readonly List<string> fieldTimes = new List<string>();
        
        public void Initialize(League league)
        {
            // Store the teams and slots for quick access later
            this.teamNames.AddRange(league.Teams.Select(t => t.Name));
            this.fieldTimes.AddRange(league.GameSlots.Select(s => s.StartTime).Distinct());
        }

        public int Apply(Season season)
        {
            return this.InnerApply(season, null);
        }

        private int InnerApply(Season season, List<RuleMessage> messages)
        {
            int totalPenalty = 0;

            // Store an ordered list of slots for each team.
            var fieldTimeDictionary = new Dictionary<string, List<string>>();
            this.teamNames.ForEach(t => fieldTimeDictionary.Add(t, new List<string>()));

            foreach (var week in season.Weeks)
            {
                foreach (var game in week.Games)
                {
                    fieldTimeDictionary[game.Home.Name].Add(game.Slot.StartTime);
                    fieldTimeDictionary[game.Away.Name].Add(game.Slot.StartTime);
                }
            }

            // Now for each team, penalize repetitions that happen too early based on the penalty matrix
            foreach (var team in fieldTimeDictionary.Keys)
            {
                var slotSchedule = fieldTimeDictionary[team];
                foreach (var fieldTime in this.fieldTimes)
                {
                    var indecies = slotSchedule.FindIndecies(fieldTime);
                    for (int i = 0; i < indecies.Count - 1; i++)
                    {
                        int weekDifference = (indecies[i + 1] - indecies[i]);
                        if (weekDifference == 1)
                        {
                            totalPenalty += PenaltyPerInfraction;
                            if (messages != null) messages.Add(new RuleMessage(PenaltyPerInfraction, string.Format("{0} plays at {1} on week {2} and then week {3}", team, fieldTime, indecies[i] + 1, indecies[i + 1] + 1)));
                        }
                    }
                }
            }

            return totalPenalty;
        }

        public List<RuleMessage> Report(Season season)
        {
            var messages = new List<RuleMessage>();
            InnerApply(season, messages);
            return messages;
        }
    }
}
