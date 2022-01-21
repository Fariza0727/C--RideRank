using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RR.AdminService;
using RR.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RR.Admin.Controllers
{
    [Authorize(Roles = "Admin,Subadmin")]    
    public class ContestManagementController : Controller
    {
        #region Constructor

        private readonly IContestService _contestService;
        private readonly IAwardTypeService _awardTypeService;
        private readonly IEventService _eventService;
        private readonly IContestCategoryService _contestCategoryService;

        public ContestManagementController(IContestService contestService,
            IAwardTypeService awardTypeService,
            IEventService eventService,
            IContestCategoryService contestCategoryService)
        {
            _contestService = contestService;
            _awardTypeService = awardTypeService;
            _eventService = eventService;
            _contestCategoryService = contestCategoryService;
        }

        #endregion

        [Route("contests")]
        [Authorize(Policy = "PagePermission")]        
        public async Task<IActionResult> Index()
        {
            EventSearchDto model = new EventSearchDto();
            var events = await _eventService.GetAllEvents();
            List<SelectListItem> eventList = new List<SelectListItem>();
            if (events != null && events.Count() > 0)
            {
                model.EventList = events.Where(x => !string.IsNullOrEmpty(x.Title)).Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = x.Title, Value = x.Id.ToString() }).ToList();
            }
            model.EventList.Insert(0, new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "Select event", Value = "0" });

            return View(model);
        }

        [Route("addeditcontest/{id}")]
        [Authorize(Policy = "PagePermission")]        
        public async Task<IActionResult> AddEditContest(int id)
        {
            ContestDto model = new ContestDto();
            model.Id = id;

            if (id > 0)
            {
                model = await _contestService.GetContest(id);
            }

            model.IsActive = true;
            var contestCategory = await _contestCategoryService.GetContestCategories();
            model.ContestCategories = contestCategory.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            //var events = await _eventService.GetUpcomingEvents();
            var events = await _eventService.GetUpcomingEventsFiltered(id);
            model.Events = events.Select(x => new SelectListItem() { Text = x.Title, Value = x.Id.ToString(), Selected = (x.Id == model.EventId) }).ToList();
            model.Events.Insert(0, new SelectListItem() { Text = "Select event", Value = "" });

            //    model.EntryFeeTypes = new List<SelectListItem> {
            //                 new SelectListItem{ Text="Cash", Value = "1",Selected=(model.EntryFeeType==1)},
            //               new SelectListItem{ Text="Token", Value = "2",Selected=(model.EntryFeeType==2)}
            //  };
            return PartialView(model);
        }

        
        [HttpPost]
        [Route("addeditcontest/{id}")]
        [Authorize(Policy = "PagePermission")]
        public async Task<IActionResult> AddEditContest(ContestDto model)
        {
            long Id = model.Id;
            var resultId = await _contestService.AddEditContest(model, HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (Id == 0)
                return Content("<div class='alert alert-success alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Success!!!</strong> contest has been added successfully. <input type='hidden' class='newId' value='" + resultId + "' /></div>");
            else
                return Content("<div class='alert alert-success alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Success!!!</strong> contest has been edited successfully.</div>");
        }

        
        [HttpPost]
        public JsonResult IsTitleAvailable(string Title, string OldTitle)
        {
            if (Title != OldTitle)
            {
                bool ifs = _contestService.IsTitleAvailable(Title).Result;
                return Json(!ifs);
            }

            return Json(true);
        }

        [HttpPost]
        public JsonResult IsEventContestAdded(int EventId, int Id)
        {
            if (EventId > 0)
            {
                bool ifs = _contestService.IsContestAvailable(EventId, Id).Result;
                return Json(!ifs);
            }

            return Json(true);
        }

        [HttpPost]
        [Route("contests/allcontests/{eventId}")]
        public async Task<JsonResult> GetContests(long eventId = 0)
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

            var contests = await _contestService.GetAllContest(skip / pageSize, pageSize, column, search, sort, eventId);

            var data = contests.Item1;
            int recordsTotal = contests.Item2;

            return Json(new
            {
                draw = draw,
                recordsFiltered = recordsTotal,
                recordsTotal = recordsTotal,
                data = data
            });
        }

        
        [Route("deletecontest/{id}")]
        [Authorize(Policy = "PagePermission")]
        public async Task<IActionResult> DeleteContest(int id)
        {
            await _contestService.DeleteContest(id);
            return Content("<div class='alert alert-success alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Success!!!</strong> award type has been deleted successfully.</div>");
        }
    }
}