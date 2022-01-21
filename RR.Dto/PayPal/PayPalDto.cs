namespace RR.Dto
{
    public class PayPalDto
    {
        /// <summary>
        /// The Credit Card Details
        /// </summary>
        public CreditCardDto CreditCardDetails { get; set; }

        /// <summary>
        /// The Address
        /// </summary>
        public AddressDto Address { get; set; }

        /// <summary>
        /// The Amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// The Unique Order Number
        /// </summary>
        public string PONumber { get; set; }

        /// <summary>
        /// The Unique Invoice Number
        /// </summary>
        public string InvNumber { get; set; }
    }
}