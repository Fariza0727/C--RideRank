
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RR.Dto;
using RR.Dto.Contest;
using RR.Dto.Team;
using RR.Service;
using RR.Service.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RR.Web.Controllers
{
    public class ContestController : Controller
    {
        #region Constructor     
        private readonly IContestService _contestService;
        private readonly IEventService _eventService;
        private readonly IUserService _userService;
        private readonly ITeamService _teamService;
        private readonly UserManager<IdentityUser> _userManager;

        public ContestController(IContestService contestService,
             IEventService eventService,
             IUserService userService,
             ITeamService teamService,
              UserManager<IdentityUser> userManager)
        {
            _contestService = contestService;
            _eventService = eventService;
            _userManager = userManager;
            _userService = userService;
            _teamService = teamService;
        }

        #endregion

        /// <summary>
        /// Get Contest Of Particular Event
        /// </summary>
        /// <param name="eventId">Event Id</param>
        /// <returns></returns>
        [Route("event/contest/{eventId}")]
        public async Task<IActionResult> GetContestOfEvent(int eventId, long contestId = 0)
        {
            
            var eventDetail = await _eventService.GetEventById(eventId);

            if (TempData["NoEnoughToken"] != null)
            {
                ViewBag.NoEnoughToken = "true";
            }

            ViewBag.EventStatus = (eventDetail.PerfTime < DateTime.Now ? "Completed" : "");
            var contest = await _contestService.GetContestOfEvent(eventId, eventDetail.Title);
            var user = User.Identity.IsAuthenticated ? await _userManager.GetUserAsync(HttpContext.User) : null;

            /*var joinedContest = await _contestService.GetJoinedContestListByEventId(eventId, user != null ? user.Id : "");*/
            var userDetail = await _userService.GetUserDetail(user != null ? user.Id : "");
            ViewBag.WalletToken = userDetail != null ? userDetail.WalletToken.HasValue ? userDetail.WalletToken : 0 : 0;
            ViewBag.ContestId = contestId;
            /*ViewBag.JoinedContest = joinedContest;*/
            int teamId = 0;
            if (user != null)
            {
                teamId = await _teamService.GetTeamIdByEventIdUserId(eventId, user.Id);
            }
            int contestIdd = 0;
            foreach(var item in contest.Item1)
            {
                contestIdd = (int)item.ContestId;
            }
            ViewBag.TeamId = teamId;
            ViewBag.ContestIdd = contestIdd;
            ViewBag.EventId = eventId;
            var teamFormData = await _teamService.EventPlayerDataById(eventId, contestIdd, teamId);
            return View(new Tuple<IEnumerable<ContestLiteDto>, decimal, string, EventDto, TeamFormationDetailDto>(contest.Item1, contest.Item2, contest.Item3, eventDetail, teamFormData));
        }

        /// <summary>
        /// FilterContest
        /// </summary>
        /// <param name="eventId">Event Id</param>
        /// <param name="priceFrom">Price From</param>
        /// <param name="priceTo">Price To</param>
        /// <param name="priceFilter">Sorting Price</param>
        /// <returns></returns>
        public async Task<IActionResult> FilterContest(int eventId, int priceFrom, int priceTo, int priceFilter)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var joinedContest = await _contestService.GetJoinedContestListByEventId(eventId, user != null ? user.Id : "");

            var contest = await _contestService.FilterContest(eventId, priceFrom, priceTo, priceFilter);

            return PartialView("~/Views/Shared/_ContestList.cshtml", new Tuple<IEnumerable<ContestLiteDto>, List<long>>(contest, joinedContest));
        }

        /// <summary>
        /// Get Contest Awards
        /// </summary>
        /// <param name="contestId">Contest Id</param>
        /// <returns></returns>
        public async Task<IActionResult> GetContestAwards(int contestId, int eventId)
        {
            var contestAward = await _contestService.GetContestAwards(contestId, eventId);
            return PartialView(contestAward);
        }

        /// <summary>
        /// Joined User Contest
        /// </summary>
        /// <param name="contestId">Contest Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("joinedusercontests/{contestId}/{eventId}/{eventStatus?}")]
        public async Task<IActionResult> JoinedUserContest(long contestId, int eventId, string eventStatus = "")
        {
            
            var eventDetail = await _eventService.GetEventById(eventId);
            ViewBag.ContestId = contestId;
            ViewBag.EventId = eventId;
            ViewBag.EventName = eventDetail.Title;
            ViewBag.EventStatus = eventStatus;
            ViewBag.IsChatApplicable = false;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {

            ViewBag.IsChatApplicable = ((await _userService.GetUserDetail(userId)).IsPaidMember == true && 
                                        (await _contestService.GetContestOfCurrentUser(userId)).Item1.Where(r => r.ContestId == contestId).Count() > 0 );
            }

            return View(eventDetail);
        }

        [HttpPost]
        [Route("joinedcontestsajax/{contestId}/{eventId}")]
        public async Task<JsonResult> JoinedUserContestAjax(long contestId, int eventId)
        {
            
            var draw = Request.Form["draw"];
            var start = Request.Form["start"];
            var length = Request.Form["length"];
            
            var joinedUsers = await _contestService.JoinedUserContestAjax(contestId, eventId, Convert.ToInt32(start), Convert.ToInt32(length));
            //var data = joinedUsers.Item1.Skip(Convert.ToInt32(start)).Take(Convert.ToInt32(length));
            var data = joinedUsers.Item1;
            int recordsTotal = joinedUsers.Item2;
            var temp = new Tuple<JoinUserContestLiteDto, JoinUserContestLiteDto, JoinUserContestLiteDto>[3];
            temp[0] = new Tuple<JoinUserContestLiteDto, JoinUserContestLiteDto, JoinUserContestLiteDto>(data.Count() > 0? data.ElementAt(0): null, data.Count() > 1 ? data.ElementAt(1) : null, data.Count() > 2 ? data.ElementAt(2) : null);
            temp[1] = new Tuple<JoinUserContestLiteDto, JoinUserContestLiteDto, JoinUserContestLiteDto>(data.Count() > 3 ? data.ElementAt(3) : null, data.Count() > 4 ? data.ElementAt(4) : null, data.Count() > 5 ? data.ElementAt(5) : null);
            temp[2] = new Tuple<JoinUserContestLiteDto, JoinUserContestLiteDto, JoinUserContestLiteDto>(data.Count() > 6 ? data.ElementAt(6) : null, data.Count() > 7 ? data.ElementAt(7) : null, data.Count() > 8 ? data.ElementAt(8) : null);



            return Json(new
            {
                draw,
                recordsFiltered = recordsTotal,
                recordsTotal,
                data = temp,
            });
        }
        [HttpPost]
        [Route("joinedusercontests/{contestId}/{eventId}/{eventName}")]
        public async Task<JsonResult> JoinedUserContestPaging(long contestId, int eventId, string eventName)
        {
            eventName = eventName.Replace("-", " ").Replace("%2F", "/");
            var draw = Request.Form["draw"];
            var start = Request.Form["start"];
            var length = Request.Form["length"];
            //Global search field


            var search = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = !(string.IsNullOrEmpty(length)) ? Convert.ToInt32(length) : 0;
            int skip = !(string.IsNullOrEmpty(start)) ? Convert.ToInt32(start) : 0;
            var sort = Request.Form["order[0][dir]"];
            var column = Convert.ToInt32(Request.Form["order[0][column]"]);
            var joinedUsers = await _contestService.JoinedUserContest(contestId, eventId, skip / pageSize, pageSize, column, search, sort);
            var data = joinedUsers.Item1.Skip(Convert.ToInt32(start)).Take(Convert.ToInt32(length));
            int recordsTotal = joinedUsers.Item1.Count;
            var members = joinedUsers.Item2;
            var contestName = joinedUsers.Item3;

            eventName = eventName.Replace(" ", "-").Replace("/", "%2F");
            ViewBag.ContestId = contestId;
            ViewBag.EventId = eventId;
            ViewBag.EventName = eventName;

            return Json(new
            {
                draw,
                recordsFiltered = recordsTotal,
                recordsTotal,
                data,
                members,
                contestName
            });
        }
        
        [HttpPost]
        [Route("getcontestofcurrentuserajax")]
        public async Task<JsonResult> GetCurrentUserContestAjax()
        {

            var draw = Request.Form["draw"];
            var start = Request.Form["start"];
            var length = Request.Form["length"];
            //Global search field

            var contestDetail = await _contestService.GetContestOfCurrentUser(_userManager.GetUserId(HttpContext.User), Convert.ToInt32(start), Convert.ToInt32(length));

            var data = contestDetail.Item1;
            int recordsTotal = contestDetail.Item2;
            var temp = new Tuple<UserContestLiteDto, UserContestLiteDto, UserContestLiteDto>[3];
            temp[0] = new Tuple<UserContestLiteDto, UserContestLiteDto, UserContestLiteDto>(data.Count() > 0 ? data.ElementAt(0) : null, data.Count() > 1 ? data.ElementAt(1) : null, data.Count() > 2 ? data.ElementAt(2) : null);
            temp[1] = new Tuple<UserContestLiteDto, UserContestLiteDto, UserContestLiteDto>(data.Count() > 3 ? data.ElementAt(3) : null, data.Count() > 4 ? data.ElementAt(4) : null, data.Count() > 5 ? data.ElementAt(5) : null);
            temp[2] = new Tuple<UserContestLiteDto, UserContestLiteDto, UserContestLiteDto>(data.Count() > 6 ? data.ElementAt(6) : null, data.Count() > 7 ? data.ElementAt(7) : null, data.Count() > 8 ? data.ElementAt(8) : null);
            


            return Json(new
            {
                draw,
                recordsFiltered = recordsTotal,
                recordsTotal,
                data = temp,
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("joinedcontestschatroom")]
        public IActionResult JoinedContestsChatroom(long contestId, int eventId, string eventName, string eventStatus)
        {
            eventName = eventName.Replace(" ", "-").Replace("/", "%2F");
            ViewBag.ContestId = contestId;
            ViewBag.EventId = eventId;
            ViewBag.EventName = eventName;
            ViewBag.EventStatus = eventStatus;
            ViewBag.userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View();
        }

        /// <summary>
        /// Get Contest Of User
        /// </summary>
        /// <param name="contestId">Contest Id</param>
        /// <param name="teamId">Team Id</param>
        /// <returns></returns>
        [Route("usercontest/{userId}/{contestId}/{teamId}")]
        public async Task<IActionResult> GetContestOfUser(string userId, int contestId, int teamId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var userInfo = await _userService.GetUserDetail(!string.IsNullOrEmpty(userId) ? userId : user != null ? user.Id : "");

                var contestDetail = await _contestService.GetContestById(contestId);
                var event_ = await _eventService.GetEventById(contestDetail.EventId);

                var joinedContestDetailOfUser = await _contestService.GetContestDetailOfCurrentUser(contestDetail.EventId, contestId, teamId);
                ViewBag.EventId = event_.Id;
                ViewBag.ContestId = contestId;
                ViewBag.EventStatus = "";
                ViewBag.EventName = event_.Title;
                ViewBag.TeamName = userInfo.TeamName;
                ViewBag.Position = joinedContestDetailOfUser.Item4;
                ViewBag.NumberOfEntries = joinedContestDetailOfUser.Item3;

                return View(new Tuple<IEnumerable<RiderContestLiteDto>, IEnumerable<BullContestLiteDto>, string, string, decimal>(
                            joinedContestDetailOfUser.Item1,
                            joinedContestDetailOfUser.Item2,
                            userInfo.UserName,
                            contestDetail.Title,
                            contestDetail.JoiningFee));

                
            }
            catch (Exception ex)
            {
                return RedirectToAction("index", "home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetPrivateContest(JoinPrivateContestDto model)
        {
            if (!string.IsNullOrEmpty(model.Code))
            {
                var privateContest = await _contestService.GetContestByUniqueCode(model.Code);

                if (privateContest != null)
                {
                    if (string.IsNullOrEmpty(privateContest.Message))
                    {
                        return Content("<div class='alert alert-success alert-dismissible' role='alert' style='background-color:#59ca7c;'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span><input type='hidden' id='contestId' value='" + privateContest.ContestId + "' /><input type='hidden' id='eventId' value='" + privateContest.EventId + "' /></button><strong>Success!!!</strong> Please wait, redirecting to team formation page.</div>");
                    }
                    else
                    {
                        return Content("<div class='alert alert-danger alert-dismissible' role='alert' style='background-color:#fd5454;'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Oops!!!</strong> This contest has been filled up.</div>");
                    }
                }
                return Content("<div class='alert alert-danger alert-dismissible' role='alert' style='background-color:#fd5454;'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>N/A</strong> No Contest Available Yet</div>");
            }
            else
            {
                return Content("<div class='alert alert-danger alert-dismissible' role='alert' style='background-color:#fd5454;'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Oops!!!</strong> Please enter private contest code.!!</div>");
            }
        }
    }
}