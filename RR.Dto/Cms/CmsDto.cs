using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace RR.Dto
{
     public class CmsDto
     {
          /// <summary>
          /// Id
          /// </summary>
          public int Id { get; set; }

          /// <summary>
          /// Page Name
          /// </summary>
          [Remote("PageNameIsExist", "CMS", HttpMethod = "POST", AdditionalFields = "Id", ErrorMessage = "Page Name already exists")]
          [Required(ErrorMessage = "Page Name is required")]
          public string PageName { get; set; }

          /// <summary>
          /// Page Url
          /// </summary>
          [Required(ErrorMessage = "Page Url is required")]
          public string PageUrl { get; set; }

          /// <summary>
          /// Page Content
          /// </summary>
          [Required(ErrorMessage = "Page Content is required")]
          public string PageContent { get; set; }

          /// <summary>
          /// Meta Title
          /// </summary>
          [Required(ErrorMessage = "Meta Title is required")]
          public string MetaTitle { get; set; }

          /// <summary>
          /// Meta Description
          /// </summary>
          [Required(ErrorMessage = "Meta Description is required")]
          public string MetaDescription { get; set; }

          /// <summary>
          /// Meta Keyword
          /// </summary>
          [Required(ErrorMessage = "Meta Keyword is required")]
          public string MetaKeyword { get; set; }

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
          public DateTime UpdatedDate { get; set; }
     }
}
