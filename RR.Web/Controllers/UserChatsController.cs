using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AppWeb.SignalRHubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RR.Dto;
using RR.Service;
using RR.Service.User;
using RR.Web.SignalRHubs;


namespace RR.Web.Controllers
{

    public class UserChatsController : Controller
    {
        private IHubContext<ChatHub> _hubContext;
        private readonly IUserChatsService _chatsService;
        private readonly IUserService _userService;
        private readonly SignInManager<IdentityUser> _signinManager;

        public UserChatsController(
            IHubContext<ChatHub> hubContext,
            IUserChatsService chatsService,
            IUserService userService,
            SignInManager<IdentityUser> signinManager
            )
        {
            _hubContext = hubContext;
            _chatsService = chatsService;
            _userService = userService;
            _signinManager = signinManager;
        }

        [HttpGet("/userchats")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(ChatingDto chatingDto)
        {

            if (!ModelState.IsValid || (chatingDto.contestId <= 0 && string.IsNullOrEmpty(chatingDto.ConnectedUserid)))
                return Json(new { status = false, message = "Validation error." });

            try
            {
                var date_ = DateTime.Now;
                chatingDto.CreatedDate = date_;
                chatingDto.Time = date_.ToString("h:mm tt");

                #region AddMessagetoServer 
                var AspNetUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var connectedUser = await _userService.GetUserDetail(chatingDto.ConnectedUserid);
                bool isReceived = await _chatsService.SendMessage(new UserChatMessageDto
                {
                    ConnectedUserId = chatingDto.ConnectedUserid,
                    ContestId = chatingDto.contestId,
                    CreatedDate = chatingDto.CreatedDate,
                    IsReceived = connectedUser?.IsOnline == true ? true : false,
                    IsSeen = false,
                    Message = chatingDto.Message.
                    Replace(Environment.NewLine, "<br />")
                    .Replace("\r", "<br />")
                    .Replace("\n", "<br />"),
                    UserId = AspNetUserId,
                });
                #endregion
                if (isReceived)
                {
                    if (chatingDto.contestId > 0)
                    {
                        await _hubContext.Clients.Group(chatingDto.contestId.ToString()).SendAsync("ReceiveMessage", chatingDto);

                    }
                    else
                    {
                        foreach (var _connectionId in ChatHub._connections.GetConnections(chatingDto.ConnectedUserid))
                        {
                            await _hubContext.Clients.Client(_connectionId).SendAsync("ReceiveMessage", chatingDto);
                        }
                    }
                }

                return Json(new { status = true });
            }
            catch (Exception e)
            {
                return Json(new { status = false, message = "Oops! something wrong." });
            }

        }

        [HttpPost]
        public async Task<IActionResult> UserConnect(string UserId)
        {
            if (!string.IsNullOrEmpty(UserId))
            {
                await _userService.UpdateLogedInStatus(UserId, true);
            };

            return Json(new { status = true });
        }

        [HttpPost]
        public async Task<IActionResult> UserDisConnect(string UserId)
        {
            if (!string.IsNullOrEmpty(UserId))
            {
                await _userService.UpdateLogedInStatus(UserId, false);
            };

            return Json(new { status = true });
        }

        [Route("chatsingal/{Email}/{Password}")]
        public async Task<IActionResult> ChatSingal(string Email, string Password)
        {

            if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password))
            {
                var _userManager = _signinManager.UserManager;
                var user = await _userManager.FindByEmailAsync(Email);

                if (user == null)
                    return Content("Invalid user");

                if (!(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    return Content("Your Email is not verified. Please verify your email first!! In order to access your account");
                }
                int countLock = await _userManager.GetAccessFailedCountAsync(user);
                int count = 4 - countLock;
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signinManager.PasswordSignInAsync(Email, Password, false, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    if (await isApplicableToChat(user.Id))
                    {
                        await _userService.UpdateLogedInStatus(user.Id, true);
                        var userDetail = await _userService.GetUserDetail(user.Id);
                        return View(userDetail);
                    }
                    else
                    {
                        return Content("Please subscribe to use this feature!");
                    }
                }
                if (result.IsLockedOut)
                {

                    return Content("Your account has been locked. Please contact to support Or Try After Sometime!");
                }
                else
                {
                    return Content("Invalid Credentials." + count + " Attempts Remaining!Please try again!");
                }

            }
            else
            {
                return Content("Invalid UserId!");
            }

        }

        [Route("joinchatroom/{Email}/{Password}/{contestId}/{eventId}/{eventName}")]
        public async Task<IActionResult> JoinChatRoom(string Email, string Password, long contestId, int eventId, string eventName)
        {

            if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password) && contestId > 0 && eventId > 0 && !string.IsNullOrEmpty(eventName))
            {
                var _userManager = _signinManager.UserManager;
                var user = await _userManager.FindByEmailAsync(Email);

                if (user == null)
                    return Content("Invalid user");

                if (await isApplicableToChat(user.Id))
                {
                    if (!(await _userManager.IsEmailConfirmedAsync(user)))
                    {
                        return Content("Your Email is not verified. Please verify your email first!! In order to access your account");
                    }
                    int countLock = await _userManager.GetAccessFailedCountAsync(user);
                    int count = 4 - countLock;
                    // This doesn't count login failures towards account lockout
                    // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                    var result = await _signinManager.PasswordSignInAsync(Email, Password, false, lockoutOnFailure: true);
                    if (result.Succeeded)
                    {
                        await _userService.UpdateLogedInStatus(user.Id, true);
                        eventName = eventName.Replace(" ", "-").Replace("/", "%2F");
                        ViewBag.ContestId = contestId;
                        ViewBag.EventId = eventId;
                        ViewBag.EventName = eventName;
                        ViewBag.EventStatus = "";
                        ViewBag.userId = user.Id;
                        return View();
                    }
                    if (result.IsLockedOut)
                    {
                        return Content("Your account has been locked. Please contact to support Or Try After Sometime!");
                    }
                    else
                    {
                        return Content("Invalid Credentials." + count + " Attempts Remaining!Please try again!");
                    }

                }
                else
                {
                    return Content("Please subscribe  to use this feature!");
                }
            }
            else
            {
                return Content("Invalid request!");
            }


        }

        private async Task<bool> isApplicableToChat(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var userDetail = await _userService.GetUserDetail(userId);
                return await Task.FromResult((userDetail != null && userDetail.IsPaidMember != false));
            }

            return await Task.FromResult(false);
        }

    }
}