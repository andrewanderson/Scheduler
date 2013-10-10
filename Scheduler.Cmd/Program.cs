using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler.Domain;
using Scheduler.GeneticAlgorithm;

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

            league.GameSlots.Add(new GameSlot { Field = field, StartTime = "6:00" });
            league.GameSlots.Add(new GameSlot { Field = field, StartTime = "7:15" });
            league.GameSlots.Add(new GameSlot { Field = field, StartTime = "8:30" });
            league.GameSlots.Add(new GameSlot { Field = field, StartTime = "9:45" });

            var factory = new SeasonFactory(league);
            var season = factory.GenerateRandom();

            for (int i = 0; i < season.Weeks.Count; i++)
            {
                Console.Out.WriteLine("Week {0}", i);
                var week = season.Weeks[i];
                foreach (var game in week.Games)
                {
                    Console.Out.WriteLine("{0} - {1} vs {2}", game.Slot.StartTime, game.Home.Name, game.Away.Name);
                }
                Console.Out.WriteLine(string.Empty);
            }

            Console.ReadKey();
                
        }
    }
}
