using System.ComponentModel.DataAnnotations;

namespace RR.Dto
{
     public class PasswordRequestDto
     {
          [Required]
          public string Code { get; set; }

          [Required]
          [RegularExpression(@"/^([a-zA-Z0-9_\.\-\+])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/",
           ErrorMessage = "Email Address is not valid")]
          public string Email { get; set; }

          public bool IsUsed { get; set; }
     }
}
