using System;
using System.Collections.Generic;
using System.Text;

namespace RR.Dto.Event
{
    public class EventCompleteDto
    {
        public int Id { get; set; }
        public string PBRId { get; set; }
        public string RId { get; set; }
        public bool WinningDistributed { get; set; }
    }
}
