using System;
using System.Collections.Generic;
using System.Text;

namespace RR.Dto.Event
{
    public class EventResultDto
    {
        /// <summary>
        /// Event Id
        /// </summary>
        public string rid { get; set; }
        /// <summary>
        /// Rider Name
        /// </summary>
        public string rider_name { get; set; }
        /// <summary>
        /// Rider Id
        /// </summary>
        public int rider_id { get; set; }
        /// <summary>
        /// Bull Id
        /// </summary>
        public int pbid { get; set; }
        /// <summary>
        /// Rider Score
        /// </summary>
        public decimal ride_score { get; set; }
        /// <summary>
        /// Bull Score
        /// </summary>
        public decimal bull_score { get; set; }
        /// <summary>
        /// Time
        /// </summary>
        public string time { get; set; }
        /// <summary>
        /// Round
        /// </summary>
        public string round { get; set; }
        /// <summary>
        /// Comment
        /// </summary>
        public string comments { get; set; }
        /// <summary>
        /// qualified ride
        /// </summary>
        public string qr { get; set; }
        /// <summary>
        /// Is Out
        /// </summary>
        public string isout { get; set; }
        /// <summary>
        /// did out result in RR option
        /// </summary>
        public string isrr { get; set; }
        /// <summary>
        /// prp
        /// </summary>
        public decimal? prp { get; set; }
        /// <summary>
        /// prp_conf
        /// </summary>
        public string? prp_conf { get; set; }
        /// <summary>
        /// Rider tier
        /// </summary>
        public string rider_tier { get; set; }
        /// <summary>
        /// Rider tier score
        /// </summary>
        public string rider_tier_score { get; set; }
        /// <summary>
        /// Bull tier
        /// </summary>
        public string bull_tier { get; set; }
        /// <summary>
        /// R1 bull tier
        /// </summary>
        public string r1_bull_tier { get; set; }
        /// <summary>
        /// Bull tier score
        /// </summary>
        public string bull_tier_score { get; set; }
        /// <summary>
        /// RR rider score
        /// </summary>
        public decimal rr_rider_score { get; set; }
        /// <summary>
        /// RR bull score
        /// </summary>
        public decimal rr_bull_score { get; set; }
        /// <summary>
        /// Mid (internal matchup ID in PBS)
        /// </summary>
        public string mid { get; set; }

        public string rider_avatar { get; set; }
        public string bull_avatar { get; set; }
        public string bull_name { get; set; }
    }
}
