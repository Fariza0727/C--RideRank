using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RR.Dto
{
    public class ContestWinnerDto
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required(ErrorMessage = "Required")]
        public long Id { get; set; }

        /// <summary>
        /// ContestId
        /// </summary>
        [Required(ErrorMessage = "Please choose contest")]
        public long ContestId { get; set; }

        /// <summary>
        /// RankFrom
        /// </summary>
        [Required(ErrorMessage = "Please choose From rank.")]
        public int RankFrom { get; set; }

        /// <summary>
        /// RankTo
        /// </summary>
        [Required(ErrorMessage = "Please choose To rank.")]
        public int RankTo { get; set; }

        /// <summary>
        /// AwardId
        /// </summary>
        [Required(ErrorMessage = "Please choose award Id.")]
        public long AwardId { get; set; }

        /// <summary>
        /// AwardValue
        /// </summary>
        public decimal AwardValue { get; set; }

        /// <summary>
        /// Created By
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Created Date
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Updated By
        /// </summary>
        public string UpdatedBy { get; set; }

        /// <summary>
        /// Updated Date
        /// </summary>
        public DateTime UpdatedDate { get; set; }


        /// <summary>
        /// Awards
        /// </summary>
        public List<SelectListItem> Awards { get; set; }

        /// <summary>
        /// Award Types
        /// </summary>
        public List<SelectListItem> AwardTypes { get; set; }

        /// <summary>
        /// AwardValues
        /// </summary>
        public List<decimal> AwardValues { get; set; }

        /// <summary>
        /// Price Percentage
        /// </summary>
        public int PricePercentage { get; set; }

        /// <summary>
        /// Token Percentage
        /// </summary>
        public int TokenPercentage { get; set; }

        /// <summary>
        /// Marchendise
        /// </summary>
        public long Merchandise { get; set; }
        /// <summary>
        /// Marchendise
        /// </summary>
        public bool? IsPaidMember { get; set; }
        /// <summary>
        /// Marchendise List
        /// </summary>
        public List<SelectListItem> MerchandiseList { get; set; }

        /// <summary>
        /// Token Percentage
        /// </summary>
        public long OtherReward { get; set; }

        /// <summary>
        /// Other Reward List
        /// </summary>
        public List<SelectListItem> OtherRewardList { get; set; }
    }
}
