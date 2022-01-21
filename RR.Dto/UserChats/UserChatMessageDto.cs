using System;
using System.Collections.Generic;
using System.Text;

namespace RR.Dto
{
    public class UserChatMessageDto
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string Message { get; set; }
        public long ContestId { get; set; }
        public string ConnectedUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool? IsSeen { get; set; }
        public bool? IsReceived { get; set; }
    }
}
