using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RR.Admin.Models;
using RR.AdminService;
using RR.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.Admin.Controllers.Components
{
   
    [ViewComponent(Name = "UserCardComponent")]
    public class UserCardComponent : ViewComponent
    {
        private readonly IUserService userService;

        public UserCardComponent(IUserService userService)
        {
            this.userService = userService;
        }
        /// <summary>
        /// Get user 
        /// </summary>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await userService.GetUsersAsCard());
        }
    }


    [ViewComponent(Name = "EventCardComponent")]
    public class EventCardComponent : ViewComponent
    {
        private readonly IEventService eventService;

        public EventCardComponent(IEventService eventService)
        {
            this.eventService = eventService;
        }
        /// <summary>
        /// Get events
        /// </summary>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await eventService.GetEventsAsCard());
        }
    }

    [ViewComponent(Name = "RidersCardComponent")]
    public class RidersCardComponent : ViewComponent
    {
        private readonly IRiderService riderService;

        public RidersCardComponent(IRiderService riderService)
        {
            this.riderService = riderService;
        }
        /// <summary>
        /// Get riders
        /// </summary>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await riderService.GetRidersAsCard());
        }
    }

    [ViewComponent(Name = "BullsCardComponent")]
    public class BullsCardComponent : ViewComponent
    {
        private readonly IBullService bullService;

        public BullsCardComponent(IBullService bullService)
        {
            this.bullService = bullService;
        }
        /// <summary>
        /// Get bulls
        /// </summary>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await bullService.GetBullsAsCard());
        }
    }

    [ViewComponent(Name = "DailyUserCardComponent")]
    public class DailyUserCardComponent : ViewComponent
    {
        private readonly IUserService userService;

        public DailyUserCardComponent(IUserService userService)
        {
            this.userService = userService;
        }
        /// <summary>
        /// Get user 
        /// </summary>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await userService.GetWeeklyUsersAsCard());
        }
    }

    [ViewComponent(Name = "LogComponent")]
    public class LogComponent : ViewComponent
    {
        private readonly AppSettings _settings;
        private readonly IHostingEnvironment _hostingEnvironment;

        public LogComponent(IOptions<AppSettings> settings, IHostingEnvironment hostingEnvironment)
        {
            _settings = settings.Value;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IViewComponentResult> InvokeAsync(DateTime date)
        {
            //var files = LogManagers.GetLogs(string.Concat(_hostingEnvironment.ContentRootPath, "/", _settings.Logfilepath.Replace("{date}", date.ToString("yyyyMMdd")).TrimStart('/')));
            return View(new Tuple<DateTime, List<LogResponse>>(date, new List<LogResponse>()));
        }
    }
}
