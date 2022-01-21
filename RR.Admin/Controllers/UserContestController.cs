using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RR.AdminService;
using RR.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.Admin.Controllers
{
    [Authorize(Roles = "Admin,Subadmin")]    
    public class UserContestController : Controller
    {
        #region Constructor
        private readonly IEventService _eventService;
        private readonly IUserContestService _userContestService;
        private readonly IContestService _contestService;

        public UserContestController(IEventService eventService, IUserContestService userContestService, IContestService contestService)
        {
            _eventService = eventService;
            _userContestService = userContestService;
            _contestService = contestService;
        }
        #endregion
        [Authorize(Policy = "PagePermission")]
        [Route("usercontests/{contestId=0}")]
        public async Task<IActionResult> Index(long contestId = 0)
        {
            EventSearchDto model = new EventSearchDto();
            var events = await _eventService.GetAllEvents();
            List<SelectListItem> eventList = new List<SelectListItem>();
            if (events != null && events.Count() > 0)
            {
                model.EventList = events.Where(x => !string.IsNullOrEmpty(x.Title)).Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = x.Title, Value = x.Id.ToString() }).ToList();
            }
           

            return View(model);
        }

        [Route("contestdropdown")]
        public async Task<JsonResult> GetContests(int eventId = 0)
        {
            try
            {
                return Json(await _contestService.GetContestDropdownByEventId(eventId));
            }
            catch (System.Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpPost]
        [Route("contests")]
        public async Task<JsonResult> GetContest(long contestId = 0)
        {
            var contests = await _contestService.GetContest(contestId);
            return Json(new
            {
                data = contests
            });
        }

        [HttpPost]
        [Route("usercontests/allusercontests/{contestId}")]
        public async Task<JsonResult> GetUserContests(long contestId = 0)
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
           var contests = await _userContestService.GetAlluserbyContest(skip / pageSize, pageSize, column, search, sort, contestId);
            var data = contests.Item1;
            int recordsTotal = contests.Item2;

            return Json(new
            {
                draw,
                recordsFiltered = recordsTotal,
                recordsTotal,
                data
            });
        }

        [HttpGet]
        [Route("usercontestwinners/{contestId}")]
        [Authorize(Policy = "PagePermission")]
        public IActionResult GetUserContestWinners(long contestId = 0)
        {
          
            return View();
        }

        [HttpPost]
        [Route("usercontestwinnersdata/{contestId}")]
        [Authorize(Policy = "PagePermission")]
        public async Task<JsonResult> GetUserContestwinnerdata(long contestId = 0)
        {
            var contests = await _userContestService.GetContestWinnerUsers(contestId);

            return Json(new
            {
                data = contests
            });
        }

    }
}