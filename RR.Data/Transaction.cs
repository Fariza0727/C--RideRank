using System;
using System.Collections.Generic;

namespace RR.Data
{
    public partial class Transaction
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public int? TokenCredit { get; set; }
        public decimal? TransactionDebit { get; set; }
        public byte TransactionType { get; set; }
        public string TransactionId { get; set; }
        public string TextMessage { get; set; }
        public string ResponseMessage { get; set; }
        public string AuthCode { get; set; }
        public DateTime TransactionDate { get; set; }
        public bool IsActive { get; set; }

        public virtual AspNetUsers User { get; set; }
        public virtual JoinedContest JoinedContest { get; set; }
    }
}
