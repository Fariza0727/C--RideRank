using System;
using System.Collections.Generic;
using System.Text;

namespace RR.Dto
{
   public class ShopifyResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string JsonObject { get; set; }
    }
}
