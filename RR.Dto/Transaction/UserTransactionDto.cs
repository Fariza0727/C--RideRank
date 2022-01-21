using System;

namespace RR.Dto
{
     public class UserTransactionDto
     {
          public string UserName { get; set; }
          public decimal? TransactionAmount { get; set; }
          public DateTime TransactionDate { get; set; }
          public string ResponseMessage { get; set; }
          public int? CurrentWallet { get; set; }
          public int? TokenCredit { get; set; }
          public string date { get; set; }
          public int TransactionType { get; set; }
          public string TransactionId { get; set; }
          public string Avtar { get; set; }
     }
}
