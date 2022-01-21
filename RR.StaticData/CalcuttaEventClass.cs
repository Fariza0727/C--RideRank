using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.StaticData
{
    public partial class CalcuttaEventClass
    {
        public int Id { get; set; }
        public string ParentEventId { get; set; }
        public string EventId { get; set; }
        public string EventClass { get; set; }
        public string EventType { get; set; }
        public string EventLabel { get; set; }
        public string Sanction { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ClassUTCLockTime { get; set; }
        public decimal Fees { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
