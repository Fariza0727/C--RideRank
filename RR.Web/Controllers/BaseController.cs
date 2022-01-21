using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using RR.Core;
using RR.Dto;

namespace RR.Web.Controllers
{
     public class BaseController : Controller
     {
          public static AppSettings _appSettings;

          public BaseController(IOptions<AppSettings> appSettings)
          {
               _appSettings = appSettings.Value;
          }

          //public override void OnActionExecuted(ActionExecutedContext filterContext)
          //{   
          //     var log = new LoggerDto
          //     {
          //          UserId = _userManager.GetUserId(HttpContext.User),
          //          Url = filterContext.HttpContext.Request.Path.ToString(),
          //          Message = "Succeeded"
          //     };
          //}
     }
}