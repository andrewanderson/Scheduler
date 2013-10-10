using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler.Domain
{
    public class Season
    {
        public Season()
        {
            this.Weeks = new List<Schedule>();
        }

        public List<Schedule> Weeks { get; private set; }
    }
}
