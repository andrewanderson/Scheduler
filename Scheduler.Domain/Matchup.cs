using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler.Domain
{
    public class Matchup
    {
        public GameSlot Slot { get; set; }
        public Team Home { get; set; }
        public Team Away { get; set; }
    }
}
