using System.ComponentModel.DataAnnotations;

namespace RR.Dto
{
     public class LoginDto
     {
          /// <summary>
          /// Email Address
          /// </summary>
          [Required]
          [EmailAddress]
          [Display(Name = "Email")]
          public string Email { get; set; }

          /// <summary>
          /// Password
          /// </summary>
          [Required]
          [DataType(DataType.Password)]
          [Display(Name = "Password")]
          public string Password { get; set; }

          /// <summary>
          /// Return Url
          /// </summary>
          public string ReturnUrl { get; set; }

          /// <summary>
          /// Remember Credentials
          /// </summary>
          [Display(Name = "Remember me?")]
          public bool Rememberme { get; set; }    
     }
}
