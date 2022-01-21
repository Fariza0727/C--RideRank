using System;
using System.Collections.Generic;

namespace RR.Data
{
    public partial class FavoriteBullRiders
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public int? RiderId { get; set; }
        public int? BullId { get; set; }
    }
}
