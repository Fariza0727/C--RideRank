using System;
using System.Collections.Generic;

namespace RR.Dto
{
     public partial class TransactionDto
     {
          public string Description { get; set; }
          public int? TokenCredit { get; set; }
          public decimal TransactionDebit { get; set; }
          public byte TransactionType { get; set; }
          public string TransactionId { get; set; }
          public string TextMessage { get; set; }
          public string ResponseMessage { get; set; }
          public string AuthCode { get; set; }
          public string UserId { get; set; }
     }
}
