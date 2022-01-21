using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RR.Dto;
using RR.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.Web.Controllers
{
     [ViewComponent(Name = "HomePageUpcomingEventComponent")]
     public class HomePageUpcomingEventComponent : ViewComponent
     {
          #region Constructor

          private IConfiguration configuration;
          private readonly IEventService _eventService;

          public HomePageUpcomingEventComponent(IEventService eventService, IConfiguration config)
          {
            _eventService = eventService;
               configuration = config;
          }

          #endregion

          /// <summary>
          /// This Method is used for getting all 
          /// relevent news on home page
          /// </summary>
          /// <returns>List of all news on home page</returns>
          public async Task<IViewComponentResult> InvokeAsync()
          {
               var upComingEvent = await _eventService.GetUpcomingEvents(3);               
               return View(upComingEvent);
          }
     }
}
