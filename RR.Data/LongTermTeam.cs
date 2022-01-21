using System;
using System.Collections.Generic;

namespace RR.Data
{
    public partial class LongTermTeam
    {
        public LongTermTeam()
        {
            LongTermTeamBull = new HashSet<LongTermTeamBull>();
            LongTermTeamRider = new HashSet<LongTermTeamRider>();
            UserRequests = new HashSet<UserRequests>();
        }

        public int Id { get; set; }
        public string UserId { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public decimal TeamPoint { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string TeamIcon { get; set; }
        public string TeamBrand { get; set; }

        public virtual AspNetUsers User { get; set; }
        public virtual ICollection<LongTermTeamBull> LongTermTeamBull { get; set; }
        public virtual ICollection<LongTermTeamRider> LongTermTeamRider { get; set; }
        public virtual ICollection<UserRequests> UserRequests { get; set; }
    }
}
