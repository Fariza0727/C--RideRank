using System;
using System.Collections.Generic;

namespace RR.Data
{
    public partial class UserRequests
    {
        public long Id { get; set; }
        public string RequestNo { get; set; }
        public string Title { get; set; }
        public string RequestMessage { get; set; }
        public string Message { get; set; }
        public bool IsApproved { get; set; }
        public bool IsDelete { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ReturlUrl { get; set; }
        public string UserId { get; set; }
        public int? LongTermTeamId { get; set; }

        public virtual LongTermTeam LongTermTeam { get; set; }
        public virtual AspNetUsers User { get; set; }
    }
}
