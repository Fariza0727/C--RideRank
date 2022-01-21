using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace RR.Dto
{
    public class EventSearchDto
    {
        public int EventId { get; set; }
        public int ContestId { get; set; }

        public List<SelectListItem> EventList { get; set; }
    }
}
