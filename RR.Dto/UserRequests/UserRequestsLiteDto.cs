using System;
using System.Collections.Generic;
using System.Text;

namespace RR.Dto
{
    public class UserRequestsLiteDto
    {
        public long Id { get; set; }
        public string RequestNo { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string RequestMessage { get; set; }
        public bool IsApproved { get; set; }
        public string ReturlUrl { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int LongTermTeamId { get; set; }
        public string TeamIcon { get; set; }
        public string TeamBrand { get; set; }
    }
}
