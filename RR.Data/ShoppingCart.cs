using System;
using System.Collections.Generic;

namespace RR.Data
{
    public partial class ShoppingCart
    {
        
        public int Id { get; set; }
        public string UserId { get; set; }
        public int EntId { get; set; }
        public string ParentEventId { get; set; }
        public string EventId { get; set; }
        public string EntryId { get; set; }
        public decimal CalcuttaPrice { get; set; }

        public bool IsSold { get; set; }
        public string OrderId { get; set; }
        public string PayerId { get; set; }

    }
}
