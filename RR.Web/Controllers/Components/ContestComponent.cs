using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RR.Service;
using System;
using System.Threading.Tasks;

namespace RR.Web.Controllers
{
     [ViewComponent(Name = "ContestComponent")]
     public class ContestComponent : ViewComponent
     {
          private readonly UserManager<IdentityUser> _userManager;
          private readonly IContestService _contestService;

          public ContestComponent(
             UserManager<IdentityUser> userManager,
             IContestService contestService)
          {
               _userManager = userManager;
               _contestService = contestService;
          }

          /// <summary>
          /// Get user Contest history
          /// </summary>
          /// <returns></returns>
          public async Task<IViewComponentResult> InvokeAsync()
          { 
               var contestDetail = await _contestService.GetContestOfCurrentUser(_userManager.GetUserId(HttpContext.User));
               return View(contestDetail);
          }
     }
}
