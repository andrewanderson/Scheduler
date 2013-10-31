using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler.Domain;
using Scheduler.GeneticAlgorithm;
using Scheduler.GeneticAlgorithm.Reproduction;
using Scheduler.GeneticAlgorithm.Rules;

namespace Scheduler.Cmd
{
    public class Program
    {
        private const int PopulationSize = 1500;
        private const int DefaultGenerationIncrement = 10000;
        private const bool AutoSave = true;
        private const int AutoSaveIncrement = 50;

        private static PrimordialSoup GeneticAlgorithm = null;
        private static string RunId = null;
        private static bool IsVerboseMode = false;

        // CHANGE THESE LINES TO SELECT A DIFFERENT LEAGUE!
        private static League league = BfpWinterLeague.BfpLeague;
        private static List<IRule> customRules = BfpWinterLeague.CustomRules;

        public static void Main(string[] args)
        {
            if (args.Where(a => a.Equals("-v")).Any())
                IsVerboseMode = true;

            GeneticAlgorithm = new PrimordialSoup(league, customRules, PopulationSize);
            GeneticAlgorithm.Initialize();

            GeneticAlgorithm.GenerationComplete += geneticAlgorithm_GenerationComplete;
            
            char key = '?';
            while (key != 'q') 
            {
                if (key == 's' || (key == 'r' && AutoSave == true && RunId == null))
                {
                    if (RunId == null)
                    {
                        Console.Out.WriteLine("Choose a name for this run (letters & numbers only)");
                        Console.Out.Write(">>> ");
                        RunId = Console.ReadLine();
                    }

                    if (key != 'r')
                        SaveState(GeneticAlgorithm, RunId, league);
                }

                if (key == 'r')
                {
                    GeneticAlgorithm.Run(DefaultGenerationIncrement);

                    var topSeason = GeneticAlgorithm.CurrentPopulation.First();
                    Console.Out.WriteLine();
                    Console.Out.WriteLine("Top Season");
                    Console.Out.WriteLine("----------");
                    Console.Out.WriteLine();
                    PrintSeason(topSeason, Console.Out);
                }

                Console.Out.WriteLine();
                Console.Out.WriteLine("Pick: (r)un {0} generations, (s)ave to a file, (q)uit", DefaultGenerationIncrement);
                Console.Out.Write(">>> ");

                key = Console.ReadKey().KeyChar;
                Console.Out.WriteLine();
            }
        }

        static void geneticAlgorithm_GenerationComplete(object sender, EventArgs e)
        {
            var gea = e as GenerationEventArgs;
            Console.Out.WriteLine("Generation {0} Completed - top fitness: {1}", gea.Generation, gea.MostFitSeason.Fitness);

            if (AutoSave && gea.Generation % AutoSaveIncrement == 0)
            {
                SaveState(GeneticAlgorithm, RunId, league);
            }
        }

        private static void SaveState(PrimordialSoup geneticAlgorithm, string runId, League league)
        {
            // Gather a list of all Seasons
            var seasons = new Dictionary<string, SeasonReport>();
            foreach (var season in geneticAlgorithm.CurrentPopulation)
            {
                string thumbprint = Thumbprint(season);

                if (!seasons.ContainsKey(thumbprint))
                {
                    var sr = new SeasonReport { Index = seasons.Count + 1, Occurences = 1, Season = season, Thumbprint = thumbprint };
                    seasons.Add(thumbprint, sr);
                }
                else
                {
                    var sr = seasons[thumbprint];
                    sr.Occurences++;
                }
            }
            var sortedSRs = seasons.Values.OrderBy(x => x.Index).ToList();
            
            // Write out a summary of the diversity to the console
            Console.Out.WriteLine("Current Diversity: {0} unique, {1} max, {2} min", sortedSRs.Count, sortedSRs.First().Season.Fitness, sortedSRs.Last().Season.Fitness);

            // Directory created based on the run ID
            if (!Directory.Exists(runId))
            {
                Directory.CreateDirectory(runId);
            }

            // Verbose output
            if (IsVerboseMode)
            {
                string fileName = Path.Combine(runId, "generation" + geneticAlgorithm.CurrentGeneration + "_verbose.txt");
                using (var writer = File.CreateText(fileName))
                {
                    writer.WriteLine("Report for generation {0}", geneticAlgorithm.CurrentGeneration);
                    writer.WriteLine("======================================");
                    writer.WriteLine();

                    // Now print them all out
                    foreach (var sr in sortedSRs)
                    {
                        PrintSessionWithStats(writer, sr, geneticAlgorithm.CurrentPopulation.Count);
                    }
                }
            }

            string advancedStatsPath = Path.Combine(runId, "generation" + geneticAlgorithm.CurrentGeneration + "_" + geneticAlgorithm.CurrentPopulation.First().Fitness + ".txt");
            using (var writer = File.CreateText(advancedStatsPath))
            {
                writer.WriteLine("Top 10 seasons for generation {0}", geneticAlgorithm.CurrentGeneration);
                writer.WriteLine("======================================");
                writer.WriteLine();

                foreach (var sr in seasons.Values.OrderBy(x => x.Index).Take(10))
                {
                    PrintAdvancedSeasonStats(writer, sr, geneticAlgorithm, league);
                }
            }
        }

        private static void PrintSeason(Season season, TextWriter destination)
        {
            for (int i = 0; i < season.Weeks.Count; i++)
            {
                destination.WriteLine("Week {0}", i + 1);
                var week = season.Weeks[i];
                foreach (var game in week.Games.OrderBy(g => g.Slot.Id))
                {
                    destination.WriteLine("{0} - {1} vs {2}", game.Slot.StartTime, game.Home.Name, game.Away.Name);
                }
                destination.WriteLine(string.Empty);
            }
            destination.WriteLine(string.Empty);
        }

        private static void PrintSessionWithStats(TextWriter writer, SeasonReport sr, int populationSize)
        {
            var percentOfTotal = ((double)sr.Occurences / (double)populationSize) * 100.0;

            writer.WriteLine("=============================================");
            writer.WriteLine("Season #{0} - {1} fitness,  {2}%", sr.Index, sr.Season.Fitness, Math.Round(percentOfTotal, 4));
            writer.WriteLine("=============================================");
            PrintSeason(sr.Season, writer);
        }

        private static void PrintAdvancedSeasonStats(TextWriter writer, SeasonReport sr, PrimordialSoup ga, League league)
        {
            PrintSessionWithStats(writer, sr, ga.CurrentPopulation.Count);

            var slotAllocation = new Dictionary<string, Dictionary<string, int>>();
            var opponentProgression = new Dictionary<string, StringBuilder>();
            var opponentDistribution = new Dictionary<string, Dictionary<string, int>>();
            foreach (var team in league.Teams)
            {
                slotAllocation.Add(team.Name, new Dictionary<string, int>());
                opponentProgression.Add(team.Name, new StringBuilder());
                opponentDistribution.Add(team.Name, new Dictionary<string, int>());
                foreach (var slot in league.GameSlots)
                {
                    slotAllocation[team.Name].Add(slot.Id, 0);
                }
                foreach (var opponent in league.Teams.Where(t => t.Name != team.Name))
                {
                    opponentDistribution[team.Name].Add(opponent.Name, 0);
                }
            }
            foreach (var week in sr.Season.Weeks)
            {
                foreach (var game in week.Games)
                {
                    slotAllocation[game.Home.Name][game.Slot.Id]++;
                    slotAllocation[game.Away.Name][game.Slot.Id]++;

                    opponentProgression[game.Home.Name].Append(game.Away.Name + "->");
                    opponentProgression[game.Away.Name].Append(game.Home.Name + "->");

                    opponentDistribution[game.Home.Name][game.Away.Name]++;
                    opponentDistribution[game.Away.Name][game.Home.Name]++;
                }
            }

            writer.WriteLine("Thumbprint: {0}", Thumbprint(sr.Season));
            writer.WriteLine();
            writer.WriteLine("Game slot allocation");
            writer.WriteLine("--------------------");
            foreach (var team in league.Teams)
            {
                writer.Write("{0}: ", team.Name);
                foreach (var slot in league.GameSlots)
                {
                    writer.Write("{0} - {1}, ", slot.StartTime, slotAllocation[team.Name][slot.Id]);    
                }
                writer.WriteLine();
            }
            writer.WriteLine();
            writer.WriteLine("Opponent distribution");
            writer.WriteLine("---------------------");
            foreach (var team in league.Teams)
            {
                writer.Write("{0}: ", team.Name);
                foreach (var opponent in league.Teams.Where(t => t.Name != team.Name))
                {
                    writer.Write("{0} - {1}, ", opponent.Name, opponentDistribution[team.Name][opponent.Name].ToString());
                }
                writer.WriteLine();
            }
            writer.WriteLine();
            writer.WriteLine("Opponent progression");
            writer.WriteLine("--------------------");
            foreach (var team in league.Teams)
            {
                writer.WriteLine("{0}: {1}", team.Name, opponentProgression[team.Name].ToString());
            }
            writer.WriteLine();
            writer.WriteLine("Rule scoring");
            writer.WriteLine("------------");
            foreach (var rule in ga.Rules)
            {
                int score = rule.Apply(sr.Season);
                writer.WriteLine("{0}: {1}", rule.GetType().Name, score);
            }
            writer.WriteLine();
            writer.WriteLine("Rule analysis");
            writer.WriteLine("------------");
            foreach (var rule in ga.Rules)
            {
                var messages = rule.Report(sr.Season);
                writer.WriteLine("{0}", rule.GetType().Name);
                if (messages.Count == 0)
                {
                    writer.WriteLine("       No penalties or bonuses");
                }
                foreach (var message in messages)
                {
                    writer.WriteLine("       {0} - {1}", message.Points, message.Message);
                }
            }
            writer.WriteLine();
        }

        private static Dictionary<string, string> teamKeys = null;
        private static string Thumbprint(Season season)
        {
            if (teamKeys == null)
            {
                teamKeys = new Dictionary<string, string>();
                var week = season.Weeks.First();
                int key = 1;
                foreach (var game in week.Games)
                {
                    teamKeys.Add(game.Home.Name, key.ToString());
                    key++;
                    teamKeys.Add(game.Away.Name, key.ToString());
                    key++;
                }
            }

            var sb = new StringBuilder();
            foreach (var week in season.Weeks)
            {
                foreach (var game in week.Games)
                {
                    // normalize the order of the games.  A vs B == B vs A (for now)
                    string homeKey = teamKeys[game.Home.Name];
                    string awayKey = teamKeys[game.Away.Name];

                    if (homeKey.CompareTo(awayKey) > 0)
                    {
                        sb.Append(homeKey);
                        sb.Append(awayKey);
                    }
                    else
                    {
                        sb.Append(awayKey);
                        sb.Append(homeKey);
                    }
                }
            }
            return sb.ToString();
        }
    }
}
