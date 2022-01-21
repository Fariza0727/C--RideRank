using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RR.AdminService;
using System;
using System.Threading.Tasks;

namespace RR.Admin.Controllers
{
    [Authorize(Roles = "Admin,Subadmin")]
  
    public class NewsLetterController : Controller
     {
          private readonly INewsLetterService _newsLetterService;

          public NewsLetterController(INewsLetterService newsLetterService)
          {
               _newsLetterService = newsLetterService;
          }

          public IActionResult Index()
          {
               return View();
          }

          [AllowAnonymous]
          public async Task<JsonResult> GetAllNewsLetterSubscriber()
          {
               var draw = Request.Form["draw"];
               var start = Request.Form["start"];
               var length = Request.Form["length"];

               //Global search field
               var search = Request.Form["search[value]"];
               var sort = Request.Form["order[0][dir]"];
               var column = Convert.ToInt32(Request.Form["order[0][column]"]);

               int pageSize = !(string.IsNullOrEmpty(length)) ? Convert.ToInt32(length) : 0;
               int skip = !(string.IsNullOrEmpty(start)) ? Convert.ToInt32(start) : 0;

               var eventData = await _newsLetterService.GetAllNewsLetterSubscribers(skip / pageSize, pageSize, column, search, sort);

               var data = eventData.Item1;
               int recordsTotal = eventData.Item2;

               return Json(new
               {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = data
               });
          }

          [HttpDelete]
          public ActionResult DeleteNewsLetterSubscriber(int newsletterId)
          {
               try
               {
                    _newsLetterService.DeleteNewsLetter(newsletterId);

                    return Ok("Deleted");
               }
               catch (Exception ex)
               {
                    return BadRequest(ex.Message);
               }
          }
     }
}