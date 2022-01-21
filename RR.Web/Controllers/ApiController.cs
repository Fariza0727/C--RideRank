using Microsoft.AspNetCore.Mvc;
using RR.Dto;
using RR.Web.Helpers;
using System.Threading.Tasks;

namespace RR.Web.Controllers
{
    public class ApiController : Controller
    {
        public readonly SessionHelperService _sessionHelperService;
        public ApiController(SessionHelperService sessionHelperService)
        {
            _sessionHelperService = sessionHelperService;
        }

        [HttpPost]
        public async Task<JsonResult> PutUserDetail(BecomeAPlayerDto userDetail)
        {
            _sessionHelperService.UserDetail = userDetail;
            return Json(await Task.FromResult("Stored"));
        }

    }
}