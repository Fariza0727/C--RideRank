using System;
using System.Collections.Generic;

namespace RR.AdminData
{
    public partial class Contest
    {
        public Contest()
        {
            ContestWinner = new HashSet<ContestWinner>();
        }

        public long Id { get; set; }
        public int EntryFeeTypeId { get; set; }
        public int EventId { get; set; }
        public decimal JoiningFee { get; set; }
        public int Members { get; set; }
        public string WinningTitle { get; set; }
        public int Winners { get; set; }
        public long WinningToken { get; set; }
        public decimal WinningPrice { get; set; }
        public string UniqueCode { get; set; }
        public bool IsPrivate { get; set; }
        public DateTime CreatdDate { get; set; }
        public string CreatedBy { get; set; }
        public bool IsActive { get; set; }
        public bool IsRefunded { get; set; }
        public int ContestCategoryId { get; set; }
        public string Title { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        public virtual ContestCategory ContestCategory { get; set; }
        public virtual ICollection<ContestWinner> ContestWinner { get; set; }
    }
}
