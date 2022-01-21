using Newtonsoft.Json;

namespace RR.Dto
{
     public class BullLiteDto
     {
          /// <summary>
          /// Bull Id
          /// </summary>
          [JsonProperty("pbid")]
          public int Id { get; set; }

          /// <summary>
          /// BullId
          /// </summary>
          public int BullId { get; set; }

          /// <summary>
          /// Bull Name
          /// </summary>
          [JsonProperty("bull_name")]
          public string Name { get; set; }

          /// <summary>
          /// Owner Name of Bull
          /// </summary>
          [JsonProperty("owner")]
          public string Owner { get; set; }

          /// <summary>
          ///  Historical Rank
          /// </summary>
          [JsonProperty("historical_rank")]
          public int HistoricalRank { get; set; }

          /// <summary>
          /// Mounted
          /// </summary>
          [JsonProperty("mounted")]
          public int Mounted { get; set; }

          /// <summary>
          /// Power Rating
          /// </summary>
          [JsonProperty("power_rating")]
          public decimal PowerRating { get; set; }

          /// <summary>
          /// BuckOff Percentage
          /// </summary>
          [JsonProperty("buckoff_perc")]
          public decimal BuckOffPerc { get; set; }

        /// <summary>
        /// Aatar
        /// </summary>
        [JsonProperty("avatar")]
        public string Aatar { get; set; }
        /// <summary>
        /// Is Active
        /// </summary>
        public bool isActive { get; set; }
     }
}
