using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RR.Data
{
    public partial class SimpleTeamBull
    {
        [Key]
        public int TeamCompetitorId { get; set; }
        public int SimpleTeamId { get; set; }
        public string CompetitorId { get; set; }
        public decimal CompetitorPoint { get; set; }

        public virtual SimpleTeam SimpleTeam { get; set; }
    }
}
