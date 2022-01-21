using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RR.Admin.Models;
using RR.AdminService;
using RR.Dto;
using System;
using System.Threading.Tasks;

namespace RR.Admin.Controllers
{
    [Authorize(Roles = "Admin,Subadmin")]
    [ServiceFilter(typeof(AdminExceptionFilter))]
    public class BullController : Controller
    {
        #region Constructor

        private readonly IBullService _bullService;
        private readonly ILogger<BullController> _logger;
        public BullController(IBullService bullService, ILogger<BullController> logger)
        {
            _bullService = bullService;
            _logger = logger;
        }

        #endregion

        /// <summary>
        /// Bull Index Page
        /// </summary>
        /// <returns></returns>        
        [Route("bulls")]
        [Authorize(Policy = "PagePermission")]
        public ActionResult Index(string mode = "")
        {
            if (!string.IsNullOrEmpty(mode))
            {
                ViewBag.Message = "updated";
            }
            return View();
        }

        /// <summary>
        /// Edit Bull Detail
        /// </summary>
        /// <param name="bullId">Bull Id</param>
        /// <returns></returns>        
        [Route("bull/detail/{bullId}")]
        [Authorize(Policy = "PagePermission")]
        public async Task<ActionResult> EditBullDetail(int bullId)
        {
            try
            {
                return View(await _bullService.GetBullById(bullId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update Bull Detail
        /// </summary>
        /// <param name="bullDto">The BullDto</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateBullDetail(BullDto bullDto)
        {
            try
            {
                string msg = "updated";
                await _bullService.UpdateBullDetail(bullDto);

                return RedirectToAction("index", new { mode = msg });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("bull/getallbulls/{rank}/{getCleanbull?}")]
        public async Task<JsonResult> GetAllBulls(int rank = 1, bool getCleanbull = false)
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

            var bulls = await _bullService.GetAllBulls(skip / pageSize, pageSize, column, search, sort, rank, getCleanbull);

            var data = bulls.Item1;
            int recordsTotal = bulls.Item2;

            return Json(new
            {
                draw = draw,
                recordsFiltered = recordsTotal,
                recordsTotal = recordsTotal,
                data = data
            });
        }

        /// <summary>
        /// Update Status
        /// </summary>
        /// <param name="bullId">The Bull Id</param>
        /// <returns>bull status detail</returns>
        [HttpPost]
        [Route("bullstatus")]
        public async Task<JsonResult> UpdateStatus(int bullId)
        {
            try
            {
                await _bullService.UpdateStatus(bullId);
                var bullDetail = await _bullService.GetBullById(bullId);
                return Json(bullDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);
                return Json(ex.Message);
            }
        }

        /// <summary>
        /// Delete Bull
        /// </summary>
        /// <param name="bullId">The BullId</param>
        /// <returns></returns>
        [Authorize(Policy = "PagePermission")]
        public async Task<JsonResult> DeleteBull(int bullId)
        {
            try
            {
                await _bullService.DeleteBullById(bullId);
                return Json("Deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);
                return Json(ex.Message);
            }
        }

        /// <summary>
        /// Get Bull Detail
        /// </summary>
        /// <param name="bullId">The Bull Id</param>
        /// <returns>Bull Detail</returns>
        [HttpPost]
        [Authorize(Policy = "PagePermission")]
        public async Task<JsonResult> GetBullDetail(int bullId)
        {
            try
            {
                
                return Json(await _bullService.GetBullById(bullId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);
                return Json(ex.Message);
            }
        }
        /// <summary>
        /// Delete Rider
        /// </summary>
        /// <param name="riderId">The RiderId</param>
        /// <returns></returns>
        [Authorize(Policy = "PagePermission")]
        public async Task<JsonResult> DeleteNotSeenBulls(bool isparmnentdelete = false)
        {
            try
            {

                await _bullService.DeleteNotSeenBulls(isparmnentdelete);
                return Json("Deleted");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
    }
}