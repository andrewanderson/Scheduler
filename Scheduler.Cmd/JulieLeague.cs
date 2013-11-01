using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler.Domain;
using Scheduler.GeneticAlgorithm.Rules;

namespace Scheduler.Cmd
{
    public class JulieLeague
    {
        public static League TheLeague = new League { Duration = 4 };
        public static List<IRule> CustomRules = new List<IRule>();

        static JulieLeague()
        {
            var field1 = new Field { Name = "Field1" };
            var field2 = new Field { Name = "Field2" };

            TheLeague.Teams.Add(new Team { Name = "01" });
            TheLeague.Teams.Add(new Team { Name = "02" });
            TheLeague.Teams.Add(new Team { Name = "03" });
            TheLeague.Teams.Add(new Team { Name = "04" });
            TheLeague.Teams.Add(new Team { Name = "05" });
            TheLeague.Teams.Add(new Team { Name = "06" });
            TheLeague.Teams.Add(new Team { Name = "07" });
            TheLeague.Teams.Add(new Team { Name = "08" });
            TheLeague.Teams.Add(new Team { Name = "09" });
            TheLeague.Teams.Add(new Team { Name = "10" });
            TheLeague.Teams.Add(new Team { Name = "11" });
            TheLeague.Teams.Add(new Team { Name = "12" });

            TheLeague.GameSlots.Add(new GameSlot { Id = "F1_8:15", Field = field1, StartTime = "20:15" });
            TheLeague.GameSlots.Add(new GameSlot { Id = "F2_8:15", Field = field2, StartTime = "20:15" });

            TheLeague.GameSlots.Add(new GameSlot { Id = "F1_9:30", Field = field1, StartTime = "21:30" });
            TheLeague.GameSlots.Add(new GameSlot { Id = "F2_9:30", Field = field2, StartTime = "21:30" });

            TheLeague.GameSlots.Add(new GameSlot { Id = "F1_10:45", Field = field1, StartTime = "22:45" });
            TheLeague.GameSlots.Add(new GameSlot { Id = "F2_10:45", Field = field2, StartTime = "22:45" });

        }
    }
}
