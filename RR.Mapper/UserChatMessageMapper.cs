using RR.Data;
using RR.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RR.Mapper
{
   public class UserChatMessageMapper
    {
        /// <summary>
        /// Map Entity
        /// </summary>
        /// <param name="news">List Of Chats</param>
        /// <returns>List Of UserChatDto</returns>
        public static IEnumerable<UserChatMessageDto> Map(IEnumerable<UserChatMessages> userChats)
        {
            return userChats.Select(p => MapDto(p));
        }

        /// <summary>
        /// Map Dto
        /// </summary>
        /// <param name="newsDto">The UserChatMessages</param>
        /// <returns>The UserChatMessageDto</returns>
        public static UserChatMessageDto MapDto(UserChatMessages userChat)
        {
            return new UserChatMessageDto
            {
                Id = userChat.Id,
                ConnectedUserId = userChat.ConnectedUserId,
                CreatedDate = userChat.CreatedDate,
                ContestId = userChat.ContestId,
                IsReceived = userChat.IsReceived,
                IsSeen = userChat.IsSeen,
                Message = userChat.Message,
                UserId = userChat.UserId,
                
            };
        }
    }
}

