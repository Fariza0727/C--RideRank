using System;
using System.Collections.Generic;

namespace RR.AdminData
{
    public partial class PicuresManager
    {
        public long Id { get; set; }
        public int? BullId { get; set; }
        public string BullPicture { get; set; }
        public int? RiderId { get; set; }
        public string RiderPicture { get; set; }
        public string RiderName { get; set; }
        public string BullName { get; set; }
    }
}
