using System;
using System.Collections.Generic;

namespace RR.Data
{
    public partial class ContestUserWinner
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int TeamId { get; set; }
        public long ContestId { get; set; }
        public int EventId { get; set; }
        public long ContestWinnerId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int TeamRank { get; set; }

        public virtual Team Team { get; set; }
    }
}
