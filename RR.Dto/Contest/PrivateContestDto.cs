using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace RR.Dto
{
     public class PrivateContestDto
     {
          /// <summary>
          /// Id
          /// </summary>
          [Required(ErrorMessage = "Required")]
          public long Id { get; set; }

          /// <summary>
          /// AwardTypeId
          /// </summary>
          [Required(ErrorMessage = "Please choose award type.")]
          public int? AwardTypeId { get; set; }

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
          [Range(1, Double.MaxValue, ErrorMessage = "joining fee can not be zero.")]
          [Required(ErrorMessage = "Please enter joining fee.")]
          public decimal JoiningFee { get; set; }

          /// <summary>
          /// Members
          /// </summary>
          [Range(2, 10, ErrorMessage = "Members can't be less than two and greater than ten")]
          [Required(ErrorMessage = "Please enter number of member.")]
          public int Members { get; set; }

          /// <summary>
          /// Winners
          /// </summary>
          [Range(1, 10, ErrorMessage = "Members can't be less than one and greater than ten")]
          [Required(ErrorMessage = "Please enter no of winners")]
          public int Winners { get; set; }

          /// <summary>
          /// WinnerTitle
          /// </summary>
          [Required(ErrorMessage = "Please enter winner title.")]
          public string WinnerTitle { get; set; }

          public int EntryFeeType { get; set; }

          /// <summary>
          /// WinningPrice
          /// </summary>
          public decimal WinningPrice { get; set; }

          /// <summary>
          /// UniqueCode
          /// </summary>
          public string UniqueCode { get; set; }

          /// <summary>
          /// IsPrivate
          /// </summary>
          public bool IsPrivate { get; set; }

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
          /// IsActive
          /// </summary>
          public bool IsActive { get; set; }

          /// <summary>
          /// IsRefunded
          /// </summary>
          public bool IsRefunded { get; set; }

          //// Contest Winner

          /// <summary>
          /// RankFrom
          /// </summary>
          [Range(1, 10, ErrorMessage = "From rank can't be less than one and greater than ten")]
          [Required(ErrorMessage = "Please enter Rank From.")]
          [RequiredArray(ErrorMessage = "RankFrom is required.")]
          public int?[] RankFrom { get; set; }

          /// <summary>
          /// RankTo
          /// </summary>
          [Range(1, 10, ErrorMessage = "To rank can't be less than one and greater than ten")]
          [Required(ErrorMessage = "Please enter Rank To.")]
          [RequiredArray(ErrorMessage = "RankTo is required.")]
          public int?[] RankTo { get; set; }

          /// <summary>
          /// Value
          /// </summary>
          [Range(1, 100, ErrorMessage = "Value can't be less than one and greater than one hundrad")]
          [Required(ErrorMessage = "Please enter Value Of Winner.")]
          [RequiredArray(ErrorMessage = "Value is required.")]
          public int?[] Value { get; set; }

          public string UserId { get; set; }
     }

     public class RequiredArray : ValidationAttribute
     {
          public override bool IsValid(object value)
          {
               var list = value as IList;
               if (list != null)
               {
                    return list.Count > 0;
               }
               return false;
          }
     }
}
