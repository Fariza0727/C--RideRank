using System;
using System.Collections.Generic;

namespace RR.AdminData
{
    public partial class Banner
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string PicPath { get; set; }
        public string Url { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
