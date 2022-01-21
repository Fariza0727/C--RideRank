using System;
using System.Collections.Generic;

namespace RR.Data
{
    public partial class PasswordRequest
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string UserEmail { get; set; }
        public bool IsUsed { get; set; }
    }
}
