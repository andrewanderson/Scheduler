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

        /// <summary>
        /// The score assigned to show how relatively desirable this schedule is based on a
        /// calculation provided by the consumer of this class.
        /// </summary>
        public int Fitness { get; set; }
    }
}
