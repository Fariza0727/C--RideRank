using System;
using System.Collections.Generic;

namespace RR.Data
{
    public partial class NewsLetterSubscribe
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
