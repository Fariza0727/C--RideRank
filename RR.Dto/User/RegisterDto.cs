using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RR.Dto
{
    public class RegisterDto  
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Email Address
        /// </summary>
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// Full Name
        /// </summary>
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        /// <summary>
        /// User Name
        /// </summary>
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$@!%&*?])[A-Za-z\d#$@!%&*?].{7,}$",
           ErrorMessage = "Password Should contain atleast 8 characters with 1 special character,1 lowercase and 1 uppercase character")]
       // [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        /// <summary>
        /// Confirm Password
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Page List
        /// </summary>
        [Required]
        public List<PageDto> PageList { get; set; }

        
    }
}
