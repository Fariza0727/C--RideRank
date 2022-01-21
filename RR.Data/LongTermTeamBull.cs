using System;
using System.Collections.Generic;

namespace RR.Data
{
    public partial class LongTermTeamBull
    {
        public int TeamBullId { get; set; }
        public int TeamId { get; set; }
        public int BullId { get; set; }
        public bool IsSubstitute { get; set; }
        public decimal BullPoint { get; set; }
        public decimal? BonusPoint { get; set; }
        public int BullTier { get; set; }

        public virtual LongTermTeam Team { get; set; }
    }
}
