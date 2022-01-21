using System;
using System.Collections.Generic;

namespace RR.StaticData
{
    public partial class PayoutBasic
    {
        public int Id { get; set; }
        public int RowId { get; set; }
        public string Sanction { get; set; }
        public int Position { get; set; }
        public int PlaceTTL { get; set; }
        public decimal PayPerc { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
