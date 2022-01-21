using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RR.Dto
{
     public class UserDetailDto
     {
          /// <summary>
          /// Id
          /// </summary>
          public long Id { get; set; }

          /// <summary>
          /// User Id
          /// </summary>
          public string UserId { get; set; }

          /// <summary>
          /// User Name
          /// </summary>
          public string UserName { get; set; }

          /// <summary>
          /// First Name
          /// </summary>
          [Required(ErrorMessage = "First Name is required.")]
          public string FirstName { get; set; }

        /// <summary>
        /// Last Name
        /// </summary>
        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }

          /// <summary>
          /// Email Address
          /// </summary>
          public string Email { get; set; }

        /// <summary>
        /// Phone Number
        /// </summary>
        [Required(ErrorMessage = "Phone number is required.")]
        [StringLength(15, ErrorMessage = "The {0} must be at least {2} and at max {1} numbers long.", MinimumLength = 8)]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Address 
        /// </summary>
        [Required(ErrorMessage = "Address1 is required.")]
        public string Address1 { get; set; }

          /// <summary>
          /// Another Address
          /// </summary>
          public string Address2 { get; set; }

          /// <summary>
          /// Another Address
          /// </summary>
          public string Address3 { get; set; }

          /// <summary>
          /// City
          /// </summary>
          public string City { get; set; }

          /// <summary>
          /// State
          /// </summary>
          public int? State { get; set; }

          /// <summary>
          /// Country
          /// </summary>
          public int? Country { get; set; }

          /// <summary>
          /// Country Name
          /// </summary>
          public string CountryName { get; set; }

          /// <summary>
          /// State Name
          /// </summary>
          public string StateName { get; set; }

          /// <summary>
          /// City Name
          /// </summary>
          public string CityName { get; set; }

          /// <summary>
          /// Banking
          /// </summary>
          public string Banking { get; set; }

          /// <summary>
          /// Avtar
          /// </summary>
          public string Avtar { get; set; }

          /// <summary>
          /// League Notification
          /// </summary>
          public bool LeagueNotification { get; set; }

          /// <summary>
          /// Is Block
          /// </summary>
          public bool IsBlock { get; set; }

          /// <summary>
          /// Is Active
          /// </summary>
          public bool IsActive { get; set; }

          /// <summary>
          /// Is Delete
          /// </summary>
          public bool IsDelete { get; set; }

          /// <summary>
          /// Created Date
          /// </summary>
          public DateTime CreatedDate { get; set; }

          /// <summary>
          /// Updated Date
          /// </summary>
          public DateTime? UpdatedDate { get; set; }

          /// <summary>
          /// Date of birth
          /// </summary>
          [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Required(ErrorMessage = "DOB is required.")]
        public DateTime? DOB { get; set; }
          
        
          public string DateOfBirth { get; set; }

          /// <summary>
          /// Country list
          /// </summary>
          public List<SelectListItem> CountryList { get; set; }

          /// <summary>
          /// State List
          /// </summary>
          public List<SelectListItem> StateList { get; set; }

          /// <summary>
          /// City List
          /// </summary>
          public List<SelectListItem> CityList { get; set; }

          /// <summary>
          /// Player Type
          /// </summary>
          public string PlayerType { get; set; }

          /// <summary>
          /// PlayerTypeId
          /// </summary>
          public string PlayerTypeId { get; set; }

          /// <summary>
          /// PlayerTypeList
          /// </summary>
          public List<SelectListItem> PlayerTypeList { get; set; }

          /// <summary>
          /// Password
          /// </summary>
          [Required]
          [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
          [DataType(DataType.Password)]
          public string Password { get; set; }

          /// <summary>
          /// Confirm Password
          /// </summary>
          [DataType(DataType.Password)]
          [Display(Name = "Confirm password")]
          [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
          public string ConfirmPassword { get; set; }

          public int? WalletToken { get; set; }

          public DateTime? SubscriptionExpiryDate { get; set; }

          public string RegisteredOn { get; set; }
          [Required(ErrorMessage = "ZipCode is required.")]
          public string ZipCode { get; set; }

          /// <summary>
          /// User Role List
          /// </summary>
          public List<SelectListItem> UserRoleList { get; set; }

          /// <summary>
          /// Payent Mode
          /// </summary>
          public string PaymentMode { get; set; }
        /// <summary>
        /// Is Paid Member
        /// </summary>
        public bool? IsPaidMember { get; set; }
        public bool? IsOnline { get; set; }

        public long? ShopifyCustomerId { get; set; }
        public string ShopifyMembership { get; set; }

        public bool IsNotifyEmail { get; set; }
        public bool IsNotifySms { get; set; }
        public string OptPhoneno { get; set; }
        [Required(ErrorMessage = "Team Name is required.")]
        public string TeamName { get; set; }

        public string ReferralCode { get; set; }
        public int? ReferredCustomers { get; set; }
        public string CustomReferralCode { get; set; }
    }
}
