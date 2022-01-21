using System;
using System.Collections.Generic;
using System.Text;

namespace RR.Dto
{
    public class ChatUsersLiteDto
    {
        public string AspUserId { get; set; }
        public string Username { get; set; }
        public string Avatar { get; set; }
        public DateTime? LastSeenDate { get; set; }
        public string Lastmessage { get; set; }
        public bool? IsUserOnline { get; set; }
        public int Unseend { get; set; }

        public string ContestName { get; set; }
        public long ContestId { get; set; }
        public string EventName { get; set; }
        public long EventId { get; set; }
        public bool isChatRoom { get; set; }
        public IEnumerable<string> chatRoomUsers { get; set; }
    }
}
