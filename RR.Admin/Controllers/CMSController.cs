using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RR.AdminService;
using RR.Dto;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;



namespace RR.Admin.Controllers
{
    [Authorize(Roles = "Admin,Subadmin")]
    public class CmsController : Controller
    {
        #region Constructor

        private readonly ICmsService _cmsService;

        public CmsController(ICmsService cmsService)
        {
            _cmsService = cmsService;
        }

        #endregion

        /// <summary>
        /// Cms Index Page
        /// </summary>
        /// <returns>Cms Index View</returns>
        [Authorize(Policy = "PagePermission")]
        [Route("cms")]
        public ActionResult Index(string mode = "")
        {
            if (!string.IsNullOrEmpty(mode))
            {
                ViewBag.Message = mode;
            }
            return View();
        }

        /// <summary>
        /// Get All Bulls
        /// </summary>
        /// <returns>List Of 10 Bulls</returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> GetAllCms()
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

            var cms = await _cmsService.GetAllCms(skip, pageSize, column, search, sort);

            var data = cms.Item1;
            int recordsTotal = cms.Item2;

            return Json(new
            {
                draw,
                recordsFiltered = recordsTotal,
                recordsTotal,
                data
            });
        }

        /// <summary>
        /// Create Edit Cms
        /// </summary>
        /// <param name="id">Cms Id</param>
        /// <returns></returns>
        [Authorize(Policy = "PagePermission")]
        public async Task<ActionResult> CreateEditCms(int id)
        {
            CmsDto model = new CmsDto();
            if (id > 0)
            {
                return View(await _cmsService.GetCmsById(id));

            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "PagePermission")]
        public async Task<ActionResult> CreateEditCms(CmsDto model)
        {
            string msg = "";
            await _cmsService.AddEditCms(model, HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (model.Id == 0)
            {
                msg = "Added";
            }
            else
            {
                msg = "Updated";
            }
            return RedirectToAction("Index", new { mode = msg });
        }


        [HttpPost]
        public bool PageNameIsExist(string PageName, int Id)
        {
            if (Id > 0)
                return true;
            return _cmsService.IsExistCmsRecordName(PageName);
        }

        /// <summary>
        /// Delete Cms
        /// </summary>
        /// <param name="id">Cms Id</param>
        /// <returns></returns>
        [Authorize(Policy = "PagePermission")]
        [HttpDelete]
        public async Task<JsonResult> DeleteCMS(int id)
        {
            try
            {
                await _cmsService.DeleteCmsById(id);
                return Json("Deleted");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
    }
}