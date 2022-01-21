using System;
using System.Collections.Generic;

namespace RR.StaticData
{
    public partial class EventBull
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int BullId { get; set; }
        public int EventTier { get; set; }
        public decimal TierScore { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public bool? IsDropout { get; set; }

        public virtual Bull Bull { get; set; }
        public virtual Event Event { get; set; }
    }
}
