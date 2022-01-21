using System.ComponentModel.DataAnnotations;

namespace RR.Dto
{
     public class SubscribeDto
     {
          /// <summary>
          /// Id
          /// </summary>
          public int Id { get; set; }

          /// <summary>
          /// Email
          /// </summary>
          [Required(ErrorMessage = "Please enter email.")]
          [EmailAddress(ErrorMessage = "Please enter valid email address.")]
          public string Email { get; set; }

          public string CreatedOn { get; set; }
     }
}
