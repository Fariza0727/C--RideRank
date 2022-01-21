using System;
using System.Collections.Generic;

namespace RR.Data
{
    public partial class UserChatMessages
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string Message { get; set; }
        public long ContestId { get; set; }
        public string ConnectedUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool? IsSeen { get; set; }
        public bool? IsReceived { get; set; }
        public DateTime? LastSeenDate { get; set; }

        public virtual AspNetUsers ConnectedUser { get; set; }
        public virtual AspNetUsers User { get; set; }
    }
}
