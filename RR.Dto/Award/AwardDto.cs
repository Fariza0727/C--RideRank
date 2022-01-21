using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RR.Dto.Award
{
     public class AwardDto
     {
          /// <summary>
          /// Id
          /// </summary>
          public long Id { get; set; }

          /// <summary>
          /// AwardTypeId
          /// </summary>
          [Required(ErrorMessage = "Please choose award type.")]
          public int AwardTypeId { get; set; }

          /// <summary>
          /// AwardTypeName
          /// </summary>
          public string AwardTypeName { get; set; }

          /// <summary>
          /// Message
          /// </summary>
          [Required(ErrorMessage = "Message is required.")]
          public string Message { get; set; }

          /// <summary>
          /// Token
          /// </summary>
          public string Token { get; set; }

          /// <summary>
          /// Image
          /// </summary>
          public string Image { get; set; }

          /// <summary>
          /// CreatedDate
          /// </summary>
          public DateTime CreatedDate { get; set; }

          /// <summary>
          /// CreatedBy
          /// </summary>
          public string CreatedBy { get; set; }

          /// <summary>
          /// AwardTypes
          /// </summary>
          public List<SelectListItem> AwardTypes { get; set; }
     }
}
