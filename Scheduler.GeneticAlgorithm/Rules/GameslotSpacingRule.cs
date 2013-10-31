using Scheduler.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler.GeneticAlgorithm.Rules
{
    /// <summary>
    /// A rule that penalizes seasons where teams play in the same game slot in close proximity.
    /// </summary>
    public class GameslotSpacingRule : IRule
    {
        private int optimialMinimumSpaceBetweenSlots = 0;
        private readonly List<string> teamNames = new List<string>();
        private readonly List<string> slotIds = new List<string>();
        private readonly List<int> penaltyMatrix = new List<int>();

        public void Initialize(League league)
        {
            this.optimialMinimumSpaceBetweenSlots = league.GameSlots.Count;

            // Store the teams and slots for quick access later
            this.teamNames.AddRange(league.Teams.Select(t => t.Name));
            this.slotIds.AddRange(league.GameSlots.Select(s => s.Id));

            // Penalize close repetitions harsher than those that are nearly optimal
            for (int i = 0; i < this.optimialMinimumSpaceBetweenSlots; i++)
            {
                int p = Math.Max(0, i - 1);
                this.penaltyMatrix.Add(p*p*2*-1);
            }

            // For an 4 slot league, the penalty matrix will be:
            // 0, 0, 2, 8
        }

        public int Apply(Season season)
        {
            return this.InnerApply(season, null);
        }

        private int InnerApply(Season season, List<RuleMessage> messages)
        {
            int totalPenalty = 0;

            // Store an ordered list of slots for each team.
            var slotDictionary = new Dictionary<string, List<string>>();
            this.teamNames.ForEach(t => slotDictionary.Add(t, new List<string>()));

            foreach (var week in season.Weeks)
            {
                foreach (var game in week.Games)
                {
                    slotDictionary[game.Home.Name].Add(game.Slot.Id);
                    slotDictionary[game.Away.Name].Add(game.Slot.Id);
                }
            }

            // Now for each team, penalize repetitions that happen too early based on the penalty matrix
            var alreadyProcessed = new List<string>(); // the teams we've already dealt with
            foreach (var team in slotDictionary.Keys)
            {
                var slotSchedule = slotDictionary[team];
                foreach (var slot in this.slotIds)
                {
                    var indecies = slotSchedule.FindIndecies(slot);
                    for (int i = 0; i < indecies.Count - 1; i++)
                    {
                        int penaltyIndex = this.optimialMinimumSpaceBetweenSlots - (indecies[i + 1] - indecies[i]);
                        if (penaltyIndex >= 0 && penaltyIndex < this.penaltyMatrix.Count)
                        {
                            totalPenalty += this.penaltyMatrix[penaltyIndex];
                            if (messages != null && this.penaltyMatrix[penaltyIndex] != 0) messages.Add(new RuleMessage(this.penaltyMatrix[penaltyIndex], string.Format("{0} plays at {1} on week {2} and then week {3}", team, slot, indecies[i] + 1, indecies[i + 1] + 1)));
                        }
                    }
                }

                alreadyProcessed.Add(team);
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
