using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler.Domain
{
    public class League
    {
        public League()
        {
            this.Teams = new List<Team>();
            this.GameSlots = new List<GameSlot>();
        }

        public int Duration { get; set; }
        public List<Team> Teams { get; private set; }
        public List<GameSlot> GameSlots { get; private set; }
    }
}
