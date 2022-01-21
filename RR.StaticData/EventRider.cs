using System;
using System.Collections.Generic;

namespace RR.StaticData
{
    public partial class EventRider
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int RiderId { get; set; }
        public decimal CwrpBonus { get; set; }
        public decimal EventTierScore { get; set; }
        public int EventTier { get; set; }
        public int EventTierRank { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public bool? IsDropout { get; set; }

        public virtual Event Event { get; set; }
        public virtual Rider Rider { get; set; }
    }
}
