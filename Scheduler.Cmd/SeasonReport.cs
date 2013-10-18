using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler.Domain;

namespace Scheduler.Cmd
{
    public class SeasonReport
    {
        public Season Season { get; set; }
        public int Occurences { get; set; }
        public string Thumbprint { get; set; }
        public int Index { get; set; }
    }
}
