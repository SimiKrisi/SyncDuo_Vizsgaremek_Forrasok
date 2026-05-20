using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncDuoAdmin
{
    public class Users
    {
        public string user_id { get; set; }
        public string game_id { get; set; }
        public string google_id { get; set; }    
        public string display_name { get; set; }
        public string created_at { get; set; }
        public string last_login { get; set; }
        public int status { get; set; }
    }
}
