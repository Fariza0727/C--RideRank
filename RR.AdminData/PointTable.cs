using System;
using System.Collections.Generic;

namespace RR.AdminData
{
    public partial class PointTable
    {
        public long Id { get; set; }
        public int PointFor { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
