using Newtonsoft.Json;

namespace RR.Dto
{
     public class EventBullsDto
     {
          /// <summary>
          /// Event Id
          /// </summary>
          [JsonProperty("event_id")]
          public string EventId { get; set; }

          public int Id { get; set; }

          /// <summary>
          /// Bull Id
          /// </summary>
          [JsonProperty("pbid")]
          public int BullId { get; set; }

          /// <summary>
          /// Event Tier
          /// </summary>
          [JsonProperty("event_tier")]
          public int EventTier { get; set; }

          /// <summary>
          /// Tier Score
          /// </summary>
          [JsonProperty("tier_score")]
          public decimal TierScore { get; set; }

          /// <summary>
          /// BullDto
          /// </summary>
          public BullDto BullDto { get; set; }

          /// <summary>
          /// RiderDto
          /// </summary>
          public RiderDto RiderDto { get; set; }

          /// <summary>
          /// Error
          /// </summary>
          [JsonProperty("error")]
          public string Error { get; set; }
     }
}
