using System;
using System.Collections.Generic;
using System.Text;

namespace RR.Dto
{
   public class ChatingDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string ConnectedUserid { get; set; }
        public string Avatar { get; set; }
        public string Message { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Time { get; set; }

        public long contestId { get; set; }
    }
}
