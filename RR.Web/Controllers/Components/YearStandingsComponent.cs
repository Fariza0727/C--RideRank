using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RR.Service;
using RR.Service.User;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RR.Web.Controllers
{
    [ViewComponent(Name = "YearStandingsComponent")]
    public class YearStandingsComponent : ViewComponent
    {
        #region Constructor

        private readonly ITeamService _teamService;
        private readonly IUserService _userService;
        private readonly UserManager<IdentityUser> _userManager;

        public YearStandingsComponent(ITeamService teamService, IUserService userService,
            UserManager<IdentityUser> userManager)
        {
            _teamService = teamService;
            _userManager = userManager;
            _userService = userService;
        }

        #endregion

        /// <summary>
        /// This Method is used for getting all 
        /// relevent news on home page
        /// </summary>
        /// <returns>List of all news on home page</returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var yearStandings = await _teamService.GetStandings(false, 0, 5000, 0, "", "");
            var record = yearStandings.Where(x => x.UserId == userId).ToList();
            var userDetail = await _userService.GetUserDetail(userId);
            ViewBag.TeamName = userDetail.TeamName;

            return View(record);
        }
    }
}
