using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RR.Dto;
using RR.Service;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace RR.Web.Controllers
{
    public class EventsController : Controller
    {
        #region Constructor

        private readonly IEventService _eventService;
        private readonly ITeamService _teamService;

        public EventsController(IEventService eventService,
             ITeamService teamService)
        {
            _eventService = eventService;
            _teamService = teamService;
        }

        #endregion


        /// <summary>
        /// Get All Events Index Page
        /// </summary>
        /// <returns></returns>
        [Route("events")]
        public async Task<IActionResult> Index()
        {
            /*var eventResults = await _eventService.GetAllEvents();
            ViewBag.PlayerStandings = await _teamService.GetStandings(true);*/
            return View();
        }
        /// <summary>
        /// This method is used for getting all completed events with paging
        /// </summary>
        /// <returns>Json</returns>
        [HttpPost]
        public async Task<JsonResult> GetCompletedEvents()
        {
            var draw = Request.Form["draw"];
            var start = Request.Form["start"];
            var length = Request.Form["length"];


            int pageSize = !(string.IsNullOrEmpty(length)) ? Convert.ToInt32(length) : 0;
            int skip = !(string.IsNullOrEmpty(start)) ? Convert.ToInt32(start) : 0;
            
            var events = await _eventService.GetAllCompletedEvents(skip / pageSize, pageSize);

            var data = events.Item1.Skip(Convert.ToInt32(start)).Take(Convert.ToInt32(length)); ;
            int recordsTotal = events.Item2;

            return Json(new
            {
                draw,
                recordsFiltered = recordsTotal,
                recordsTotal,
                data
            });
        }

        [HttpPost]
        public async Task<JsonResult> GetUpcomingEvents()
        {
            var draw = Request.Form["draw"];
            var start = Request.Form["start"];
            var length = Request.Form["length"];

            var events = await _eventService.GetUpcomingEvents();
            var data = events.Skip(Convert.ToInt32(start)).Take(Convert.ToInt32(length));

            int recordsTotal = events.Count();

            return Json(new
            {
                draw,
                recordsFiltered = recordsTotal,
                recordsTotal,
                data
            });
        }

        /// <summary>
        /// This method is used for getting all riders associated 
        /// with bulls for specific event
        /// </summary>
        /// <param name="id">An Id of event</param>
        /// <returns>Event Draw View</returns>
        public async Task<IActionResult> EventDraw(int id)
        {
            var eventData = await _eventService.GetEventById(id);
            string eID = eventData != null ? eventData.EventId : "";
            var drawResponse = await Core.WebProxy.APIResponse($"event_draw&id={eID}");

            ViewBag.EventTitle = eventData.Title;
            ViewBag.StatDate = eventData.PerfTime;
            ViewBag.Location = eventData.Location;

            if (drawResponse.ToUpper() != "FALSE")
            {
                //EventDrawApiDto responseDraw = new EventDrawApiDto();
                var draws = JsonConvert.DeserializeObject<IEnumerable<Object>>(drawResponse);

                var responseDraw = draws.Select(x =>
                    {
                        try { return JsonConvert.DeserializeObject<EventDrawDto>(JsonConvert.SerializeObject(x)); } catch { return null; }
                    }
                ).Where(x => x != null);
                
                //var responseDraw = JsonConvert.DeserializeObject<IEnumerable<EventDrawDto>>(drawResponse);
                if (responseDraw.Count() == 0)
                {
                    responseDraw = await _eventService.GetEventDrawById(id);
                }

                return View(responseDraw);
            }
            
            return View(new List<EventDrawDto>());
        }

    }
}