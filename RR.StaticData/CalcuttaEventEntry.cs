using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.StaticData
{
    public partial class CalcuttaEventEntry
    {
        public int Id { get; set; }
        public string ParentEventId { get; set; }
        public string EventId { get; set; }
        public string EntryId { get; set; }
        public string RegNo { get; set; }
        public string CompetitorId { get; set; }
        public string CompetitorName { get; set; }
        public string Owner { get; set; }
        public string Handler { get; set; }
        public string Del { get; set; }
        public string Draw { get; set; }
        public decimal CalcuttaPrice { get; set; }
        public string Standings { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
