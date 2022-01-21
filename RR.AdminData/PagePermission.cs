using System;
using System.Collections.Generic;

namespace RR.AdminData
{
    public partial class PagePermission
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int PageId { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual PageDetail Page { get; set; }
        public virtual AspNetUsers User { get; set; }
    }
}
