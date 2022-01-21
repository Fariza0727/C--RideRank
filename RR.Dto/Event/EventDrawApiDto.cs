using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace RR.Dto
{
    public class EventDrawApiDto
    {
        [JsonProperty("guyid")]
        public string RiderID { get; set; }

        [JsonProperty("rider_name")]
        public string RiderName { get; set; }

        [JsonProperty("pbid")]
        public string BullID { get; set; }

        [JsonProperty("bull")]
        public string BullName { get; set; }

        [JsonProperty("round")]
        public string Round { get; set; }

        [JsonProperty("rider_tier")]
        public string RiderTier { get; set; }

        [JsonProperty("bull_tier")]
        public string BullTier { get; set; }

        [JsonProperty("r1_bull_tier")]
        public string R1_Bull_Tier { get; set; }
    }

}
