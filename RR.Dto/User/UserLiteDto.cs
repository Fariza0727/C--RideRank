using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace RR.Dto
{
     public class UserLiteDto
     {
          /// <summary>
          /// Id
          /// </summary>
          public long Id { get; set; }

          /// <summary>
          /// User Name
          /// </summary>
          public string UserName { get; set; }

          /// <summary>
          /// Email Address
          /// </summary>
          public string Email { get; set; }
     }
}
