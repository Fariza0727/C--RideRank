using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace RR.Dto
{
   public class PictureManagerDto
    {
        public long Id { get; set; }
        public int? BullId { get; set; }
        public string BullPicture { get; set; }
        public int? RiderId { get; set; }
        public string RiderPicture { get; set; }

        public IEnumerable<SelectListItem> Bulls { get; set; }
        public IEnumerable<SelectListItem> Riders { get; set; }

        public IFormFile File { get; set; }
        public bool IsBull { get; set; }

        public RidermanagerDto RiderManager { get; set; }
    }

    public class PictureManagerLiteDto
    {
        public long Id { get; set; }
        public int? BullId { get; set; }
        public string BullName { get; set; }
        public string BullPicture { get; set; }
        public int? RiderId { get; set; }
        public string RiderPicture { get; set; }
        public string RiderName { get; set; }

    }

  

}
