using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace RR.Dto
{
     public partial class NewsLiteDto
     {
          /// <summary>
          /// Id
          /// </summary>
          public int Id { get; set; }

          /// <summary>
          /// News Title
          /// </summary>
          public string Title { get; set; }
     }
}
