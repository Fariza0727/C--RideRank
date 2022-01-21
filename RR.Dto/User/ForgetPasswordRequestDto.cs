using System.ComponentModel.DataAnnotations;

namespace RR.Dto
{
     public class ForgetPasswordRequestDto
     {
          public int Id { get; set; }
          public string UrlId { get; set; }

          public string UserId { get; set; }
     }
}
