using Microsoft.AspNetCore.Mvc;
using RR.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.Web.Controllers.Components
{

    [ViewComponent(Name = "LastEventStandingComponent")]
    public class LastEventStandingComponent : ViewComponent
    {
        #region Constructor

        private readonly ITeamService _teamService;

        public LastEventStandingComponent(ITeamService teamService)
        {
            _teamService = teamService;
        }

        #endregion

        /// <summary>
        /// This Method is used for getting all 
        /// relevent news on home page
        /// </summary>
        /// <returns>List of all news on home page</returns>
        public async Task<IViewComponentResult> InvokeAsync(int count)
        {
            var playOfStandings = await _teamService.GetLastEventStatndingPlayerPoints(count);
            return View(playOfStandings);
        }
    }
}
