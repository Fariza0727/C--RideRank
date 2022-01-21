using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Dto.Calcutta
{
    public class CalcuttaRCEntryDto
    {
        public int Id { get; set; }
        [JsonProperty("rowid")]
        public int RowId { get; set; }

        [JsonProperty("competitor_id")]
        public string CompetitorId { get; set; }

        [JsonProperty("competitor_name")]
        public string CompetitorName { get; set; }

        [JsonProperty("pevid")]
        public string ParentEventId { get; set; }


        [JsonProperty(PropertyName = "riderpower", NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(0.0)]
        public decimal RiderPower { get; set; }

        [DefaultValue(0.0)]
        [JsonProperty(PropertyName = "riderpower_current", NullValueHandling = NullValueHandling.Ignore)]
        public decimal RiderPowerCurrent { get; set; }

        [JsonProperty("rp_avg")]
        public decimal RiderPowerAvg { get; set; }

        [JsonProperty("calcutta_price")]
        public decimal CalcuttaPrice { get; set; }

    }

    public class CalcuttaRCEntriesDto
    {

        [JsonProperty("pevid")]
        public string ParentEventId { get; set; }

        [JsonProperty("entries")]
        public List<CalcuttaRCEntryDto> CalcuttaRCEntriesList { get; set; }

    }
}
