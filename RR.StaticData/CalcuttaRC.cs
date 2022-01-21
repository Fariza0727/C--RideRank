using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.StaticData
{
    public partial class CalcuttaRC
    {
        public int Id { get; set; }
        public string ParentEventId { get; set; }
        public string Title { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ContestType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ContestUTCLockTime { get; set; }
        public string ContestStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
