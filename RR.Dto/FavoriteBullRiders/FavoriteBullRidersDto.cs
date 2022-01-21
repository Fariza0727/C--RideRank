using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RR.Dto
{
    public class FavoriteBullRidersDto
    {
        
        public long Id { get; set; }
        [Required]
        public string UserId { get; set; }
        public int? RiderId { get; set; }
        public int? BullId { get; set; }
    }
}
