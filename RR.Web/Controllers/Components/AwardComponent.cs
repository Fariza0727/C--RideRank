using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RR.Service;
using System.Threading.Tasks;

namespace RR.Web.Controllers.Components
{
    [ViewComponent(Name = "AwardComponent")]
    public class AwardComponent : ViewComponent
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IContestUserWinnerService _contestUWService;

        public AwardComponent(
           UserManager<IdentityUser> userManager,
           IContestUserWinnerService contestUWService)
        {
            _userManager = userManager;
            _contestUWService = contestUWService;
        }

        /// <summary>
        /// Get user Contest award history
        /// </summary>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var contestDetail = await _contestUWService.GetUserWinnings(_userManager.GetUserId(HttpContext.User));
            return View(contestDetail);
        }
    }
}
