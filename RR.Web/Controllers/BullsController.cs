using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RR.Core;
using RR.Dto;
using RR.Service;
using RR.Service.User;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RR.Web.Controllers
{  
     public class BullsController : Controller
     {
          #region Constructor

          private readonly IBullService _bullService;
          private readonly IFavoriteBullsRidersService _favoriteBullsRiders;
        private readonly AppSettings _appSettings;

        public BullsController(
            IBullService bullService, 
            IFavoriteBullsRidersService favoriteBullsRiders,
            IOptions<AppSettings> appSettings)
          {
               _bullService = bullService;
            _favoriteBullsRiders = favoriteBullsRiders;
            _appSettings = appSettings.Value;
        }

          #endregion

          /// <summary>
          /// Bulls
          /// </summary>
          /// <returns>Bulls List View</returns>
          [Route("bulls")]
          public IActionResult Index()
          {
               return View();
          }

          /// <summary>
          /// Get Bull Detail
          /// </summary>
          /// <param name="id">An Id</param>
          /// <returns>Bull Detail View</returns>
          [Route("bull/detail/{id}")]
          public async Task<IActionResult> GetBullDetail(int id)
          {
               ViewBag.picpath = _appSettings.MainSiteURL;
               return View(await _bullService.GetBullById(id));
          }

          /// <summary>
          /// This method is used for getting all bulls with paging
          /// </summary>
          /// <returns>Json</returns>
          [HttpPost]
          public async Task<JsonResult> GetAllBulls()
          {
               var draw = Request.Form["draw"];
               var start = Request.Form["start"];
               var length = Request.Form["length"];


               //Global search field
               var search = Request.Form["search[value]"];

               int pageSize = !(string.IsNullOrEmpty(length)) ? Convert.ToInt32(length) : 0;
               int skip = !(string.IsNullOrEmpty(start)) ? Convert.ToInt32(start) : 0;
               var sort = Request.Form["order[0][dir]"];
               //var column = Convert.ToInt32(Request.Form["order[0][column]"]);
               var column = 2;
               var bulls = await _bullService.GetAllBulls(skip / pageSize, pageSize, column, search, "asc");

               var data = bulls.Item1;
               int recordsTotal = bulls.Item2;
                
               var temp = new Tuple<BullDto, BullDto, BullDto>[3];
                temp[0] = new Tuple<BullDto, BullDto, BullDto>(data.Count() > 0 ? data.ElementAt(0) : null, data.Count() > 1 ? data.ElementAt(1) : null, data.Count() > 2 ? data.ElementAt(2) : null);
            temp[1] = new Tuple<BullDto, BullDto, BullDto>(data.Count() > 3 ? data.ElementAt(3) : null, data.Count() > 4 ? data.ElementAt(4) : null, data.Count() > 5 ? data.ElementAt(5) : null);
            temp[2] = new Tuple<BullDto, BullDto, BullDto>(data.Count() > 6 ? data.ElementAt(6) : null, data.Count() > 7 ? data.ElementAt(7) : null, data.Count() > 8 ? data.ElementAt(8) : null);
               
            return Json(new
               {
                    draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal,
                    data = temp
               });
          }

        /// <summary>
        /// This method is used for getting all bulls with paging
        /// </summary>
        /// <returns>Json</returns>
        [HttpPost]
        public async Task<JsonResult> GetBullsEvents(int id)
        {
            var draw = Request.Form["draw"];
            var start = Request.Form["start"];
            var length = Request.Form["length"];


            //Global search field
            var search = Request.Form["search[value]"];

            int pageSize = !(string.IsNullOrEmpty(length)) ? Convert.ToInt32(length) : 0;
            int skip = !(string.IsNullOrEmpty(start)) ? Convert.ToInt32(start) : 0;
            var sort = Request.Form["order[0][dir]"];
            var column = Convert.ToInt32(Request.Form["order[0][column]"]);

            var bulls = await _bullService.GetBullEvents(id, skip / pageSize, pageSize, column, search, sort);

            var data = bulls.Item1;
            int recordsTotal = bulls.Item2;

            return Json(new
            {
                draw,
                recordsFiltered = recordsTotal,
                recordsTotal,
                data
            });
        }
        /// <summary>
        /// Add Bull as Favorite 
        /// </summary>
        /// <param name="bullId">bull Id</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> AddFavorite(int bullId)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new
                {
                    status = false,
                    message = "Please login or create an account to add bull as favorite."
                });
            }
            else
            {
                var model = new FavoriteBullRidersDto
                {
                    UserId = userId,
                    BullId = bullId
                };
                try
                {
                    await _favoriteBullsRiders.AddBullAsFavorite(model);
                    return Json(new
                    {
                        status = true,
                        message = "Bull successfully added as favorite"
                    });
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        status = false,
                        message = "Oops!! something wrong"
                    });
                }
            }

        }

    }
}