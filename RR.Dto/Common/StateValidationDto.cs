using System.ComponentModel.DataAnnotations;

namespace RR.Dto
{
     public class StateValidationDto
     {
          [Required(ErrorMessage = "State is required.")]
          public string State { get; set; }

          [Required(ErrorMessage = "ZipCode is required.")]
          [RegularExpression("^[0-9]{5}(?:-[0-9]{4})?$", ErrorMessage = "Enter Valid ZipCode")]
          public string ZipCode { get; set; }

          [Required(ErrorMessage = "City is required.")]
          public string City { get; set; }

          [Required(ErrorMessage = "Birthday Date is required.")]
          public string Date { get; set; }
     }
}
