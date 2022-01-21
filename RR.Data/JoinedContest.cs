using System;
using System.Collections.Generic;

namespace RR.Data
{
    public partial class JoinedContest
    {
        public int PaymentTxnId { get; set; }
        public string UserId { get; set; }
        public int ContestId { get; set; }
        public int TeamId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        public virtual Transaction PaymentTxn { get; set; }
        public virtual Team Team { get; set; }
        public virtual AspNetUsers User { get; set; }
    }
}
