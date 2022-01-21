using Newtonsoft.Json;
using System;

namespace RR.Dto
{
     public class EventDrawDto
     {
          /// <summary>
          /// Rider Id
          /// </summary>
          [JsonProperty("guyid")]
          public int RiderId { get; set; }

          /// <summary>
          /// Rider Name
          /// </summary>
          [JsonProperty("rider_name")]
          public string RiderName { get; set; }

          /// <summary>
          /// Bull Id
          /// </summary>
          [JsonProperty("pbid")]
          public int BullId { get; set; }

          /// <summary>
          /// Bull Name
          /// </summary>
          [JsonProperty("bull")]
          public string BullName { get; set; }

          /// <summary>
          /// Round
          /// </summary>
          [JsonProperty("round")]
          public string Round { get; set; }

          /// <summary>
          /// QR Prob
          /// </summary>
          [JsonProperty("qr_prob")]
          public decimal QRProb { get; set; }

          /// <summary>
          /// Event Id
          /// </summary>
          public string EventId { get; set; }

          /// <summary>
          /// Error
          /// </summary>
          [JsonProperty("error")]
          public string Error { get; set; }

          /// <summary>
          /// Event Name
          /// </summary>
          public string EventName { get; set; }

          /// <summary>
          /// Start Date
          /// </summary>
          public DateTime StartDate { get; set; }

          /// <summary>
          /// Remaining Date
          /// </summary>
          public DateTime RemainingDate { get; set; }

          /// <summary>
          /// Country Name
          /// </summary>
          public string CountryName { get; set; }
     }
}
