namespace RR.Dto
{
    public class BullContestLiteDto
    {
        /// <summary>
        /// bullid
        /// </summary>
        public int BullId { get; set; }

        /// <summary>
        /// bullname
        /// </summary>
        public string BullName { get; set; }

        /// <summary>
        /// sustitutebull
        /// </summary>
        public string SubstituteBull { get; set; }

        /// <summary>
        /// bull tier
        /// </summary>
        public int BullTier { get; set; }
        /// <summary>
        /// Bull Points
        /// </summary>
        public decimal BullPoint { get; set; }
        public bool IsDropout { get; set; }

        public bool IsAddedFavorite { get; set; }
    }
}
