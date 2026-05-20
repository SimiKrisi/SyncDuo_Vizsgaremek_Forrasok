using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncDuoAdmin
{
    public class Stats
    {
        public string user_id { get; set; }
        public int coins { get; set; }
        public int best_speedrun_amount { get; set; }
        public double best_dailyc_time { get; set; }
        public int levels_completed { get; set; }
    }
}
