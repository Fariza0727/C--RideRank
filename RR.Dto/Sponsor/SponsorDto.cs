using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace RR.Dto
{
     public class SponsorDto
     {
          /// <summary>
          /// Id
          /// </summary>
          public int Id { get; set; }

          /// <summary>
          /// SponsorName
          /// </summary>
          [Required(ErrorMessage = "Sponsor Name is Required")]
          public string SponsorName { get; set; }

          /// <summary>
          /// SponsorLogo
          /// </summary>
          [Required(ErrorMessage = "Logo is Required")]
          public string SponsorLogo { get; set; }

          /// <summary>
          /// Show Image
          /// </summary>
          public string ShowImage { get; set; }

          /// <summary>
          /// UpdatedDate
          /// </summary>
          public DateTime? UpdatedDate { get; set; }

          public string CreatedBy { get; set; }

          public DateTime CreatedDate { get; set; }

          public string UpdatedBy { get; set; }

          /// <summary>
          /// IsActive
          /// </summary>
          public bool IsActive { get; set; }

          /// <summary>
          /// Image
          /// </summary>
          //[Required(ErrorMessage = "Image is Required")]

          public IFormFile Image { get; set; }
        public string WebUrl { get; set; }
    }
}
