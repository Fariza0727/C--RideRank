using System;
using System.Collections.Generic;

namespace RR.Data
{
    public partial class LongTermTeamRider
    {
        public int TeamRiderId { get; set; }
        public int TeamId { get; set; }
        public int RiderId { get; set; }
        public bool IsSubstitute { get; set; }
        public decimal RiderPoint { get; set; }
        public decimal? BonusPoint { get; set; }
        public int RiderTier { get; set; }

        public virtual LongTermTeam Team { get; set; }
    }
}
