using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RR.AdminService;
using RR.Dto;
using System;
using System.Threading.Tasks;

namespace RR.Admin.Controllers
{
    [Authorize(Roles = "Admin,Subadmin")]    
    public class RiderController : Controller
    {
        #region Constructor

        private readonly IRiderService _riderService;

        public RiderController(IRiderService riderService)
        {
            _riderService = riderService;
        }

        #endregion

        /// <summary>
        /// Rider Index Page
        /// </summary>
        /// <returns></returns>        
        [Route("riders")]
        [Authorize(Policy = "PagePermission")]
        public ActionResult Index(string mode ="")
        {
            if (!string.IsNullOrEmpty(mode))
            {
                ViewBag.Message = mode;
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("rider/getallriders/{rank}/{getCleanRider?}")]
        public async Task<JsonResult> GetAllRiders(int rank = 1, bool getCleanRider = false)
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

               var riders = await _riderService.GetAllRiders(skip / pageSize, pageSize, column, search, sort, rank, getCleanRider);

            var data = riders.Item1;
            int recordsTotal = riders.Item2;

            return Json(new
            {
                draw = draw,
                recordsFiltered = recordsTotal,
                recordsTotal = recordsTotal,
                data = data
            });
        }

        /// <summary>
        /// Edit Rider Detail
        /// </summary>
        /// <param name="riderId">Rider Id</param>
        /// <returns></returns>
        [Route("rider/detail/{riderId}")]
        [Authorize(Policy = "PagePermission")]
        public async Task<ActionResult> EditRiderDetail(int riderId)
        {
            try
            {
                return View(await _riderService.GetRiderById(riderId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update Rider Detail
        /// </summary>
        /// <param name="riderDto">The RiderDto</param>
        /// <returns></returns>        
        [HttpPost]
        [ValidateAntiForgeryToken]        
        public async Task<ActionResult> UpdateRiderDetail(RiderDto riderDto)
        {
            try
            {
                string msg = "updated";
                await _riderService.UpdateRiderDetail(riderDto);
                return RedirectToAction("index",new {mode = msg });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// Get Rider Detail
        /// </summary>
        /// <param name="riderId">The Rider Id</param>
        /// <returns>Rider Detail</returns>
        [HttpPost]
        [Authorize(Policy = "PagePermission")]
        public async Task<JsonResult> GetRiderDetail(int riderId)
        {
            try
            {
                return Json(await _riderService.GetRiderById(riderId));
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

        }

        /// <summary>
        /// Update Status
        /// </summary>
        /// <param name="riderId">The Rider Id</param>
        /// <returns>rider status detail</returns>        
        [HttpPost]
        [Route("riderstatus")]
        [Authorize(Policy = "PagePermission")]
        public async Task<JsonResult> UpdateStatus(int riderId)
        {
            try
            {
                await _riderService.UpdateStatus(riderId);
                var riderDetail = await _riderService.GetRiderById(riderId);
                return Json(riderDetail);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        /// <summary>
        /// Delete Rider
        /// </summary>
        /// <param name="riderId">The RiderId</param>
        /// <returns></returns>
        [Authorize(Policy = "PagePermission")]
        public async Task<JsonResult> DeleteRider(int riderId)
        {
            try
            {
                await _riderService.DeleteRider(riderId);
                return Json("Deleted");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        /// <summary>
        /// Delete Rider
        /// </summary>
        /// <param name="riderId">The RiderId</param>
        /// <returns></returns>
        [Authorize(Policy = "PagePermission")]
        public async Task<JsonResult> DeleteNotSeenRider(bool isparmnentdelete = false)
        {
            try
            {
                
                await _riderService.DeleteNotSeenRider(isparmnentdelete);
                return Json("Deleted");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
    }
}