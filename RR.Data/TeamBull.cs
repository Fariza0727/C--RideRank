using System;
using System.Collections.Generic;

namespace RR.Data
{
    public partial class TeamBull
    {
        public int TeamBullId { get; set; }
        public int TeamId { get; set; }
        public int BullId { get; set; }
        public bool IsSubstitute { get; set; }
        public decimal BullPoint { get; set; }

        public virtual Team Team { get; set; }
    }
}
