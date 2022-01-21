using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace RR.Dto
{
    public partial class NewsDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// News Title
        /// </summary>
        [Required(ErrorMessage = "Title is Required")]
        public string Title { get; set; }

        /// <summary>
        /// Image Path
        /// </summary>
        [Required(ErrorMessage = "PicPath is Required")]
        public string PicPath { get; set; }

        /// <summary>
        /// Video Url
        /// </summary>
        public string VideoUrl { get; set; }

        /// <summary>
        /// Video Path
        /// </summary>
        public string VideoPath { get; set; }

        /// <summary>
        /// Image
        /// </summary>
        public IFormFile Image { get; set; }

        /// <summary>
        /// News Date
        /// </summary>
        public DateTime NewsDate { get; set; }

        /// <summary>
        /// News Tag
        /// </summary>
        [Required(ErrorMessage = "NewsTag is Required")]
        public string NewsTag { get; set; }

        /// <summary>
        /// Show Image
        /// </summary>
        public string ShowImage { get; set; }

        /// <summary>
        /// Show Video
        /// </summary>
        public string ShowVideo { get; set; }

        /// <summary>
        /// News Content
        /// </summary>
        [Required(ErrorMessage = "News Content is Required")]
        public string NewsContent { get; set; }

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
    }
}
