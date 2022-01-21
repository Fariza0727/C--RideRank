using System.ComponentModel.DataAnnotations;

namespace RR.Dto
{
     public class ForgetPasswordDto
     {
          /// <summary>
          /// Email Address
          /// </summary>
          [Required]
          [EmailAddress]
          public string Email { get; set; }

          public int UserId { get; set; }    
     }
}
