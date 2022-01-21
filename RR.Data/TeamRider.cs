using System;
using System.Collections.Generic;

namespace RR.Data
{
    public partial class TeamRider
    {
        public int TeamRiderId { get; set; }
        public int TeamId { get; set; }
        public int RiderId { get; set; }
        public bool IsSubstitute { get; set; }
        public decimal RiderPoint { get; set; }

        public virtual Team Team { get; set; }
    }
}
