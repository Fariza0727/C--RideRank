using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using RR.AdminData;
using RR.AdminService;
using RR.Core;
using RR.Dto;

namespace RR.Admin.Controllers
{
    [Authorize(Roles = "Admin,Subadmin")]
    public class PicturesManagersController : Controller
    {
        private readonly IPictureManagerService _pictureManager;
        private readonly IBullService _BullService;
        private readonly IRiderService _riderService;
        private readonly AppSettings _appSettings;

        public PicturesManagersController(
            IPictureManagerService pictureManager, 
            IBullService bullService,
            IRiderService riderService,
            IOptions<AppSettings> appSettings)
        {
            _pictureManager = pictureManager;
            _BullService = bullService;
            _riderService = riderService;
            _appSettings = appSettings.Value;
        }

        /// <summary>
        /// Riders/Bulls pictures 
        /// </summary>
        /// <returns></returns>        
        [Route("picturesmanagers/{isBool?}")]
        [Authorize(Policy = "PagePermission")]
        public ActionResult Index(bool isBool = false)
        {
            TempData["IsBull"] = isBool;
            return View();
        }

        [HttpPost]
        [Route("picturesmanagers/getallpictures/{isBull}")]
        public async Task<JsonResult> GetAllPictures(bool isBull = false)
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

            var bulls = await _pictureManager.GetAllPictures(skip / pageSize, pageSize, column, search, sort, isBull);

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


        
        [Authorize(Policy = "PagePermission")]
        public async Task<IActionResult> AddEditPicture(long id = 0, bool isBull = false)
        {
            PictureManagerDto model = new PictureManagerDto();
            model.RiderManager = new RidermanagerDto();
            if (id > 0)
            {
                model = await _pictureManager.GetPicture(id)??new PictureManagerDto();
                
            }
            model.IsBull = isBull;
            TempData["IsBull"] = isBull;

            var added  = await _pictureManager.AddRiderBullPictursIds();
            model.Bulls = (await _BullService.GetBulls(model.BullId ?? 0)).Select(r => CheckIsAdded(added.Item2, r));
            model.Riders = (await _riderService.GetRiders(model.RiderId ?? 0)).Select(r => CheckIsAdded(added.Item1, r));
            if (model.RiderId > 0)
            {
                model.RiderManager = await _pictureManager.GetRiderManager(model.RiderId??0)??new RidermanagerDto();
            }

            if(model.RiderManager.SocialLinks.Count==0)
                model.RiderManager.addSocial(1);

            return View(model);
        }

        
        [Authorize(Policy = "PagePermission")]
        [HttpPost]
        public async Task<JsonResult> AddEditPicture(PictureManagerDto pictureManager)
        {
            try
            {
                if (pictureManager.File?.Length > 0 && !(pictureManager.BullId == null && pictureManager.RiderId == null))
                {
                    var extenstions = new string[] { ".jpg", ".png", ".jpeg" };

                    if (!extenstions.Contains(Path.GetExtension(pictureManager.File.FileName)))
                        return Json(new { status = false, message = "Select only image file" });


                    TimeSpan span = DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
                    string createdFileName = span.TotalSeconds + pictureManager.File.FileName.Substring(pictureManager.File.FileName.LastIndexOf("."));

                    var path1 = Path.Combine(_appSettings.SaveasbullriderImage, createdFileName);
                    using (var stream1 = new FileStream(path1, FileMode.Create))
                    {
                        await pictureManager.File.CopyToAsync(stream1);
                    }

                    if (pictureManager.IsBull)
                    {
                        pictureManager.BullPicture = createdFileName;
                    }
                    else
                    {
                        pictureManager.RiderPicture = createdFileName;
                    }

                    await _pictureManager.AddEditPicture(pictureManager);
                }

                await _pictureManager.AddEditRiderManager(pictureManager.RiderManager, pictureManager.RiderId ?? 0);
                return Json(new { status = true, isbull = pictureManager.IsBull });
            }
            catch (Exception ex)
            {

                return Json(new { status = false, message = ex.GetActualError() });
            }

        }

        
        [HttpPost]
        [Authorize(Policy = "PagePermission")]
        public async Task<JsonResult> DeletePicture(long id)
        {

            if (id > 0)
            {
                
                try
                {

                    string fileName = "";
                    var file_ = await _pictureManager.GetPicture(id);
                    fileName = file_?.RiderPicture;
                    if(string.IsNullOrEmpty(fileName))
                        fileName = file_?.BullPicture; 

                    await _pictureManager.DeletePicture(id);

                    if (System.IO.File.Exists(string.Concat(_appSettings.SaveasbullriderImage, fileName)))
                    {
                        System.IO.File.Delete(string.Concat(_appSettings.SaveasbullriderImage, fileName));
                    };

                    return Json(new { status = true });
                }
                catch (Exception ex)
                {
                    return Json(new { status = false, message = ex.GetActualError() });
                }
            }

            return Json(new { status = false, message = "file not found" });


        }

        private SelectListItem CheckIsAdded(List<int> addIds, SelectListItem listItem)
        {
            if (addIds.Contains(Convert.ToInt32(listItem.Value)))
                listItem.Disabled = true;

            return listItem;
        }

    }


}