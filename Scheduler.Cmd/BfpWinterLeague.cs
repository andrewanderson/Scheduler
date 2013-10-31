using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler.Domain;
using Scheduler.GeneticAlgorithm.Rules;

namespace Scheduler.Cmd
{
    public static class BfpWinterLeague
    {
        public static League BfpLeague = new League { Duration = 20 };
        public static List<IRule> CustomRules = new List<IRule>(); 

        static BfpWinterLeague()
        {
            var field = new Field { Name = "Ben Franklin" };

            BfpLeague.Teams.Add(new Team { Name = "WWR1-Matt" });
            BfpLeague.Teams.Add(new Team { Name = "WWR2-Chris" });
            BfpLeague.Teams.Add(new Team { Name = "WWR3-George" });
            BfpLeague.Teams.Add(new Team { Name = "WWR4-Bill" });
            BfpLeague.Teams.Add(new Team { Name = "WWR5-Colin" });
            BfpLeague.Teams.Add(new Team { Name = "WWR6-Bridget" });
            BfpLeague.Teams.Add(new Team { Name = "WWR7-Josh" });
            BfpLeague.Teams.Add(new Team { Name = "WWR8-Andrew" });

            BfpLeague.GameSlots.Add(new GameSlot { Id = "6:00", Field = field, StartTime = "6:00" });
            BfpLeague.GameSlots.Add(new GameSlot { Id = "7:15", Field = field, StartTime = "7:15" });
            BfpLeague.GameSlots.Add(new GameSlot { Id = "8:30", Field = field, StartTime = "8:30" });
            BfpLeague.GameSlots.Add(new GameSlot { Id = "9:45", Field = field, StartTime = "9:45" });

            var week1 = new Schedule();
            week1.Games.Add(new Matchup { Home = BfpLeague.Teams[5], Away = BfpLeague.Teams[3], Slot = BfpLeague.GameSlots[0] }); // Bridget vs Bill @ 6:00
            week1.Games.Add(new Matchup { Home = BfpLeague.Teams[4], Away = BfpLeague.Teams[7], Slot = BfpLeague.GameSlots[1] }); // Colin vs Andrew @ 7:15
            week1.Games.Add(new Matchup { Home = BfpLeague.Teams[6], Away = BfpLeague.Teams[0], Slot = BfpLeague.GameSlots[2] }); // Josh vs Matt @ 8:30
            week1.Games.Add(new Matchup { Home = BfpLeague.Teams[1], Away = BfpLeague.Teams[2], Slot = BfpLeague.GameSlots[3] }); // Chris vs George @ 9:45
            
            CustomRules.AddRange(new List<IRule>
                {
                    new ExactWeekRule(week1, 0),
                    new TeamsNotInConsecutiveSlotsRule(BfpLeague.Teams[2], BfpLeague.Teams[7]),  /* Sophie and Erik not back-to-back */
                    new MatchupGameslotRule(BfpLeague.Teams[2], BfpLeague.Teams[7], new List<GameSlot> { BfpLeague.GameSlots[1], BfpLeague.GameSlots[2] }), /* Sophie and Erik in middle slots */
                });
        }

    }
}
