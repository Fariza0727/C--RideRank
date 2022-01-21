namespace RR.Dto
{
    public class RiderContestLiteDto
    {
        /// <summary>
        /// JoiningDate
        /// </summary>
        public System.DateTime JoiningDate { get; set; }

        /// <summary>
        /// Team Point
        /// </summary>
        public decimal TeamPoint { get; set; }

        /// <summary>
        /// TeamId
        /// </summary>
        public int TeamId { get; set; }

        /// <summary>
        /// RiderId
        /// </summary>
        public int RiderId { get; set; }

        /// <summary>
        /// Rider name
        /// </summary>
        public string RiderName { get; set; }

        /// <summary>
        /// Substitute rider
        /// </summary>
        public string SubstituteRider { get; set; }

        /// <summary>
        /// rider tier
        /// </summary>
        public int RiderTier { get; set; }

        /// <summary>
        /// contest type
        /// </summary>
        public string ContestType { get; set; }
        /// <summary>
        /// Joining Date
        /// </summary>
        public string jDate { get; set; }
        /// <summary>
        /// Rider Point
        /// </summary>
        public decimal RiderPoint { get; set; }
        public bool IsDropout { get; set; }

        public bool IsAddedFavorite { get; set; }
    }
}
