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
        public static void Main(string[] args)
        {
            var field = new Field { Name = "Ben Franklin" };

            var league = new League { Duration = 20 };
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

            var customRules = new List<IRule> {
                new SpecificGameslotRule(league.Teams[0], league.GameSlots[0], 4),
                new TeamsInConsecutiveSlotsRule(league.Teams[1], league.Teams[2]),
                new MatchupGameslotRule(league.Teams[1], league.Teams[2], new List<GameSlot> { league.GameSlots[0], league.GameSlots[1] }),
            };

            var geneticAlgorithm = new PrimordialSoup(league, customRules, 5000);
            geneticAlgorithm.Initialize();

            geneticAlgorithm.GenerationComplete += geneticAlgorithm_GenerationComplete;
            
            char key = '?';
            while (key != 'q') 
            {
                if (key == 'r')
                {
                    geneticAlgorithm.Run(10000);

                    var topSeason = geneticAlgorithm.CurrentPopulation.First();
                    Console.Out.WriteLine();
                    Console.Out.WriteLine("Top Season");
                    Console.Out.WriteLine("----------");
                    Console.Out.WriteLine();
                    PrintSeason(topSeason, Console.Out);
                }
                else if (key == 's')
                {

                }

                Console.Out.WriteLine();
                Console.Out.WriteLine("Pick: (r)un 10000 generations, (s)ave to a file, (q)uit");
                Console.Out.Write(">>> ");

                key = Console.ReadKey().KeyChar;
                Console.Out.WriteLine();
            }
        }

        static void geneticAlgorithm_GenerationComplete(object sender, EventArgs e)
        {
            var gea = e as GenerationEventArgs;
            Console.Out.WriteLine("Generation {0} Completed - top fitness: {1}", gea.Generation, gea.MostFitSeason.Fitness);
        }

        private static void PrintSeason(Season season, TextWriter destination) 
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
            Console.Out.WriteLine("Fitness: {0}", season.Fitness);
            Console.Out.WriteLine(string.Empty);
        }

    }
}
