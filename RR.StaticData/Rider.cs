using System;
using System.Collections.Generic;

namespace RR.StaticData
{
    public partial class Rider
    {
        public Rider()
        {
            EventRider = new HashSet<EventRider>();
        }

        public int Id { get; set; }
        public int RiderId { get; set; }
        public string Name { get; set; }
        public int Mounted { get; set; }
        public int Rode { get; set; }
        public decimal Streak { get; set; }
        public string Hand { get; set; }
        public decimal RidePerc { get; set; }
        public decimal RidePrecCurent { get; set; }
        public int MountedCurrent { get; set; }
        public decimal RiderPower { get; set; }
        public decimal RiderPowerCurrent { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public int? Cwrp { get; set; }

        public virtual ICollection<EventRider> EventRider { get; set; }
    }
}
