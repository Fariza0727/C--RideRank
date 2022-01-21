using RR.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace RR.Dto
{
   public class ChatLiteDto
    {
        public long Id { get; set; }
        public string AspUserId { get; set; }
        public string Username { get; set; }
        public string Avatar { get; set; }
        public string Message { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? SeenDate { get; set; }
        public bool? isSeen { get; set; }
        public bool? isReveived { get; set; }
        public bool IsConnectedUser { get; set; }
        public long ContestId { get; set; }
        public string ChatAgo { get; set; }
    }
}
