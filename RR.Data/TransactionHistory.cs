using System;
using System.Collections.Generic;

namespace RR.Data
{
    public partial class TransactionHistory
    {
        
        public int Id { get; set; }
        public string UserId { get; set; }
        public int EventId { get; set; }
        public int EventMode { get; set; }
        public string OrderId { get; set; }
        public string PayerId { get; set; }
        public string PayerEmail { get; set; }
        public string PayerGivenName { get; set; }
        public string PayerSureName { get; set; }
        public decimal Amount { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}
