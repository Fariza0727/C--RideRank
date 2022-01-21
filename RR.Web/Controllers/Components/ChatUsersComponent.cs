using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RR.Dto;
using RR.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RR.Web.Controllers.Components
{
    [ViewComponent(Name = "ChatUsersComponent")]

    public class ChatUsersComponent : ViewComponent
    {
        private readonly IUserChatsService _userChats;
        private readonly IHttpContextAccessor _httpContext;

        public ChatUsersComponent(IUserChatsService userChats, IHttpContextAccessor httpContext)
        {
            _userChats = userChats;
            _httpContext = httpContext;
        }

        /// <summary>
        /// Get paid user 
        /// </summary>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync(ChatUserInvokeRequestDto request = null)
        {
            var userId = _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var users_ = await _userChats.GetChatUsers(userId, request);
            return View(users_);

        }
    }
}
