using System;
using System.ComponentModel.DataAnnotations;

namespace RR.Dto
{
     public class RegisterUserDto
     {
          /// <summary>
          /// FullName
          /// </summary>
          [Required(ErrorMessage = "Full Name is required.")]
          public string FullName { get; set; }

          /// <summary>
          /// Email
          /// </summary>
          /// 
          [Required(ErrorMessage = "Email is required.")]
          // [Remote("IsAlreadySigned", "AccountApi", HttpMethod = "POST", ErrorMessage = "EmailId already exists in database.")]
          //[RegularExpression(@"/^([a-zA-Z0-9_\.\-\+])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/",
          // ErrorMessage = "Email Address is not valid")]
          [EmailAddress(ErrorMessage = "Enter valid email address.")]
          public string Email { get; set; }

          /// <summary>
          /// Password
          /// </summary>    
          [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$@!%&*?])[A-Za-z\d#$@!%&*?].{7,}$",
           ErrorMessage = "Password Should contain atleast 8 characters with 1 special character,1 lowercase and 1 uppercase character")]
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
          public string DateOfBirth { get; set; }

          /// <summary>
          /// UserName
          /// </summary>
          [Required(ErrorMessage = "UserName is required.")]
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
          [Required(ErrorMessage = "Address1 is required.")]
          public string Address1 { get; set; }

          /// <summary>
          /// Address2
          /// </summary>
          [Required(ErrorMessage = "Address2 is required.")]
          public string Address2 { get; set; }

          /// <summary>
          /// Address3
          /// </summary>
          public string Address3 { get; set; }

          /// <summary>
          /// TermOfUse
          /// </summary>
          //[Required(ErrorMessage = "Agree term of use.")]
          public bool TermOfUse { get; set; }

          /// <summary>
          /// PlayerType
          /// </summary>
          //[Required(ErrorMessage = "Please choose playet type from previous screen.")]
          public string PlayerType { get; set; }

          public string FileName { get; set; }
         
          public string PlanType { get; set; }

          //#region PayPal Response
          ///// <summary>
          ///// PayPal Response
          ///// </summary>
          //public bool IsSuccess { get; set; }

          //public int Result { get; set; }

          //[Required(ErrorMessage = "Transaction Referenced Number is required")]
          //public string PnRef { get; set; }

          //[Required(ErrorMessage = "Response Message From Transction is required")]
          //public string RespMsg { get; set; }

          //[Required(ErrorMessage = "Authentication code is required")]
          //public string AuthCode { get; set; }
          //#endregion

          //#region Transaction Dto

          //public string UserId { get; set; }
          //public int ContestId { get; set; }
          //public decimal? ContestFee { get; set; }


          //public string ExpiryDate { get; set; }
          //public int? AwardType { get; set; }
          //public int TokenCount { get; set; }
          //public bool IsToken { get; set; }

          //[Required(ErrorMessage = "PaymentMode is required")]
          //public string PaymentMode { get; set; }

          //public bool IsUpgrade = false;
          //public string PaymentMadeFor { get; set; }
          //public string TokenWillGet { get; set; }

          //[Required(ErrorMessage = "Enter expiry month.")]
          //public string ExpiryMonth { get; set; }
          //[Required(ErrorMessage = "Enter expiry year.")]
          //public string ExpiryYear { get; set; }
          //#endregion

     }
}
