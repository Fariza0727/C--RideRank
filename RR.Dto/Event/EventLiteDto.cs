using Newtonsoft.Json;
using System;

namespace RR.Dto
{

    public class EventLiteDto
    {
        /// <summary>
        /// Event Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        [JsonProperty("event_title")]
        public string Title { get; set; }

        /// <summary>
        /// Loaction
        /// </summary>
        [JsonProperty("location")]
        public string Location { get; set; }

        /// <summary>
        /// City
        /// </summary>
        [JsonProperty("city")]
        public string City { get; set; }

        /// <summary>
        /// State
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Perfect Time
        /// </summary>
        [JsonProperty("perftime")]
        public DateTime PerfTime { get; set; }

        /// <summary>
        ///  Is Active
        /// </summary>
        public bool IsActive { get; set; }

        public string Type { get; set; }

        public string PBRID { get; set; }
    }
}
