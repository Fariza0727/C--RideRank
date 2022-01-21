using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RR.AdminService;
using RR.Dto;

namespace RR.Admin.Controllers
{
    [Authorize(Roles = "Admin,Subadmin")]
    public class UserRequestsController : Controller
    {
        private readonly IUserRequestsServices _userRequests;

        public UserRequestsController(IUserRequestsServices userRequests)
        {
            _userRequests = userRequests;
        }

         
        [Route("userrequests")]
        [Authorize(Policy = "PagePermission")]
        public ActionResult Index(string mode = "")
        {
            if (!string.IsNullOrEmpty(mode))
            {
                ViewBag.Message = mode;
            }
            return View();
        }

        
        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> GetAllRequests()
        {
            var draw = Request.Form["draw"];
            var start = Request.Form["start"];
            var length = Request.Form["length"];


            //Global search field
            var search = Request.Form["search[value]"].FirstOrDefault();

            int pageSize = !(string.IsNullOrEmpty(length)) ? Convert.ToInt32(length) : 0;
            int skip = (!(string.IsNullOrEmpty(start)) ? Convert.ToInt32(start) : 1) / pageSize;
            var sort = Request.Form["order[0][dir]"].FirstOrDefault();
            var column = Convert.ToInt32(Request.Form["order[0][column]"]);

            var requests = await _userRequests.GetAllRequests(skip, pageSize, column, search, sort);

            var data = requests.Item1;
            int recordsTotal = requests.Item2;

            return Json(new
            {
                draw,
                recordsFiltered = recordsTotal,
                recordsTotal,
                data
            });
        }

        [Authorize(Policy = "PagePermission")]
        [HttpPost]
        public async Task<JsonResult> UpdateStatus(long Id)
        {
            var request_ = await _userRequests.UpdateRequest(Id);
            return Json(new { status = true });
        }

    }
}