using System;
using System.Collections.Generic;

namespace RR.AdminData
{
    public partial class Award
    {
        public Award()
        {
            ContestWinner = new HashSet<ContestWinner>();
        }

        public long Id { get; set; }
        public int AwardTypeId { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public string Image { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public bool IsDelete { get; set; }

        public virtual AwardType AwardType { get; set; }
        public virtual ICollection<ContestWinner> ContestWinner { get; set; }
    }
}
