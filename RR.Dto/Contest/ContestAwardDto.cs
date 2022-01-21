using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace RR.Dto
{
    public class ContestAwardDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// ContestId
        /// </summary>
        public long ContestId { get; set; }

        /// <summary>
        /// AwardTypeId
        /// </summary>
        public int?[] AwardTypeId { get; set; }

        /// <summary>
        /// Awards
        /// </summary>
        public List<SelectListItem> Awards { get; set; }

        /// <summary>
        /// Award Types
        /// </summary>
        public List<SelectListItem> AwardTypes { get; set; }

        /// <summary>
        /// Winners
        /// </summary>
        public IEnumerable<ContestWinnerDto> Winners { get; set; }

        /// <summary>
        /// RankFrom
        /// </summary>
        public int?[] RankFrom { get; set; }

        /// <summary>
        /// RankTo
        /// </summary>
        public int?[] RankTo { get; set; }

        /// <summary>
        /// Price Percentage
        /// </summary>
        public int?[] PricePercentage { get; set; }

        /// <summary>
        /// Token Percentage
        /// </summary>
        public int?[] TokenPercentage { get; set; }

        /// <summary>
        /// Marchendise
        /// </summary>
        public long?[] Merchandise { get; set; }

        /// <summary>
        /// Marchendise List
        /// </summary>
        public List<SelectListItem> MerchandiseList { get; set; }

        /// <summary>
        /// Token Percentage
        /// </summary>
        public long?[] OtherReward { get; set; }

        /// <summary>
        /// Other Reward List
        /// </summary>
        public List<SelectListItem> OtherRewardList { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public int?[] Value { get; set; }

        /// <summary>
        /// AwardId
        /// </summary>
        public int?[] AwardId { get; set; }

        /// <summary>
        /// CreatedBy
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// WinnerCount
        /// </summary>
        public int WinnerCount { get; set; }
        /// <summary>
        /// Is Paid Member
        /// </summary>
        public bool?[] IsPaidMember { get; set; }
    }
}
