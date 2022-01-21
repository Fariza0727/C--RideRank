using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RR.Core;
using RR.Dto;
using RR.Service;
using RR.Service.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RR.Web.Controllers
{
     public class RidersController : Controller
     {
        #region Constructor
        
          private readonly IRiderService _riderService;
          private readonly IFavoriteBullsRidersService _repofavoriteBulls;
        private readonly AppSettings _appSettings;
        private readonly IBullRiderPicturesService _picturesService;

        public RidersController(IRiderService riderService,
            IBullRiderPicturesService picturesService, IFavoriteBullsRidersService repofavoriteBulls, IOptions<AppSettings> appSettings)
          {
               _riderService = riderService;
                _repofavoriteBulls = repofavoriteBulls;
            _appSettings = appSettings.Value;
            _picturesService = picturesService;
        }

        #endregion


        /// <summary>
        /// Riders Index
        /// </summary>
        /// <returns>Riders View</returns>
        [Route("riders")]
          public IActionResult Index()
          {
               return View();
          }

          /// <summary>
          /// Get Rider Detail
          /// </summary>
          /// <param name="id">An Id</param>
          /// <returns>Rider Detail View</returns>
          [Route("rider/detail/{id}")]
          public async Task<IActionResult> GetRiderDetail(int id)
          {
               ViewBag.picpath = _appSettings.MainSiteURL;
               return View(await _riderService.GetRiderById(id));
          }

        /// <summary>
        /// Get Rider Detail
        /// </summary>
        /// <param name="id">An Id</param>
        /// <returns>Rider Detail View</returns>
        [Route("rider/getpic/{id}")]
        public async Task<string> GetGetPic(int id)
        {
            return await Task.FromResult(await _picturesService.GetRiderPic(id, _appSettings.MainSiteURL));

        }
        [Route("bull/getpic/{id}")]
        public async Task<string> GetBullPic(int id)
        {
            return await Task.FromResult(await _picturesService.GetBullPic(id, _appSettings.MainSiteURL));

        }

        /// <summary>
        /// Get All Riders
        /// </summary>
        /// <returns>List Of 10 Riders Along With Total Count On Riders Index View</returns>
        [HttpPost]
          public async Task<JsonResult> GetAllRiders()
          {
               var draw = Request.Form["draw"];
               var start = Request.Form["start"];
               var length = Request.Form["length"];
                

               //Global search field
               var search = Request.Form["search[value]"];
               var sort = Request.Form["order[0][dir]"];
               var sortby = Request.Form["order[0][column]"];

               int pageSize = !(string.IsNullOrEmpty(length)) ? Convert.ToInt32(length) : 1;
               int skip = !(string.IsNullOrEmpty(start)) ? Convert.ToInt32(start) : 1;
            //var column = Convert.ToInt32(Request.Form["order[0][column]"]);

                var column = 3;
               var riders = await _riderService.GetAllRidersShort(skip / pageSize, pageSize, column, search, "asc");

               var data = riders.Item1;
               int recordsTotal = riders.Item2;
            
               var temp = new Tuple<RiderDto, RiderDto, RiderDto>[3];
            temp[0] = new Tuple<RiderDto, RiderDto, RiderDto>(data.Count() > 0 ? data.ElementAt(0) : null, data.Count() > 1 ? data.ElementAt(1) : null, data.Count() > 2 ? data.ElementAt(2) : null);
            temp[1] = new Tuple<RiderDto, RiderDto, RiderDto>(data.Count() > 3 ? data.ElementAt(3) : null, data.Count() > 4 ? data.ElementAt(4) : null, data.Count() > 5 ? data.ElementAt(5) : null);
            temp[2] = new Tuple<RiderDto, RiderDto, RiderDto>(data.Count() > 6 ? data.ElementAt(6) : null, data.Count() > 7 ? data.ElementAt(7) : null, data.Count() > 8 ? data.ElementAt(8) : null);





            return Json(new
               {
                    draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal,
                data = temp
            });
          }

        /// <summary>
        /// Get All Riders
        /// </summary>
        /// <returns>List Of 10 Riders Along With Total Count On Riders Index View</returns>
        [HttpPost]
        public async Task<JsonResult> GetAllRidersEvens(int id)
        {
            var draw = Request.Form["draw"];
            var start = Request.Form["start"];
            var length = Request.Form["length"];


            //Global search field
            var search = Request.Form["search[value]"];
            var sort = Request.Form["order[0][dir]"];
            var sortby = Request.Form["order[0][column]"];

            int pageSize = !(string.IsNullOrEmpty(length)) ? Convert.ToInt32(length) : 1;
            int skip = !(string.IsNullOrEmpty(start)) ? Convert.ToInt32(start) : 1;
            var column = Convert.ToInt32(Request.Form["order[0][column]"]);

            var riders = await _riderService.GetAllRiderEvents(id,skip / pageSize, pageSize, column, search, sort);
            var data = riders.Item1;
            int recordsTotal = riders.Item2;

            return Json(new
            {
                draw,
                recordsFiltered = recordsTotal,
                recordsTotal,
                data,
                riders.Item3
            });
        }


        /// <summary>
        /// Add Rider as Favorite 
        /// </summary>
        /// <param name="riderId">rider Id</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> AddFavorite(int riderId )
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new
                {
                    status = false,
                    message = "Please login or create an account to add riders as favorite."
                });
            }
            else
            {
                var model = new FavoriteBullRidersDto
                {
                    UserId = userId,
                    RiderId = riderId
                };
                try
                {
                    await _repofavoriteBulls.AddRiderAsFavorite(model);
                    return Json(new
                    {
                        status = true,
                        message = "Rider successfully added as favorite"
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