using AppWeb.SignalRHubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RR.Core;
using RR.Dto;
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
    [Authorize(Roles = "TM, PTM, FTM, NTM")]
    public class TeamController : Controller
    {
        #region Constructor

        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITeamService _teamService;
        private readonly IContestService _contestService;
        private readonly IUserService _userService;
        private readonly IEventService _eventService;
        private readonly ILongTermTeamService _longTermTeam;
        //private IHubContext<ChatHub> _hubContext;
        private AppSettings _appsetting;

        public TeamController(UserManager<IdentityUser> userManager,
             ITeamService teamService,
             IContestService contestService,
             IUserService userService,
             IEventService eventService,
             ILongTermTeamService longTermTeam,
             //IHubContext<ChatHub> hubContext,
             IOptions<AppSettings> appsetting
             )
        {
            _userManager = userManager;
            _teamService = teamService;
            _contestService = contestService;
            _userService = userService;
            _eventService = eventService;
            _longTermTeam = longTermTeam;
            //_hubContext = hubContext;
            _appsetting = appsetting.Value;
        }

        #endregion

        /// <summary>
        /// Team Formation Index Page
        /// </summary>
        /// <returns></returns>
        [Route("team-formation/{contestId}/{eventId}/{teamId}")]
        public async Task<IActionResult> TeamFormation(int contestId, int eventId = 0, int teamId = 0)
        {
            TeamFormationDto formationDto = new TeamFormationDto();
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userInfo = await _userService.GetUserDetail(user.Id);
            var contestInfo = await _contestService.GetContestById(contestId);
            var eventInfo = await _eventService.GetEventById(eventId);

            bool CanEditTeam = (DateTime.Now.AddHours(_appsetting.CanUpdateHr) <= eventInfo.PerfTime);
            if (!CanEditTeam)
            {
                ViewBag.TeamUpdateError = "You can update the team up to 10 minutes before the event starts.";
                return View(formationDto);
            }
            if (contestInfo != null)
            {
                var walletInfo = userInfo.WalletToken.HasValue ? userInfo.WalletToken.Value : 0;
                if (contestInfo.EntryFeeType == "Token" && (contestInfo.JoiningFee > walletInfo))
                {
                    TempData["NoEnoughToken"] = "true";
                    string path = "/event/contest/" + eventInfo.Title.Replace(" ", "-").Replace("/", "%2F") + "/" + eventId;
                    return RedirectToAction("GetContestOfEvent", "Contest", new { eventId = eventId });
                }
            }
            ViewBag.userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.teamId = teamId;
            var joinedContest = await _contestService.GetJoinedContestListByEventId(eventId, user != null ? user.Id : "");

            formationDto = await _teamService.EventPlayersById(eventId, contestId, teamId);
            if (teamId == 0)
                formationDto.IsAlreadyJoined = joinedContest.Contains(contestId) ? true : false;

            return View(formationDto);
        }

       
        /// <summary>
        /// Create Team of Player
        /// </summary>
        /// <param name="teamData">The Team data</param>
        /// <param name="eventId">Event Id</param>
        /// <returns></returns>
        public async Task<ActionResult> CreateTeam(string teamData, int eventId)
        {
            try
            {
                var eventInfo = await _eventService.GetEventById(eventId);

                bool CanEditTeam = (DateTime.UtcNow.AddHours(_appsetting.CanUpdateHr) <= eventInfo.PerfTimeUTC);
                if (!CanEditTeam)
                {
                    return Json(new {timeout=true,message= "You can update the team up to 1 hour before the event starts" });
                }

                var user = _userManager.GetUserId(HttpContext.User);
                if (user != null)
                {
                    var team = JsonConvert.DeserializeObject<IEnumerable<TeamDto>>(teamData);

                    var teamObj = team.FirstOrDefault(x => x.TeamId > 0);
                    var TeamID = teamObj != null ? teamObj.TeamId : 0;
                    
                    if (TeamID > 0)
                     await _teamService.DeleteTeam(TeamID);
                     var teamId = await _teamService.CreateTeam(team, eventId, user);



                    return Json(new
                    {
                        eventId,
                        teamId
                    });
                }
                else
                {
                    return Json(null);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        /// <summary>
        /// Create long term team 
        /// </summary>
        /// <param name="requestDto">CreateLongTermTeamRequestDto</param>
        /// <returns></returns>
        public async Task<ActionResult> CreateLongTermTeam(CreateLongTermTeamRequestDto requestDto )
        {
            try
            {
                var userId = _userManager.GetUserId(HttpContext.User);
                requestDto.UserId = userId;
                if (userId != null)
                {
                    var team = JsonConvert.DeserializeObject<IEnumerable<LongTermTeamDto>>(requestDto.TeamData);

                    var teamObj = team.FirstOrDefault(x => x.TeamId > 0);
                    var TeamID = teamObj != null ? teamObj.TeamId : 0;
                    if (TeamID > 0)
                        await _longTermTeam.DeleteTeam(TeamID);

                    int newteamId = await _longTermTeam.CreateTeam(team, requestDto);
                    
                    return Json(new
                    {
                        requestDto.TeamId,
                        newteamId
                    });
                }
                else
                {
                    return Json(null);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        /// <summary>
        /// Team List Page
        /// </summary>
        /// <param name="eventId">EventId</param>
        /// <param name="contestId">Contest Id</param>
        /// <returns></returns>
        //[Route("team/{eventId}/{contestId}")]
        //public IActionResult Index(int eventId, int contestId, int result = 0)
        //{
        //     ViewBag.Result = result;
        //     ViewBag.EventId = eventId;
        //     ViewBag.ContestId = contestId;

        //     return View();
        //}

        //public async Task<JsonResult> TeamList(int eventId, int contestId)
        //{
        //     ViewBag.ContestId = contestId;
        //     var draw = Request.Form["draw"];
        //     var start = Request.Form["start"];
        //     var length = Request.Form["length"];
        //     var user = _userManager.GetUserName(HttpContext.User);

        //     //Global search field
        //     var search = Request.Form["search[value]"];

        //     int pageSize = !(string.IsNullOrEmpty(length)) ? Convert.ToInt32(length) : 0;
        //     int skip = !(string.IsNullOrEmpty(start)) ? Convert.ToInt32(start) : 0;
        //     var sort = Request.Form["order[0][dir]"];
        //     var column = Convert.ToInt32(Request.Form["order[0][column]"]);

        //     var teamList = await _teamService.TeamList(eventId, contestId, user, skip / pageSize, pageSize, column, search, sort);

        //     var data = teamList.Item1;
        //     int recordsTotal = teamList.Item2;

        //     return Json(new
        //     {
        //          draw,
        //          recordsFiltered = recordsTotal,
        //          recordsTotal,
        //          data
        //     });
        //}

        ///// <summary>
        ///// Join Team Contest
        ///// </summary>
        ///// <param name="contestId">Contest Id</param>
        ///// <param name="teamId">Team Id</param>
        ///// <param name="eventId">Event Id</param>
        ///// <returns></returns>
        //[Route("team/joincontest/{teamId}/{contestId}/{eventId}")]
        //public async Task<IActionResult> JoinTeamContest(int contestId, int teamId, int eventId, int trasactionId)
        //{
        //     var joinContest = new JoinedContestDto();
        //     joinContest.ContestId = contestId;
        //     joinContest.TeamId = teamId;
        //     joinContest.UserId = _userManager.GetUserId(HttpContext.User);

        //     await _teamService.AddJoinTeamContest(joinContest);
        //     ViewBag.Result = 1;
        //     return RedirectToAction("index", new { eventId, contestId, result = ViewBag.Result });
        //}
    }
}