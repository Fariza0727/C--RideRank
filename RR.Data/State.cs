using System;
using System.Collections.Generic;

namespace RR.Data
{
    public partial class State
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CountryId { get; set; }
        public bool IsDelete { get; set; }
        public bool IsNovice { get; set; }
        public bool IsIntermediate { get; set; }
        public bool IsPro { get; set; }
        public int Age { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string StateCode { get; set; }

        public virtual Country Country { get; set; }
    }
}
