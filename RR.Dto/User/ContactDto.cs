using System;
using System.ComponentModel.DataAnnotations;

namespace RR.Dto
{
     public class ContactDto
     {
          /// <summary>
          /// User Name
          /// </summary>
          [Required(ErrorMessage = "Name is required.")]
          public string Name { get; set; }

          /// <summary>
          /// Phone Number
          /// </summary>
          [Required(ErrorMessage = "Phone is required.")]
          public string Phone { get; set; }

          /// <summary>
          /// Message
          /// </summary>
          [Required(ErrorMessage = "Message is required.")]
          public string Message { get; set; }

          /// <summary>
          /// Email Address
          /// </summary>
          [Required(ErrorMessage = "Email is required.")]
          [EmailAddress(ErrorMessage = "Enter valid email address.")]
          public string Email { get; set; }

          /// <summary>
          /// Created Date
          /// </summary>
          public DateTime CreatedDate { get; set; }
     }
}
