using System;
using System.Collections.Generic;

namespace RR.StaticData
{
    public partial class Event
    {
        public Event()
        {
            EventBull = new HashSet<EventBull>();
            EventDraw = new HashSet<EventDraw>();
            EventRider = new HashSet<EventRider>();
        }

        public int Id { get; set; }
        public string EventId { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Type { get; set; }
        public int Season { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime PerfTime { get; set; }
        public string Sanction { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool? WinningDistributed { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public string EventResult { get; set; }
        public string Pbrid { get; set; }
        public DateTime? PerfTimeUTC { get; set; }

        public virtual ICollection<EventBull> EventBull { get; set; }
        public virtual ICollection<EventDraw> EventDraw { get; set; }
        public virtual ICollection<EventRider> EventRider { get; set; }
    }
}
