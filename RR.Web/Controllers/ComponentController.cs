using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RR.Dto;
using RR.Service;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RR.Web.Controllers
{
    public class ComponentController : Controller
    {
        #region Constructor

        private readonly ITeamService _teamService;
        private readonly ILongTermTeamService _longtermteamService;
        private readonly UserManager<IdentityUser> _userManager;

        public ComponentController(ITeamService teamService, ILongTermTeamService longtermteamService,
            UserManager<IdentityUser> userManager)
        {
            _teamService = teamService;
            _longtermteamService = longtermteamService;
            _userManager = userManager;
        }

        #endregion
        public async Task<JsonResult> YearStanding()
        {
            var draw = Request.Form["draw"];
            var start = Request.Form["start"];
            var length = Request.Form["length"];
            //Global search field

            var search = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = !(string.IsNullOrEmpty(length)) ? Convert.ToInt32(length) : 0;
            int skip = !(string.IsNullOrEmpty(start)) ? Convert.ToInt32(start) : 0;
            //var sort = Request.Form["order[0][dir]"];
            var sort = "asc";
            //var column = Convert.ToInt32(Request.Form["order[0][column]"]);
            var column = 0;
            var yearStandings = await _teamService.GetStandings(false, skip / pageSize, pageSize, column, search, sort);
            var data = yearStandings.Skip(Convert.ToInt32(start)).Take(Convert.ToInt32(length));
            int recordsTotal = yearStandings.Count;
            var temp = new Tuple<StandingDto, StandingDto, StandingDto>[3];
            temp[0] = new Tuple<StandingDto, StandingDto, StandingDto>(data.Count() > 0 ? data.ElementAt(0) : null, data.Count() > 1 ? data.ElementAt(1) : null, data.Count() > 2 ? data.ElementAt(2) : null);
            temp[1] = new Tuple<StandingDto, StandingDto, StandingDto>(data.Count() > 3 ? data.ElementAt(3) : null, data.Count() > 4 ? data.ElementAt(4) : null, data.Count() > 5 ? data.ElementAt(5) : null);
            temp[2] = new Tuple<StandingDto, StandingDto, StandingDto>(data.Count() > 6 ? data.ElementAt(6) : null, data.Count() > 7 ? data.ElementAt(7) : null, data.Count() > 8 ? data.ElementAt(8) : null);

            return Json(new
            {
                draw,
                recordsFiltered = recordsTotal,
                recordsTotal,
                data = temp
            });
        }

        public async Task<JsonResult> YearStandingCurrentUser()
        {
            var draw = Request.Form["draw"];
            var start = Request.Form["start"];
            var length = Request.Form["length"];
            //Global search field

            var search = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = !(string.IsNullOrEmpty(length)) ? Convert.ToInt32(length) : 0;
            int skip = !(string.IsNullOrEmpty(start)) ? Convert.ToInt32(start) : 0;
            var sort = Request.Form["order[0][dir]"];
            var column = Convert.ToInt32(Request.Form["order[0][column]"]);
            var userId =  _userManager.GetUserId(HttpContext.User);
            var yearStandings = await _teamService.GetYearStandingsOfCurrentUser(userId, skip / pageSize, pageSize, column, search, sort);
            var data = yearStandings.Skip(Convert.ToInt32(start)).Take(Convert.ToInt32(length));
            int recordsTotal = yearStandings.Count;

            return Json(new
            {
                draw,
                recordsFiltered = recordsTotal,
                recordsTotal,
                data
            });
        }

        public async Task<JsonResult> TeamStanding()
        {
            var draw = Request.Form["draw"];
            var start = Request.Form["start"];
            var length = Request.Form["length"];
            //Global search field

            var search = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = !(string.IsNullOrEmpty(length)) ? Convert.ToInt32(length) : 0;
            int skip = !(string.IsNullOrEmpty(start)) ? Convert.ToInt32(start) : 0;
            var sort = Request.Form["order[0][dir]"];
            var column = Convert.ToInt32(Request.Form["order[0][column]"]);
            var yearStandings = await _teamService.GetStandings(true, skip / pageSize, pageSize, column, search, sort);
            var data = yearStandings.Skip(Convert.ToInt32(start)).Take(Convert.ToInt32(length));
            int recordsTotal = yearStandings.Count;

            return Json(new
            {
                draw,
                recordsFiltered = recordsTotal,
                recordsTotal,
                data
            });
        }

        [HttpPost]
        public IActionResult GetUserChats(string aspuserid, bool isSeend = false)
        {
            return ViewComponent("UserChatsMessageComponent", new ChatInvokeRequestDto ( aspuserid, isSeend ));
        }
        [HttpPost]
        public IActionResult GetGroupChats(string aspuserid, long contestId)
        {
            return ViewComponent("UserChatsMessageComponent", new ChatInvokeRequestDto(aspuserid,contestId));
        }

        [HttpPost]
        [Route("GetLongTermTeam/{teamId}/{isBulls?}")]
        public async Task<JsonResult> GetLongTermTeam(int teamId, bool isBulls = false)
        {
            var draw = Convert.ToInt16(Request.Form["draw"]);
            var start = Request.Form["start"];
            var length = Request.Form["length"];
            //Global search field

            var search = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = !(string.IsNullOrEmpty(length)) ? Convert.ToInt32(length) : 0;
            int skip = (!(string.IsNullOrEmpty(start)) ? Convert.ToInt32(start) : 1) / pageSize;
            var sort = Request.Form["order[0][dir]"];
            var column = Convert.ToInt32(Request.Form["order[0][column]"]);

            var data = await _longtermteamService.LongTermTeamById(teamId, skip, pageSize, isBulls, search, column, sort);

            int recordsTotal = data.totalRiders;

            if (isBulls)
            {
                recordsTotal = data.totalBulls;
            }

            return Json(new
            {
                draw,
                recordsFiltered = recordsTotal,
                recordsTotal,
                data = data
            });
        }
    }
}