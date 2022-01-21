using System;
using System.Collections.Generic;

namespace RR.Data
{
    public partial class Team
    {
        public Team()
        {
            ContestUserWinner = new HashSet<ContestUserWinner>();
            JoinedContest = new HashSet<JoinedContest>();
            TeamBull = new HashSet<TeamBull>();
            TeamRider = new HashSet<TeamRider>();
        }

        public int Id { get; set; }
        public string UserId { get; set; }
        public int EventId { get; set; }
        public byte ContestType { get; set; }
        public int TeamNumber { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal TeamPoint { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        public virtual AspNetUsers User { get; set; }
        public virtual ICollection<ContestUserWinner> ContestUserWinner { get; set; }
        public virtual ICollection<JoinedContest> JoinedContest { get; set; }
        public virtual ICollection<TeamBull> TeamBull { get; set; }
        public virtual ICollection<TeamRider> TeamRider { get; set; }
    }
}
