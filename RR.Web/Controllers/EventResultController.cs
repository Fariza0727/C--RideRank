using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace RR.Web.Controllers
{
    public class EventResultController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("standings")]
        public IActionResult standings()
        {
            return View();
        }
    }
}