using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler.Domain
{
    public class GameSlot
    {
        public string Id { get; set; }
        public Field Field { get; set; }
        public string StartTime { get; set; }

        public override string ToString()
        {
            return string.Format("{0}@{1}", StartTime, Field);
        }
    }
}
