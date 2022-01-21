using System;
using System.Collections.Generic;

namespace RR.Data
{
    public partial class ForgetPasswordRequest
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UrlId { get; set; }
    }
}
