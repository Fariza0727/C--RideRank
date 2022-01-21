using System;

namespace RR.Dto
{
    public class ContestLiteDto
    {
        /// <summary>
        /// Contest Id
        /// </summary>
        public long ContestId { get; set; }

        /// <summary>
        /// EventId
        /// </summary>
        public int EventId { get; set; }

        /// <summary>
        /// JoiningFee
        /// </summary>
        public decimal JoiningFee { get; set; }

        /// <summary>
        /// WinnerTitle
        /// </summary>
        public string WinnerTitle { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// ContestCategoryName
        /// </summary>
        public string ContestCategoryName { get; set; }

        /// <summary>
        /// AwardType
        /// </summary>
        public string AwardTypeName { get; set; }

        /// <summary>
        /// Is joined
        /// </summary>
        public bool IsJoined = false;

        /// <summary>
        /// Award type Id
        /// </summary>
        public int? AwardTypeId { get; set; }

        /// <summary>
        /// Entry fee type
        /// </summary>
        public string EntryFeeType { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }
       
    }
}
