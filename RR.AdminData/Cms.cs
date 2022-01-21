using System;
using System.Collections.Generic;

namespace RR.AdminData
{
    public partial class Cms
    {
        public int Id { get; set; }
        public string PageName { get; set; }
        public string PageUrl { get; set; }
        public string PageContent { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeyword { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
