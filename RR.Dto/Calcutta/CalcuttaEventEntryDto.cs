using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Dto.Calcutta
{
    public class CalcuttaEventEntryDto
    {
        public int Id { get; set; }
        [JsonProperty("evid")]
        public string EventId { get; set; }

        [JsonProperty("entid")]
        public string EntryId { get; set; }

        [JsonProperty("regno")]
        public string RegNo { get; set; }

        [JsonProperty("competitor_id")]
        public string CompetitorId { get; set; }

        [JsonProperty("competitor_name")]
        public string CompetitorName { get; set; }

        [JsonProperty("owner")]
        public string Owner { get; set; }

        [JsonProperty("handler")]
        public string Handler { get; set; }

        [JsonProperty("del")]
        public string Del { get; set; }

        [JsonProperty("draw")]
        public string Draw { get; set; }

        [JsonProperty("calcutta_price")]
        public decimal CalcuttaPrice { get; set; }

        [JsonProperty("standings")]
        public List<CalcuttaEventEntryStandingDto> Standings { get; set; }

    }
    public class CalcuttaEventEntryStandingDto
    {

        [JsonProperty("competitor_id")]
        public string CompetitorId { get; set; }

        [JsonProperty("division")]
        public string Division { get; set; }

        [JsonProperty("ppos")]
        public string PPos { get; set; }

        [JsonProperty("money")]
        public decimal Money { get; set; }

        [JsonProperty("qoutcount")]
        public int QOutCount { get; set; }

    }

    public class CalcuttaEventEntriesDto
    {

        [JsonProperty("pevid")]
        public string ParentEventId { get; set; }

        [JsonProperty("entries")]
        public List<CalcuttaEventEntryDto> CalcuttaEventEntryList { get; set; }

    }
}
