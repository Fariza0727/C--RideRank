using System;
using System.Collections.Generic;

namespace RR.AdminData
{
    public partial class AwardType
    {
        public AwardType()
        {
            Award = new HashSet<Award>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsDelete { get; set; }
        public string CreatedBy { get; set; }

        public virtual ICollection<Award> Award { get; set; }
    }
}
