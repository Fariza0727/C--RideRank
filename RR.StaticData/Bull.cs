using System;
using System.Collections.Generic;

namespace RR.StaticData
{
    public partial class Bull
    {
        public Bull()
        {
            EventBull = new HashSet<EventBull>();
        }

        public int Id { get; set; }
        public int BullId { get; set; }
        public string Brand { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public bool IsRegistered { get; set; }
        public int HistoricalRank { get; set; }
        public int ActiveRank { get; set; }
        public int Mounted { get; set; }
        public int Rode { get; set; }
        public decimal AverageMark { get; set; }
        public decimal PowerRating { get; set; }
        public decimal BuckOffPerc { get; set; }
        public int OutsVsTopRiders { get; set; }
        public decimal BuckOffPercVsTopRider { get; set; }
        public int OutVsLeftHandRider { get; set; }
        public decimal BuckOffPercVsLeftHandRider { get; set; }
        public int OutVsRightHandRider { get; set; }
        public decimal BuckOffPercVsRightHandRider { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }

        public virtual ICollection<EventBull> EventBull { get; set; }
    }
}
