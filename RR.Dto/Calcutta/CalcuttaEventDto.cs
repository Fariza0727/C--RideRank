using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Dto.Calcutta
{
    public class CalcuttaEventDto
    {
        /// <summary>
        /// Event Id
        /// </summary>
        public int Id { get; set; }
        [JsonProperty("pevid")]
        public string ParentEventId { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// City
        /// </summary>
        [JsonProperty("city")]
        public string City { get; set; }

        /// <summary>
        /// State
        /// </summary>
        [JsonProperty("state")]
        public string State { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        [JsonProperty("contest_type")]
        public string ContestType { get; set; }

        /// <summary>
        /// Start Date
        /// </summary>
        [JsonProperty("startday")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// ContestUTCLockTime
        /// </summary>
        [JsonProperty("contest_utc_lock_time")]
        public DateTime ContestUTCLockTime { get; set; }

        /// <summary>
        /// Sanction
        /// </summary>
        [JsonProperty("contest_status")]
        public string ContestStatus { get; set; }

        /// <summary>
        /// EntryCount
        /// </summary>
        [JsonProperty("entry_count")]
        public int EntryCount { get; set; }

        /// <summary>
        /// Result Count
        /// </summary>
        [JsonProperty("res_count")]
        public int ResultCount { get; set; }

    }
}
