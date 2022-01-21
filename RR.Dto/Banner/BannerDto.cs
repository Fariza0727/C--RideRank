using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RR.Dto
{
   public  class BannerDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Banner Title
        /// </summary>
      //  [Required(ErrorMessage = "Banner Title is Required!")]
        public string Title { get; set; }

        /// <summary>
        /// Image Path
        /// </summary>
        [Required(ErrorMessage = "PicPath is Required")]
        public string PicPath { get; set; }

        /// <summary>
        /// Image
        /// </summary>
        [Required(ErrorMessage = "Image is Required")]
        public IFormFile Image { get; set; }

        /// <summary>
        /// Page Url
        /// </summary>
      //  [Required(ErrorMessage ="Page Url is Required")]
        public string Url { get; set; }

        /// <summary>
        /// Created By
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Created Date
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Updated By
        /// </summary>
        public string UpdatedBy { get; set; }

        /// <summary>
        /// Updated Date
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        public string ShowImage { get; set; }

    }
}
