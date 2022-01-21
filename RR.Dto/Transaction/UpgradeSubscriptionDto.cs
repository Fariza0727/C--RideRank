using System.ComponentModel.DataAnnotations;

namespace RR.Dto
{
     public class UpgradeSubscriptionDto
     {
          [Required]
          public int Amount { get; set; }
          [Required]
          public string PaymentMode { get; set; }
     }
}
