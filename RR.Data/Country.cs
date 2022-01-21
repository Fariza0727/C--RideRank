using System;
using System.Collections.Generic;

namespace RR.Data
{
    public partial class Country
    {
        public Country()
        {
            State = new HashSet<State>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        public virtual ICollection<State> State { get; set; }
    }
}
