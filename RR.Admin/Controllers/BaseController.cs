using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RR.Admin.Models;
using RR.Core;

namespace RR.Admin.Controllers
{
    [ServiceFilter(typeof(AdminExceptionFilter))]
    public class BaseController : Controller
     {
          public static AppSettings _appSettings;

          public BaseController(IOptions<AppSettings> appSettings)
          {
               _appSettings = appSettings.Value;
          }
     }
}