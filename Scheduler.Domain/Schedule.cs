using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler.Domain
{
    public class Schedule
    {
        public Schedule()
        {
            this.Games = new List<Matchup>();
        }

        public DateTime Date { get; set; }
        public List<Matchup> Games { get; private set; }
    }
}
