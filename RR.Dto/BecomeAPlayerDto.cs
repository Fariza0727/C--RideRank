using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace RR.Dto
{
    public class BecomeAPlayerDto
    {
        /// <summary>
        /// First Name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Sur Name
        /// </summary>
        public string SurName { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        /// 
        [Required(ErrorMessage = "Email is required.")]
        [Remote("IsAlreadySigned", "Account", HttpMethod = "POST", ErrorMessage = "EmailId already exists in database.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        /// <summary>
        /// Password
        /// </summary>    
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{7,}$", ErrorMessage = "Password should contain at least 7 characters with 1 number, 1 lowercase and 1 uppercase character")]
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        /// <summary>
        /// ConfirmPassword
        /// </summary>
        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// DateOfBirth
        /// </summary>
        [Required(ErrorMessage = "DOB is required.")]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// UserName
        /// </summary>
        //[Required(ErrorMessage = "UserName is required.")]
        //[Remote("isuserexists", "account", HttpMethod = "POST", ErrorMessage = "User name already exists.")]
        public string UserName { get; set; }

        /// <summary>
        /// PhoneNumber
        /// </summary>
        [Required(ErrorMessage = "Phone number is required.")]
        [StringLength(15, ErrorMessage = "The {0} must be at least {2} and at max {1} numbers long.", MinimumLength = 8)]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Address1
        /// </summary>
        public string Address1 { get; set; }

        /// <summary>
        /// Address2
        /// </summary>
        //[Required(ErrorMessage = "Address2 is required.")]
        public string Address2 { get; set; }

        /// <summary>
        /// Address3
        /// </summary>
        public string Address3 { get; set; }

        /// <summary>
        /// TermOfUse
        /// </summary>
        [Required(ErrorMessage = "Agree term of use.")]
        public bool TermOfUse { get; set; }

        /// <summary>
        /// PlayerType
        /// </summary>
        [Required(ErrorMessage = "Please choose playet type from previous screen.")]
        public string PlayerType { get; set; }

        public string FileName { get; set; }

        public string PlanType { get; set; }

        public string StateName { get; set; }
        public string CityName { get; set; }
        public string PostCode { get; set; }
        /// <summary>
        /// PhoneNumber
        /// </summary>
        //[Required(ErrorMessage = "Phone number is required.")]
        //[StringLength(15, ErrorMessage = "The {0} must be at least {2} and at max {1} numbers long.", MinimumLength = 8)]
        public string OptPhoneNo { get; set; }

        [Required(ErrorMessage = "Team Name is required.")]
        public string TeamName { get; set; }
        public bool IsEmailNotify { get; set; }
        public bool IsSmsNotify { get; set; }

        public string ReferralCode { get; set; }

    }
}
