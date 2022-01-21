using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RR.Core;

namespace RR.WebApi.Controllers
{
     public class BaseController : Controller
     {
          public static AppSettings _appSettings;

          public BaseController(IOptions<AppSettings> appSettings)
          {
               _appSettings = appSettings.Value;
          }
     }
}