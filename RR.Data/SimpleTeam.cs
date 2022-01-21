using System;
using System.Collections.Generic;

namespace RR.Data
{
    public partial class SimpleTeam
    {
        public SimpleTeam()
        {
            
            SimpleTeamBull = new HashSet<SimpleTeamBull>();
            
        }

        public int Id { get; set; }
        public string UserId { get; set; }
        public int EventId { get; set; }
        public decimal SimpleTeamPoint  { get; set; }
        
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        public virtual AspNetUsers User { get; set; }
       
        public virtual ICollection<SimpleTeamBull> SimpleTeamBull { get; set; }
        
    }
}
