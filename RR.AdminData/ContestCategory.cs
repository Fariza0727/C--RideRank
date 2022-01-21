using System;
using System.Collections.Generic;

namespace RR.AdminData
{
    public partial class ContestCategory
    {
        public ContestCategory()
        {
            Contest = new HashSet<Contest>();
        }

        public int Id { get; set; }
        public string CategoryName { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<Contest> Contest { get; set; }
    }
}
