using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Dto.Calcutta
{
    public class CalcuttaRCResultDto
    {
        public int Id { get; set; }
        [JsonProperty("pevid")]
        public string ParentEventId { get; set; }

        [JsonProperty("rowId")]
        public int RowId { get; set; }

        [JsonProperty("competitor_id")]
        public string CompetitorId { get; set; }

        [JsonProperty("competitor_name")]
        public string CompetitorName { get; set; }

        [JsonProperty("aggregate_score")]
        public decimal Score { get; set; }

    }


    public class CalcuttaRCResultsDto
    {

        [JsonProperty("pevid")]
        public string ParentEventId { get; set; }

        [JsonProperty("results")]
        public List<CalcuttaRCResultDto> CalcuttaRCResultList { get; set; }

    }
}
