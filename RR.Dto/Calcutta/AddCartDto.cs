using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Dto.Calcutta
{
    public class AddCartDto
    {
        public int EntId { get; set; }
        public string ParentEventId { get; set; }
        public string EventId { get; set; }
        public string EntryId { get; set; }
        public decimal CalcuttaPrice { get; set; }
    }
}
