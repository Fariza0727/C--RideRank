using System;
using System.Collections.Generic;

namespace RR.AdminData
{
    public partial class Sponsor
    {
        public int Id { get; set; }
        public string SponsorName { get; set; }
        public string SponsorLogo { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string WebUrl { get; set; }
    }
}
