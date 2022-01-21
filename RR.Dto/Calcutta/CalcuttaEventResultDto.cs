using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Dto.Calcutta
{
    public class CalcuttaEventResultDto
    {
        public int Id { get; set; }
        [JsonProperty("evid")]
        public string EventId { get; set; }

        [JsonProperty("entid")]
        public string EntryId { get; set; }

        [JsonProperty("outid")]
        public string OutId { get; set; }

        [JsonProperty("regno")]
        public string RegNo { get; set; }

        [JsonProperty("competitor_id")]
        public string CompetitorId { get; set; }

        [JsonProperty("competitor_name")]
        public string CompetitorName { get; set; }

        [JsonProperty("owner")]
        public string Owner { get; set; }


        [JsonProperty("del")]
        public string Del { get; set; }

        [JsonProperty("sc")]
        public decimal Score { get; set; }

        [JsonProperty("place")]
        public string Place { get; set; }

        [JsonProperty("money")]
        public decimal Money { get; set; }

        [JsonProperty("evlinkid")]
        public string EventLinkId { get; set; }

    }


    public class CalcuttaEventResultsDto
    {

        [JsonProperty("pevid")]
        public string ParentEventId { get; set; }

        [JsonProperty("results")]
        public List<CalcuttaEventResultDto> CalcuttaEventResultList { get; set; }

    }
}
