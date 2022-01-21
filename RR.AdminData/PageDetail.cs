using System;
using System.Collections.Generic;

namespace RR.AdminData
{
    public partial class PageDetail
    {
        public PageDetail()
        {
            PagePermission = new HashSet<PagePermission>();
        }

        public int Id { get; set; }
        public string PageName { get; set; }
        public string PageBaseUrl { get; set; }
        public string PageUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual ICollection<PagePermission> PagePermission { get; set; }
    }
}
