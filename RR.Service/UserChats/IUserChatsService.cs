using RR.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RR.Service
{
    public interface IUserChatsService :IDisposable
    {
        /// <summary>
        /// Send Message
        /// </summary>
        /// <param name="userChat"></param>
        /// <returns>true/false</returns>
        Task<bool> SendMessage(UserChatMessageDto userChat);

        /// <summary>
        /// Send Group Message
        /// </summary>
        /// <param name="userChat"></param>
        /// <returns>true/false</returns>
        Task<bool> SendGroupMessage(UserChatMessageDto userChat);

        /// <summary>
        /// Get Message
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="AspUserId_ConnectedUser"></param>
        /// <param name="isSeened"></param>
        /// <returns>UserConversationDto</returns>
        Task<UserConversationDto> GetMessage(string userId, string AspUserId_ConnectedUser, bool isSeened = false);

        /// <summary>
        /// Get Contest Message
        /// </summary>
        /// <param name="ContestId"></param>
        /// <returns>UserConversationDto</returns>
        Task<UserConversationDto> GetContestMessage(ChatInvokeRequestDto ContestId);

        /// <summary>
        /// Get Chat Users
        /// </summary>
        /// <param name="AspUserId"></param>
        /// <param name="contestId"></param>
        /// <returns>List<ChatUsersLiteDto></returns>
        Task<List<ChatUsersLiteDto>> GetChatUsers(string AspUserId, ChatUserInvokeRequestDto request = null);

    }
}
