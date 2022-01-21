using System;
using System.Collections.Generic;

namespace RR.StaticData
{
    public partial class EventDraw
    {
        public int Id { get; set; }
        public int RiderId { get; set; }
        public string RiderName { get; set; }
        public int BullId { get; set; }
        public string BullName { get; set; }
        public string Round { get; set; }
        public int EventId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public decimal? QRProb { get; set; }

        public virtual Event Event { get; set; }
    }
}
