using Microsoft.AspNetCore.Mvc;
using RR.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RR.Web.Controllers.Components
{

    [ViewComponent(Name = "MyRequestsComponents")]
    public class MyRequestsComponents : ViewComponent
    {
        private readonly IUserRequestsServices _requestsServices;

        public MyRequestsComponents(IUserRequestsServices requestsServices )
        {
            _requestsServices = requestsServices;
        }

        /// <summary>
        /// Get user Contest award history
        /// </summary>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var AspNetUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var requests_ = await _requestsServices.GetRequests(AspNetUserId);
            return View(requests_);
        }
    }
}
