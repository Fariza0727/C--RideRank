using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RR.AdminService;
using RR.Core;
using RR.Dto;

namespace RR.Admin.Controllers
{
    [Authorize(Roles = "Admin,Subadmin")]
    public class VideoSliderController : BaseController
    {

        #region Constructor

        private readonly IVideoSliderService _sliderService;
        private readonly IHostingEnvironment _environment;

        public VideoSliderController(IVideoSliderService sliderService, IHostingEnvironment environment, IOptions<AppSettings> appSettings) :
              base(appSettings)
        {
            _sliderService = sliderService;
            _environment = environment;
        }

        #endregion

        [Route("videosliders")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetAllSliders()
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

            var eventData = await _sliderService.GetAllVSlider(skip / pageSize, pageSize, column, search, sort);
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

        public async Task<ActionResult> AddEditSlider(int id)
        {
            VideoSliderDto model = new VideoSliderDto();
            if (id > 0)
            {
                model = await _sliderService.GetVSliderById(id);

                model.VideoPath = string.Concat(_appSettings.MainSiteURL, _appSettings.BannerSharedPath, model.VideoPath);
            }
           
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AddEditSlider(VideoSliderDto bannerDto)
        {
            try
            {
                await _sliderService.AddEditVSlider(bannerDto);
                return Json(new { status = true, message = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.GetActualError() });
            }
        }


        [HttpPost]
        public async Task<ActionResult> DeleteSlider(int Id)
        {
            try
            {
                var bannerDataDto = await _sliderService.GetVSliderById(Id);
                _sliderService.DeleteVSlider(Id);

                var filePath = Path.Combine(_appSettings.SaveasBannersWeb, bannerDataDto.VideoPath);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                return Json(new { status = true, message = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.GetActualError() });
            }
        }

        [HttpPost]
        public async Task<ActionResult> UpdateStatus(int id, bool status)
        {
            try
            {
                await _sliderService.UpdateStatus(id,status);
                return Json(new { status = true, message = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.GetActualError() });
            }
        }
    }
}