using System;
using System.Collections.Generic;

namespace RR.AdminData
{
    public partial class ContestWinner
    {
        public long Id { get; set; }
        public long ContestId { get; set; }
        public int RankFrom { get; set; }
        public int RankTo { get; set; }
        public int PricePercentage { get; set; }
        public int TokenPercentage { get; set; }
        public long? Marchendise { get; set; }
        public long? OtherReward { get; set; }
        public bool? IsPaidMember { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual Contest Contest { get; set; }
        public virtual Award MarchendiseNavigation { get; set; }
    }
}
