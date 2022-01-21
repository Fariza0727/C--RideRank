using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Dto.Calcutta
{
    public class CalcuttaPayoutDto
    {

        public int Id { get; set; }
        [JsonProperty("rowid")]
        public int RowId { get; set; }

        [JsonProperty("sanction")]
        public string Sanction { get; set; }


        [JsonProperty("pay_perc")]
        public decimal PayPerc { get; set; }

        [JsonProperty("position")]
        public int Position { get; set; }

        [JsonProperty("place_ttl")]
        public int PlaceTTL { get; set; }

    }
    public class CalcuttaPayoutsDto
    {
        [JsonProperty("RECORDS")]
        public List<CalcuttaPayoutDto> CalcuttaPayoutList { get; set; }

    }
}
