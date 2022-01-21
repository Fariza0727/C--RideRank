using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RR.Dto
{
     public class TransactionLiteApiDto
     {
          public string UserId { get; set; }
          public int ContestId { get; set; }
          public int EventId { get; set; }
          public int TeamId { get; set; }

          public decimal? ContestFee { get; set; }


          public string ExpiryDate { get; set; }
          public int? AwardType { get; set; }
          public int TokenCount { get; set; }
          public bool IsToken { get; set; }
          [Required]
          public string PaymentMode { get; set; }
          public bool IsUpgrade = false;
          public string PlayerType { get; set; }
          public string PaymentMadeFor { get; set; }
          public string TokenWillGet { get; set; }

     }
}
