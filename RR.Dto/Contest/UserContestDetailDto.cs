using System;
using System.Collections.Generic;
using System.Text;

namespace RR.Dto
{
   public class UserContestDetailDto
    {
        public UserContestDetailDto()
        {
            this.Tiers = new List<ContestTiers>();
        }
        public string Username { get; set; }
        public string Title { get; set; }
        public decimal JoiningFee { get; set; }
        public List<ContestTiers> Tiers { get; set; }
    }
    public class ContestTiers
    {
        public string Title { get; set; }
        public IEnumerable<RiderContestLiteDto> Riders { get; set; }
        public IEnumerable<BullContestLiteDto> Bulls { get; set; }

    }
}
