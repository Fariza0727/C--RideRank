using Newtonsoft.Json;

namespace RR.Dto
{
     public class EventRidersDto
     {
          /// <summary>
          /// Event Id
          /// </summary>
          [JsonProperty("event_id")]
          public string EventId { get; set; }

          /// <summary>
          /// Rider Id
          /// </summary>
          [JsonProperty("guyid")]
          public int RiderId { get; set; }

          /// <summary>
          /// Cwrp Bonus
          /// </summary>
          [JsonProperty("cwrp_bonus")]
          public decimal CwrpBonus { get; set; }

          /// <summary>
          /// Event Tier Score
          /// </summary>
          [JsonProperty("event_tier_score")]
          public decimal EventTierScore { get; set; }

          /// <summary>
          /// Event Tier
          /// </summary>
          [JsonProperty("event_tier")]
          public int EventTier { get; set; }

          /// <summary>
          /// Event Tier Rank
          /// </summary>
          [JsonProperty("event_tier_rank")]
          public int EventTierRank { get; set; }

          /// <summary>
          /// Error
          /// </summary>
          [JsonProperty("error")]
          public string Error { get; set; }

     }
}
