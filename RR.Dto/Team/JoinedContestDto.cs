using System;
using System.Collections.Generic;

namespace RR.Dto
{
    public class JoinedContestDto
    { 
        public string UserId { get; set; }
        public int ContestId { get; set; }
        public int TeamId { get; set; }
        public int PaymentTxnId { get; set; }
        public decimal Amount { get; set; }
        public int PaymentType { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentOption { get; set; }
    }
}
