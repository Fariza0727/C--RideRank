using System;
using System.Collections.Generic;
using System.Text;

namespace RR.Dto
{
    public class ContestUserWinnerDto
    {
        /// <summary>
        /// User Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// User Team Id
        /// </summary>
        public int TeamId { get; set; }

        /// <summary>
        /// Contest Id
        /// </summary>
        public long ContestId { get; set; }

        /// <summary>
        /// Event Id
        /// </summary>
        public int EventId { get; set; }

        /// <summary>
        /// Contest Winner Id
        /// </summary>
        public long ContestWinnerId { get; set; }

        /// <summary>
        /// Created date
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Team Rank
        /// </summary>
        public int TeamRank { get; set; }

        /// <summary>
        /// Award Type
        /// </summary>
        public string AwardType { get; set; }

        /// <summary>
        /// Award Token if required.
        /// </summary>
        public string AwardToken { get; set; }

        /// <summary>
        /// Award Message
        /// </summary>
        public string AwardMessage { get; set; }

        /// <summary>
        /// Award Value
        /// </summary>
        public decimal AwardValue { get; set; }

        /// <summary>
        /// Award Type Id
        /// </summary>
        public int AwardTypeId { get; set; }

        /// <summary>
        /// Contest Title
        /// </summary>
        public string ContestTitle { get; set; }

        /// <summary>
        /// Contest Winning Title
        /// </summary>
        public string ContestWinningTitle { get; set; }

        /// <summary>
        /// Price
        /// </summary>
        public string Price { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Merchendise
        /// </summary>
        public string Merchendise { get; set; }

    }
}
