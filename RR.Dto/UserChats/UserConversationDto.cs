using System;
using System.Collections.Generic;
using System.Text;

namespace RR.Dto
{
   public class UserConversationDto
    {
        public ChatUsersLiteDto UserInfo { get; set; }
        public List<ChatLiteDto> Chats { get; set; }
    }
}
