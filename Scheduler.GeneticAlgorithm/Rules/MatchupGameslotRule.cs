﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler.Domain;

namespace Scheduler.GeneticAlgorithm.Rules
{
    /// <summary>
    /// Rewards schedules where games between a pair of teams occur
    /// during the specified timeslots.
    /// </summary>
    public class MatchupGameslotRule : IRule
    {
        private const int Reward = 20;

        private readonly Team team1;
        private readonly Team team2;
        private readonly List<GameSlot> desiredSlots;

        public MatchupGameslotRule(Team team1, Team team2, List<GameSlot> slots)
        {
            if (team1 == null) throw new ArgumentNullException("team1");
            if (team2 == null) throw new ArgumentNullException("team2");
            if (slots == null) throw new ArgumentNullException("slots");
            if (slots.Count == 0) throw new ArgumentException("Require at least one slot", "slots");

            this.team1 = team1;
            this.team2 = team2;
            this.desiredSlots = new List<GameSlot>(slots);
        }

        public void Initialize(League league)
        {
            // no-op
        }

        public int Apply(Season season)
        {
            return this.InnerApply(season, null);
        }

        private int InnerApply(Season season, List<RuleMessage> messages)
        {
            int totalReward = 0;

            for (int i = 0; i < season.Weeks.Count; i++)
            {
                var week = season.Weeks[i];
                foreach (var game in week.Games)
                {
                    if (this.desiredSlots.Where(s => s.Id == game.Slot.Id).Any()
                       && (game.Home.Name == this.team1.Name || game.Home.Name == this.team2.Name)
                       && (game.Away.Name == this.team1.Name || game.Away.Name == this.team2.Name))
                    {
                        totalReward += Reward;
                        if (messages != null) messages.Add(new RuleMessage(Reward, string.Format("{0} plays {1} on week {2} at {3}", this.team1.Name, this.team2.Name, i + 1, game.Slot.StartTime)));
                    }
                }
            }

            return totalReward;
        }

        public List<RuleMessage> Report(Season season)
        {
            var messages = new List<RuleMessage>();
            InnerApply(season, messages);
            return messages;
        }
    }
}
