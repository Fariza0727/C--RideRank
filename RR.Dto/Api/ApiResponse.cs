using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.Dto
{
     public class ApiResponse
     {
          public bool Success { get; set; }

          public dynamic Data { get; set; }

          public dynamic TotalItems { get; set; }

          public string Message { get; set; }

          public string Condition { get; set; }

          public string RedirectPath { get; set; }

          public string IpAddress { get; set; }
     }
}
