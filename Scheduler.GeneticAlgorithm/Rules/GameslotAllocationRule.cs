using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler.Domain;

namespace Scheduler.GeneticAlgorithm.Rules
{
    /// <summary>
    /// A rule that penalizes seasons where teams have an uneven distribution
    /// of game slots.
    /// 
    /// 5 points per distance from the optimum value(s) are deducted per team per game slot.
    /// e.g. a slot distribution of 5/6/5/4 (distance: 2) is less penalize than one of 10/2/3/5 (distance: 10)
    /// </summary>
    public class GameslotAllocationRule : IRule
    {
        private const int PenaltyMultiplier = -5;

        private int minimumOptimalGames = 0;
        private int maximumOptimalGames = 0;
        private readonly List<string> teamNames = new List<string>();
        private readonly List<string> slotIds = new List<string>();

        public void Initialize(League league)
        {
            // Calculate the min/max games in a given slot that a team should play.
            // Examples: 
            // 20 game season, 4 slots: each team should play 5 games per slot
            // 15 game season, 4 slots: each team should play 3 games in one slot, and 4 games in the other 3 slots
            // 13 game season, 4 slots: each team should play 3 games in three slots, and 4 games in the other slot
            this.minimumOptimalGames = (int)Math.Floor((double)league.Duration / (double)league.GameSlots.Count);
            this.maximumOptimalGames = (league.Duration % league.GameSlots.Count == 0) ? minimumOptimalGames : minimumOptimalGames + 1;

            // Store the teams & game slots for quick access later
            this.teamNames.AddRange(league.Teams.Select(t => t.Name));
            this.slotIds.AddRange(league.GameSlots.Select(s => s.Id));
        }
          
        public int Apply(Season season)
        {
            return this.InnerApply(season, null);
        }

        private int InnerApply(Season season, List<RuleMessage> messages) 
        {
            int cumulativeDistance = 0;

            // Pre-seed the mapping structure now so that we can avoid "contains" checks all through the code.
            var teamSlots = CreateMappingDictionary();

            // Sum up the game slot distribution
            foreach (var week in season.Weeks)
            {
                foreach (var game in week.Games)
                {
                    teamSlots[game.Home.Name][game.Slot.Id] = teamSlots[game.Home.Name][game.Slot.Id] + 1;
                    teamSlots[game.Away.Name][game.Slot.Id] = teamSlots[game.Away.Name][game.Slot.Id] + 1;
                }
            }

            // Calculate the penalty
            foreach (var team in teamSlots.Keys)
            {
                var teamDict = teamSlots[team];
                foreach (var slotKey in teamDict.Keys)
                {
                    var numGamesInSlot = teamDict[slotKey];
                    int penalty = 0;
                    if (numGamesInSlot < this.minimumOptimalGames)
                    {
                        penalty = this.minimumOptimalGames - numGamesInSlot;
                    }
                    else if (numGamesInSlot > this.maximumOptimalGames)
                    {
                        penalty = numGamesInSlot - this.maximumOptimalGames;
                    }

                    if (penalty != 0)
                    {
                        cumulativeDistance += penalty;
                        if (messages != null) messages.Add(new RuleMessage(penalty * PenaltyMultiplier, string.Format("{0} has {1} games in slot {2}", team, numGamesInSlot, slotKey)));
                    }
                }
            }

            return cumulativeDistance * PenaltyMultiplier;
        }

        private Dictionary<string, Dictionary<string, int>> CreateMappingDictionary()
        {
            var teamSlots = new Dictionary<string, Dictionary<string, int>>();
            foreach (var team in this.teamNames)
            {
                var slotDict = new Dictionary<string, int>();
                teamSlots.Add(team, slotDict);
                foreach (var slot in this.slotIds)
                {
                    slotDict.Add(slot, 0);
                }
            }

            return teamSlots;
        }


        public List<RuleMessage> Report(Season season)
        {
            var messages = new List<RuleMessage>();
            InnerApply(season, messages);
            return messages;
        }
    }
}
