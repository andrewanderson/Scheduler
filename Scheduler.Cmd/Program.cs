using System;
using System.Collections.Generic;
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
        public static void Main(string[] args)
        {
            var field = new Field { Name = "Ben Franklin" };

            var league = new League { Duration = 10 };
            league.Teams.Add(new Team { Name = "Team 1" });
            league.Teams.Add(new Team { Name = "Team 2" });
            league.Teams.Add(new Team { Name = "Team 3" });
            league.Teams.Add(new Team { Name = "Team 4" });
            league.Teams.Add(new Team { Name = "Team 5" });
            league.Teams.Add(new Team { Name = "Team 6" });
            league.Teams.Add(new Team { Name = "Team 7" });
            league.Teams.Add(new Team { Name = "Team 8" });

            league.GameSlots.Add(new GameSlot { Id = "6:00", Field = field, StartTime = "6:00" });
            league.GameSlots.Add(new GameSlot { Id = "7:15", Field = field, StartTime = "7:15" });
            league.GameSlots.Add(new GameSlot { Id = "8:30", Field = field, StartTime = "8:30" });
            league.GameSlots.Add(new GameSlot { Id = "9:45", Field = field, StartTime = "9:45" });

            var factory = new SeasonFactory(league);

            var rules = new List<IRule> {
                new ValidScheduleRule(),
                new GameslotAllocationRule(),
                new RepeatGameRule(),
                new GameSpacingRule(),
                new SpecificGameslotRule(league.Teams[0], league.GameSlots[0], 4),
                new TeamsInConsecutiveSlotsRule(league.Teams[1], league.Teams[2]),
                new MatchupGameslotRule(league.Teams[1], league.Teams[2], new List<GameSlot> { league.GameSlots[0], league.GameSlots[1] }),
            };

            var calc = new RuleBasedFitnessCalculator(league, rules);
            
            
            var season1 = factory.GenerateRandom();
            Console.Out.WriteLine("Parent 1");
            PrintSeason(season1);
            CalculateAndPrintFitness(season1, calc);

            var season2 = factory.GenerateRandom();
            Console.Out.WriteLine("Parent 2");
            PrintSeason(season2);
            CalculateAndPrintFitness(season2, calc);

            var mutate = new SinglePointCrossover();
            var children = mutate.Breed(season1, season2);

            Console.Out.WriteLine("Child 1");
            PrintSeason(children[0]);
            CalculateAndPrintFitness(children[0], calc);

            Console.Out.WriteLine("Child 2");
            PrintSeason(children[1]);
            CalculateAndPrintFitness(children[1], calc);
            
            Console.ReadKey();
                
        }

        private static void PrintSeason(Season season)
        {
            for (int i = 0; i < season.Weeks.Count; i++)
            {
                Console.Out.WriteLine("Week {0}", i + 1);
                var week = season.Weeks[i];
                foreach (var game in week.Games)
                {
                    Console.Out.WriteLine("{0} - {1} vs {2}", game.Slot.StartTime, game.Home.Name, game.Away.Name);
                }
                Console.Out.WriteLine(string.Empty);
            }
        }

        private static void CalculateAndPrintFitness(Season season, RuleBasedFitnessCalculator ruleCalculator)
        {
            var fitness = ruleCalculator.Calculate(season);
            Console.Out.WriteLine("Fitness: {0}", fitness);
            Console.Out.WriteLine(string.Empty);
        }

    }
}
