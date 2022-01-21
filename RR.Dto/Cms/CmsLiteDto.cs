using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace RR.Dto
{
     public class CmsLiteDto
     {
          /// <summary>
          /// Id
          /// </summary>
          public int Id { get; set; }

          /// <summary>
          /// Page Name
          /// </summary>
          public string PageName { get; set; }

          /// <summary>
          /// Page Url
          /// </summary>
          public string PageUrl { get; set; }

          /// <summary>
          /// Meta Title
          /// </summary>
          public string MetaTitle { get; set; }

          /// <summary>
          /// Meta Description
          /// </summary>
          public string MetaDescription { get; set; }

          /// <summary>
          /// Meta Keyword
          /// </summary>
          public string MetaKeyword { get; set; }
     }
}
