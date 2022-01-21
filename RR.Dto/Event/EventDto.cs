using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace RR.Dto
{

    public class EventDto
    {
        /// <summary>
        /// Event Id
        /// </summary>
        public int Id { get; set; }
        [JsonProperty("rid")]
        public string EventId { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("pbrid")]
        public string PBRID { get; set; }

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
        [JsonProperty("state")]
        public string State { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        [JsonProperty("event_type")]
        public string Type { get; set; }

        /// <summary>
        /// Season
        /// </summary>
        [JsonProperty("season")]
        public int Season { get; set; }

        /// <summary>
        /// Start Date
        /// </summary>
        [JsonProperty("startdate")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Perfect Time
        /// </summary>
        [JsonProperty("perftime")]
        public DateTime PerfTime { get; set; }

        /// <summary>
        /// Perfect Time UTC
        /// </summary>
        [JsonProperty("perftime_tz")]
        public DateTime PerfTimeUTC { get; set; }

        /// <summary>
        /// Sanction
        /// </summary>
        [JsonProperty("sanction")]
        public string Sanction { get; set; }

        /// <summary>
        /// Error
        /// </summary>
        [JsonProperty("error")]
        public string Error { get; set; }

        /// <summary>
        /// Time
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// Average
        /// </summary>
        public int Average { get; set; }

        /// <summary>
        /// Saves
        /// </summary>
        public int Saves { get; set; }

        /// <summary>
        /// YellowCard
        /// </summary>
        public int YellowCard { get; set; }

        /// <summary>
        ///  Is Active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        ///  Is Winning Distributed
        /// </summary>
        public bool WinningDistributed { get; set; }

        public int is_current { get; set; }
        public string result_count { get; set; }
        public string rid_status { get; set; }
        public string PerfTimeUTCString { get; set; }

        public int EventMode { get; set; }
    }

    /// <summary>
    /// Relate Current Event Dto with EventDto/EventDrawDto/EventRiderDto/EventBullDto
    /// </summary>
    public class CurrentEventDto
    {
        public CurrentEventDto()
        {
            EventRiderDto = new List<EventRidersDto>();
            EventBullDto = new List<EventBullsDto>();
        }

        [JsonProperty("event")]
        public EventDto EventDto { get; set; }

        [JsonProperty("eventriders")]
        public List<EventRidersDto> EventRiderDto { get; set; }

        [JsonProperty("eventbulls")]
        public List<EventBullsDto> EventBullDto { get; set; }

        [JsonProperty("draw")]
        public List<EventDrawDto> EventDrawDto { get; set; }
    }
}
