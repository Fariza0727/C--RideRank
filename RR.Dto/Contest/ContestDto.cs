using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RR.Dto
{
    public class ContestDto
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required(ErrorMessage = "Required")]
        public long Id { get; set; }

        /// <summary>
        ///  Title
        /// </summary>
        [Required]
        [Remote("IsTitleAvailable", "ContestManagement", AdditionalFields = "OldTitle", HttpMethod = "Post", ErrorMessage = "This Title Already Available")]
        public string Title { get; set; }

        /// <summary>
        /// Old  Title
        /// </summary>
        public string OldTitle { get; set; }

        /// <summary>
        /// EventId
        /// </summary>
        [Remote("IsEventContestAdded", "ContestManagement", AdditionalFields = "Id", HttpMethod = "Post", ErrorMessage = "you have already created a contest for this event, please choose another event.")]
        [Required(ErrorMessage = "Please choose event.")]
        public int EventId { get; set; }

        /// <summary>
        /// ContestCategoryId
        /// </summary>
        [Required(ErrorMessage = "Please choose Contest Category.")]
        public int ContestCategoryId { get; set; }

        /// <summary>
        /// JoiningFee
        /// </summary>
        [Required(ErrorMessage = "Please enter joining fee.")]
        public decimal JoiningFee { get; set; }
        /// <summary>
        /// Display Joining Fee
        /// </summary>
        public string JoiningFeeDisplay { get; set; }
        /// <summary>
        /// Members
        /// </summary>
        [Range(1, Double.MaxValue, ErrorMessage = "Members can not be zero.")]
        [Required(ErrorMessage = "Please enter number of member.")]
        public int Members { get; set; }

        /// <summary>
        /// Winners
        /// </summary>
        [Range(1, Double.MaxValue, ErrorMessage = "Winner can not be zero. Please enter atleast 1 winner.")]
        [Required(ErrorMessage = "Please enter no of winners")]
        public int Winners { get; set; }

        /// <summary>
        /// WinnerTitle
        /// </summary>
        [Required(ErrorMessage = "Please enter no of winnerswinner title.")]
        public string WinnerTitle { get; set; }
        
        //[Required(ErrorMessage = "Please select Entry Fee Type")]
        public int EntryFeeType { get; set; }

        public int JoinedMembers { get; set; }

        /// <summary>
        /// WinningPrice
        /// </summary>
        public decimal WinningPrice { get; set; }

        /// <summary>
        /// WinningPrice
        /// </summary>
        public long WinningToken { get; set; }

        /// <summary>
        /// IsAutoCreate
        /// </summary>
        public bool IsAutoCreate { get; set; }

        /// <summary>
        /// HowManyTimesCreate
        /// </summary>
        public int HowManyTimesCreate { get; set; }

        /// <summary>
        /// UniqueCode
        /// </summary>
        public string UniqueCode { get; set; }

        /// <summary>
        /// IsPrivate
        /// </summary>
        public bool IsPrivate { get; set; }

        /// <summary>
        /// CreatedDate
        /// </summary>
        public DateTime CreatdDate { get; set; }

        /// <summary>
        /// CreatedBy
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// IsActive
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// IsRefunded
        /// </summary>
        public bool IsRefunded { get; set; }

        /// <summary>
        /// EventName
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// AwardType
        /// </summary>
        public string AwardType { get; set; }

        /// <summary>
        /// AwardTypes
        /// </summary>
        public List<SelectListItem> AwardTypes { get; set; }

        /// <summary>
        /// ContestCategories
        /// </summary>
        public List<SelectListItem> ContestCategories { get; set; }

        /// <summary>
        /// Events
        /// </summary>
        public List<SelectListItem> Events { get; set; }

        /// <summary>
        /// UpdatedDate
        /// </summary>
        public DateTime UpdatedDate { get; set; }

        /// <summary>
        /// UpdatedBy
        /// </summary>
        public string UpdatedBy { get; set; }

        public DateTime PerfTime { get; set; }
    }
}
