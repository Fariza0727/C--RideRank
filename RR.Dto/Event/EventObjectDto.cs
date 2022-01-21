using System;
using System.Collections.Generic;
using System.Text;

namespace RR.Dto
{
    public class EventObjectDto
    {
        public string rid { get; set; }
        public string pbrid { get; set; }
        public string event_title { get; set; }
        public string location { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string event_type { get; set; }
        public string season { get; set; }
        public string startdate { get; set; }
        public string perftime { get; set; }
        public int is_current { get; set; }
        public string result_count { get; set; }
        public string rid_status { get; set; }
    }

}
