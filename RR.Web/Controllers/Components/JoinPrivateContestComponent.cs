using Microsoft.AspNetCore.Mvc;
using RR.Dto.Contest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.Web.Controllers.Components
{
     [ViewComponent(Name = "JoinPrivateContestComponent")]
     public class JoinPrivateContestComponent : ViewComponent
     {
          public JoinPrivateContestComponent()
          {
          }

          /// <summary>
          /// Get user Contest history
          /// </summary>
          /// <returns></returns>
          public async Task<IViewComponentResult> InvokeAsync()
          {
               return View(await Task.FromResult(new JoinPrivateContestDto() { }));
          }
     }
}
