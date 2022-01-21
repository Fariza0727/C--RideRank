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
    [ViewComponent(Name = "SubscibeNewsLetterComponent")]
    public class SubscibeNewsLetterComponent : ViewComponent
    {
        #region Constructor

        private IConfiguration configuration;
        private readonly IEventService _eventService;

        public SubscibeNewsLetterComponent(IEventService eventService, IConfiguration config)
        {
            _eventService = eventService;
            configuration = config;
        }

        #endregion

        /// <summary>
        /// This Method is used for news letter subscription.
        /// </summary>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            SubscribeDto model = new SubscribeDto();
            return View(await Task.FromResult(model));
        }
    }
}
