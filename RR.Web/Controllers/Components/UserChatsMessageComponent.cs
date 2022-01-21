using AppWeb.SignalRHubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RR.Dto;
using RR.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RR.Web.Controllers.Components
{
    [ViewComponent(Name = "UserChatsMessageComponent")]
    public class UserChatsMessageComponent : ViewComponent
    {
        private readonly IUserChatsService _userChats;
        private readonly IHttpContextAccessor _httpContext;
        private IHubContext<ChatHub> _hubContext;

        public UserChatsMessageComponent(IUserChatsService userChats, IHttpContextAccessor httpContext, IHubContext<ChatHub> hubContext)
        {
            _userChats = userChats;
            _httpContext = httpContext;
            _hubContext = hubContext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync(ChatInvokeRequestDto request)
        {
            
            if (request==null || (string.IsNullOrEmpty(request.connectedUserId) && request.ContestId == 0))
                return View(new UserConversationDto
                {
                    UserInfo = new ChatUsersLiteDto { Avatar = "/images/RR/user-n.png", Username = "RankRide" },
                    Chats = new List<ChatLiteDto>()
                });

            var userId = _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            UserConversationDto chats = new UserConversationDto();
            if (request.ContestId > 0)
            {
                chats = await _userChats.GetContestMessage(request);
            }
            else
            {
                chats = await _userChats.GetMessage(userId, request.connectedUserId, request.IsSeen);
                await _hubContext.Clients.Client(request.connectedUserId).SendAsync("SeenedMessages", userId);
            }

            
            return View(chats);
        }

       

    }
}
