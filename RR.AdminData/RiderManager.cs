using System;
using System.Collections.Generic;

namespace RR.AdminData
{
    public partial class RiderManager
    {
        public long Id { get; set; }
        public int RiderId { get; set; }
        public string Sociallink { get; set; }
        public string Icon { get; set; }
        public string Type { get; set; }
    }
}
