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
     [ViewComponent(Name = "HeaderLatestEvent")]
     public class HeaderLatestEventComponent : ViewComponent
     {
          #region Constructor
                                         
          private readonly IEventService _eventService;

          public HeaderLatestEventComponent(IEventService eventService)
          {
            _eventService = eventService; 
          }

          #endregion

          /// <summary>
          /// This Method is used for getting all 
          /// relevent news on home page
          /// </summary>
          /// <returns>List of all news on home page</returns>
          public async Task<IViewComponentResult> InvokeAsync()
          {
               var upComingEvent = await _eventService.GetUpcomingEvent();               
               return View(upComingEvent);
          }
     }
}
