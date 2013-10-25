using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler.Domain;

namespace Scheduler.GeneticAlgorithm.Rules
{
    /// <summary>
    /// A rule that penalizes seasons where teams repeat games against opponents more often
    /// than if there was a perfectly even distribution.
    /// 
    /// 20 points per team are deducted for teams playing more/less than an even
    /// number of games against a given opponent.
    /// </summary>
    public class RepeatGameRule : IRule
    {
        private const int PenaltyPerInfraction = -10; // Will lose -10 for the each side of the match-up being out of the desired range, so -20 over all.

        private int minimumOptimalGamesPerOpponent = 0;
        private int maximumOptimalGamesPerOpponent = 0;
        private readonly List<string> teamNames = new List<string>();

        public void Initialize(League league)
        {
            // Calculate the min/max games against a given opponent that a team should play.
            // Examples: 
            // 21 game season, 8 teams: each team should play each other team 3 times
            // 20 game season, 8 teams: each team should play at least 2 games against every opponent, and at most 3 games (against 6 opponents).
            // 14 game season, 4 teams: each team should play 4 against every opponent, and at most 5 games (against 2 opponents)
            this.minimumOptimalGamesPerOpponent = (int)Math.Floor((double)league.Duration / (double)(league.Teams.Count - 1));
            this.maximumOptimalGamesPerOpponent = minimumOptimalGamesPerOpponent + 1;

            // Store the teams for quick access later
            this.teamNames.AddRange(league.Teams.Select(t => t.Name));
        }
                
        public int Apply(Season season)
        {
            return this.InnerApply(season, null);
        }

        private int InnerApply(Season season, List<RuleMessage> messages)
        {
            int totalPenalty = 0;

            // Pre-seed the mapping structure now so that we can avoid "contains" checks all through the code.
            var teamOpponents = CreateMappingDictionary();

            // Sum up the opponent distribution
            foreach (var week in season.Weeks)
            {
                foreach (var game in week.Games)
                {
                    teamOpponents[game.Home.Name][game.Away.Name] = teamOpponents[game.Home.Name][game.Away.Name] + 1;
                    teamOpponents[game.Away.Name][game.Home.Name] = teamOpponents[game.Away.Name][game.Home.Name] + 1;
                }
            }

            // Calculate the penalty
            foreach (var team in teamOpponents.Keys)
            {
                var opponentDict = teamOpponents[team];
                foreach (var opponent in opponentDict.Keys)
                {
                    var numGamesAgainstOpponent = opponentDict[opponent];
                    if (numGamesAgainstOpponent < this.minimumOptimalGamesPerOpponent || numGamesAgainstOpponent > this.maximumOptimalGamesPerOpponent)
                    {
                        totalPenalty += PenaltyPerInfraction;
                        if (messages != null) messages.Add(new RuleMessage(PenaltyPerInfraction, string.Format("{0} plays {1} games against {2} during the season", team, numGamesAgainstOpponent, opponent)));
                    }
                }
            }

            return totalPenalty;
        }

        private Dictionary<string, Dictionary<string, int>> CreateMappingDictionary()
        {
            var teamOpponents = new Dictionary<string, Dictionary<string, int>>();
            foreach (var team1 in this.teamNames)
            {
                var opponentDict = new Dictionary<string, int>();
                teamOpponents.Add(team1, opponentDict);
                foreach (var team2 in this.teamNames)
                {
                    if (team1 != team2)
                        opponentDict.Add(team2, 0);
                }
            }

            return teamOpponents;
        }


        public List<RuleMessage> Report(Season season)
        {
            var messages = new List<RuleMessage>();
            InnerApply(season, messages);
            return messages;
        }
    }
}
