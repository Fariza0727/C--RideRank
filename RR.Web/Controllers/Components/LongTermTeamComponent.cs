using Microsoft.AspNetCore.Mvc;
using RR.Dto;
using RR.Dto.Team;
using RR.Service;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RR.Web.Controllers.Components
{

    [ViewComponent(Name = "LongTermTeamComponent")]
    public class LongTermTeamComponent : ViewComponent
    {
        private readonly ILongTermTeamService _TeamService;

        public LongTermTeamComponent(ILongTermTeamService teamService) {
            _TeamService = teamService;
        }

        /// <summary>
        /// Get Long Term Team
        /// </summary>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync(ComponentRequestDto dto )
        {
            LongTermTeamFormationDto model = new LongTermTeamFormationDto();
            string userID = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            model = await _TeamService.LongTermTeamById(dto.TeamId, dto.Page, dto.PageSize, userId: userID);
            ViewBag.userId = userID;
            return View(model);
        }
    }
}
