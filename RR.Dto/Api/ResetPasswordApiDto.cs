using System.ComponentModel.DataAnnotations;

namespace RR.Dto
{
     public class ResetPasswordApiDto
     {
          /// <summary>
          /// Password
          /// </summary>
          [Required]
          [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$@!%&*?])[A-Za-z\d#$@!%&*?].{7,}$",
           ErrorMessage = "Password Should contain atleast 8 characters with 1 special character,1 lowercase and 1 uppercase character")]
          [DataType(DataType.Password)]
          public string Password { get; set; }

          /// <summary>
          /// Confirm Password
          /// </summary>
          [DataType(DataType.Password)]
          [Display(Name = "Confirm password")]
          [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
          public string ConfirmPassword { get; set; }

          /// <summary>
          /// Generated Code
          /// </summary>
          [Required(ErrorMessage = "Code is required")]
          public string Code { get; set; }
     }
}
