namespace RR.Dto
{
    public class CreditCardDto
    {
        /// <summary>
        /// The Card Number
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// The Expir Date (0109)
        /// </summary>
        public string ExpiryDate { get; set; }

        /// <summary>
        /// The CVV
        /// </summary>
        public string CVV { get; set; }
    }
}