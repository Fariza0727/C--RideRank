using Microsoft.AspNetCore.Mvc;
using RR.Service;
using System.Threading.Tasks;

namespace RR.Web.Controllers
{
     [ViewComponent(Name = "PlayOffStandingsComponent")]
     public class PlayOffStandingsComponent : ViewComponent
     {
          #region Constructor

          private readonly ITeamService _teamService;

          public PlayOffStandingsComponent(ITeamService teamService)
          {
               _teamService = teamService;
          }

          #endregion

          /// <summary>
          /// This Method is used for getting all 
          /// relevent news on home page
          /// </summary>
          /// <returns>List of all news on home page</returns>
          public async Task<IViewComponentResult> InvokeAsync()
          {
               var playOfStandings = await _teamService.GetPlayerPointsOfEvent();
               return View(playOfStandings);
          }
     }
}
