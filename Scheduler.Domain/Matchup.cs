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

        /// <summary>
        /// Make a copy of this Schedule
        /// </summary>
        public Matchup ShallowCopy()
        {
            return new Matchup { Home = this.Home, Away = this.Away, Slot = this.Slot };
        }

        public override string ToString()
        {
            return string.Format("{0} - {1} vs {2}", this.Slot, this.Home, this.Away);
        }
    }
}
