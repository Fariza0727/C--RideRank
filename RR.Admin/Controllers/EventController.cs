using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RR.AdminService;
using RR.Dto;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RR.Admin.Controllers
{
    [Authorize(Roles = "Admin,Subadmin")]    
    public class EventController : Controller
    {
        #region Constructor

        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        #endregion

        /// <summary>
        /// Event Index Page
        /// </summary>
        /// <returns></returns>        
        [Route("events")]
        [Authorize(Policy = "PagePermission")]
        public ActionResult Index(string mode ="")
        {
            if (!string.IsNullOrEmpty(mode))
            {
                ViewBag.Message = mode;
            }
            return View();
        }

        /// <summary>
        /// Edit Event Detail
        /// </summary>
        /// <param name="id">Event Id</param>
        /// <returns></returns>        
        [Route("event/detail/{eventId}")]
        [Authorize(Policy = "PagePermission")]
        public async Task<ActionResult> EditEventDetail(int eventId)
        {
            try
            {
                return View(await _eventService.GetEventById(eventId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update Event Detail
        /// </summary>
        /// <param name="eventDto">An EventDto</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]        
        public async Task<ActionResult> UpdateEventDetail(EventDto eventDto)
        {
            try
            {
                string msg = "Updated";
                await _eventService.UpdateEventDetail(eventDto);
                
                return RedirectToAction("index",new {mode = msg });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        
        [HttpPost]
        public async Task<JsonResult> GetAllEvents()
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

               var events = await _eventService.GetAllEvents(skip / pageSize, pageSize, column, search, sort);

            var data = events.Item1;
            int recordsTotal = events.Item2;

            return Json(new
            {
                draw,
                recordsFiltered = recordsTotal,
                recordsTotal,
                data
            });
        }

        /// <summary>
        /// Update Status
        /// </summary>
        /// <param name="id">An Event Id</param>
        /// <returns>event status detail</returns>
        [HttpPost]
        [Route("eventstatus")]        
        [Authorize(Policy = "PagePermission")]
        public async Task<JsonResult> UpdateStatus(int eventId)
        {
            try
            {
                await _eventService.UpdateStatus(eventId);
                var eventDetail = await _eventService.GetEventById(eventId);
                return Json(eventDetail);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        /// <summary>
        /// Get Event Detail
        /// </summary>
        /// <param name="id">An Event Id</param>
        /// <returns>Event Detail</returns>
        
        [HttpPost]
        [Authorize(Policy = "PagePermission")]
        public async Task<JsonResult> GetEventDetail(int eventId)
        {
            try
            {
                return Json(await _eventService.GetEventById(eventId));
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

        }

        /// <summary>
        /// Delete Event
        /// </summary>
        /// <param name="eventId">An EventId</param>
        /// <returns></returns>
        [Authorize(Policy = "PagePermission")]
        public async Task<JsonResult> DeleteEvent(int eventId)
        {
            try
            {
                await _eventService.DeleteEventById(eventId);
                return Json("Event Deleted");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

        }
    }
}