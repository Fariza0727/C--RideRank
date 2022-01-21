using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Dto.Calcutta
{
    public class CalcuttaEventClassDto
    {
        /// <summary>
        /// Event Id
        /// </summary>
        public int Id { get; set; }
        [JsonProperty("evid")]
        public string EventId { get; set; }

        /// <summary>
        /// Class
        /// </summary>
        [JsonProperty("evclass")]
        public string EventClass { get; set; }

        /// <summary>
        /// City
        /// </summary>
        [JsonProperty("evtype")]
        public string EventType { get; set; }

        /// <summary>
        /// Labvel
        /// </summary>
        [JsonProperty("evlabel")]
        public string EventLabel { get; set; }

        /// <summary>
        /// Fees
        /// </summary>
        [JsonProperty("fees")]
        public decimal Fees { get; set; }

        /// <summary>
        /// Saction
        /// </summary>
        [JsonProperty("sanction")]
        public string Sanction { get; set; }

        /// <summary>
        /// Start Date
        /// </summary>
        [JsonProperty("startdate")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Class UTC Lock Time
        /// </summary>
        [JsonProperty("class_utc_lock_time")]
        public DateTime ClassUTCLockTime { get; set; }


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

        public bool IsCompleted { get; set; }

    }
    public class CalcuttaEventClassesDto
    {

        [JsonProperty("pevid")]
        public string ParentEventId { get; set; }

        [JsonProperty("classes")]
        public List<CalcuttaEventClassDto> CalcuttaEventClassList { get; set; }

    }
}
