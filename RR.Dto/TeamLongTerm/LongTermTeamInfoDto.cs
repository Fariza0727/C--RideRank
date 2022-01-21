using System;
using System.Collections.Generic;
using System.Text;

namespace RR.Dto
{
   public class LongTermTeamInfoDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public decimal TeamPoint { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string TeamIcon { get; set; }
        public string TeamBrand { get; set; }

    }
}
