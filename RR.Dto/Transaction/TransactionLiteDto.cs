using System.ComponentModel.DataAnnotations;

namespace RR.Dto
{
    public class TransactionLiteDto
    {
        public string UserId { get; set; }
        public int ContestId { get; set; }
        public int EventId { get; set; }
        public int TeamId { get; set; }
        public decimal? ContestFee { get; set; }
        [StringLength(16, ErrorMessage = "Enter valid card number")]
        [Required(ErrorMessage ="Enter valid card number")]
        public string CardNumber { get; set; }

        [StringLength(3, ErrorMessage = "Enter valid cvv number.")]
        [Required(ErrorMessage ="Please enter cvv number")]
        [DataType(DataType.Password)]
        public string CVV { get; set; }

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

        [Required(ErrorMessage = "Enter expiry month.")]
        public string ExpiryMonth { get; set; }
        [Required(ErrorMessage = "Enter expiry year.")]
        public string ExpiryYear { get; set; }
    }
}
