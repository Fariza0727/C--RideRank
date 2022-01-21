using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RR.Dto;
using RR.Service;

namespace RR.Web.Controllers
{
     public class ContestStandingController : Controller
     {
          #region Constructor

          private readonly ITeamService _teamService;

          public ContestStandingController(ITeamService teamService)
          {
               _teamService = teamService;
          }

          #endregion         
          
          [Route("conteststandings")]
          public async Task<IActionResult> Index()
          {
               var playContestStandings = await _teamService.GetAllPlayerPointsOfEventContest();
               return View(playContestStandings.Distinct().ToList());
          }

        [HttpGet]
        [Route("lasteventstanding")]
        public async Task<IActionResult> LastEventStanding(bool IsFreePlay = true)
        {

            ViewBag.IsFreePlay = IsFreePlay;
            ViewBag.EventTitle = await _teamService.GetLastEventName();
            return View();
        }
        [HttpPost]
        [Route("ajaxlasteventusers/{IsFreeplay?}")]
        public async Task<JsonResult> LastEventAjax(bool IsFreePlay = true)
        {
            var draw = Request.Form["draw"];
            var start = Request.Form["start"];
            var length = Request.Form["length"];
            
            var playContestStandings = new List<PlayStandingLiteDto>(); 
            
            if (IsFreePlay)
            {
                playContestStandings = await _teamService.GetLastEventStatndingFreePlayerPoints();
            }
            else
            {
                playContestStandings = await _teamService.GetLastEventStatndingPlayerPoints();
            }
            
            var data = playContestStandings.Skip(Convert.ToInt32(start)).Take(Convert.ToInt32(length));
            int recordsTotal = playContestStandings.Count;
            var temp = new Tuple<PlayStandingLiteDto, PlayStandingLiteDto, PlayStandingLiteDto>[5];
            temp[0] = new Tuple<PlayStandingLiteDto, PlayStandingLiteDto, PlayStandingLiteDto>(data.Count() > 0 ? data.ElementAt(0) : null, data.Count() > 1 ? data.ElementAt(1) : null, data.Count() > 2 ? data.ElementAt(2) : null);
            temp[1] = new Tuple<PlayStandingLiteDto, PlayStandingLiteDto, PlayStandingLiteDto>(data.Count() > 3 ? data.ElementAt(3) : null, data.Count() > 4 ? data.ElementAt(4) : null, data.Count() > 5 ? data.ElementAt(5) : null);
            temp[2] = new Tuple<PlayStandingLiteDto, PlayStandingLiteDto, PlayStandingLiteDto>(data.Count() > 6 ? data.ElementAt(6) : null, data.Count() > 7 ? data.ElementAt(7) : null, data.Count() > 8 ? data.ElementAt(8) : null);
            temp[3] = new Tuple<PlayStandingLiteDto, PlayStandingLiteDto, PlayStandingLiteDto>(data.Count() > 9 ? data.ElementAt(9) : null, data.Count() > 10 ? data.ElementAt(10) : null, data.Count() > 11 ? data.ElementAt(11) : null);
            temp[4] = new Tuple<PlayStandingLiteDto, PlayStandingLiteDto, PlayStandingLiteDto>(data.Count() > 12 ? data.ElementAt(12) : null, data.Count() > 13 ? data.ElementAt(13) : null, data.Count() > 14 ? data.ElementAt(14) : null);

            return Json(new
            {
                draw,
                recordsFiltered = recordsTotal,
                recordsTotal,
                data = temp
            });
        }
        [HttpPost]
        [Route("topreferredajax")]
        public async Task<JsonResult> TopReferredAjax(bool IsFreePlay = true)
        {
            var draw = Request.Form["draw"];
            var start = Request.Form["start"];
            var length = Request.Form["length"];

            var topReferred = await _teamService.GetTopReferred();

            var data = topReferred.Skip(Convert.ToInt32(start)).Take(Convert.ToInt32(length));
            int recordsTotal = topReferred.Count;
            var temp = new Tuple<PlayStandingLiteDto, PlayStandingLiteDto, PlayStandingLiteDto>[5];
            temp[0] = new Tuple<PlayStandingLiteDto, PlayStandingLiteDto, PlayStandingLiteDto>(data.Count() > 0 ? data.ElementAt(0) : null, data.Count() > 1 ? data.ElementAt(1) : null, data.Count() > 2 ? data.ElementAt(2) : null);
            temp[1] = new Tuple<PlayStandingLiteDto, PlayStandingLiteDto, PlayStandingLiteDto>(data.Count() > 3 ? data.ElementAt(3) : null, data.Count() > 4 ? data.ElementAt(4) : null, data.Count() > 5 ? data.ElementAt(5) : null);
            temp[2] = new Tuple<PlayStandingLiteDto, PlayStandingLiteDto, PlayStandingLiteDto>(data.Count() > 6 ? data.ElementAt(6) : null, data.Count() > 7 ? data.ElementAt(7) : null, data.Count() > 8 ? data.ElementAt(8) : null);
            temp[3] = new Tuple<PlayStandingLiteDto, PlayStandingLiteDto, PlayStandingLiteDto>(data.Count() > 9 ? data.ElementAt(9) : null, data.Count() > 10 ? data.ElementAt(10) : null, data.Count() > 11 ? data.ElementAt(11) : null);
            temp[4] = new Tuple<PlayStandingLiteDto, PlayStandingLiteDto, PlayStandingLiteDto>(data.Count() > 12 ? data.ElementAt(12) : null, data.Count() > 13 ? data.ElementAt(13) : null, data.Count() > 14 ? data.ElementAt(14) : null);

            return Json(new
            {
                draw,
                recordsFiltered = recordsTotal,
                recordsTotal,
                data = temp
            });
        }

        [HttpGet]
        [Route("yearstandings")]
        public IActionResult YearStandings()
        {
            return View();
        }

        [Route("yearstanding")]
        public async Task<JsonResult> YearEndStanding()
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
            var yearStandings = await _teamService.GetStandings(false, skip / pageSize, pageSize, column, search, sort);
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

        
    }
}