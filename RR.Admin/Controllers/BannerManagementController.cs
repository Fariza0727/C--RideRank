using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RR.Admin.Models;
using RR.AdminService;
using RR.Core;
using RR.Dto;
using System;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RR.Admin.Controllers
{
    [Authorize(Roles = "Admin,Subadmin")]
    public class BannerManagementController : BaseController
    {
        #region Constructor

        private readonly IBannerService _bannerService;
        private readonly IHostingEnvironment _environment;

        public BannerManagementController(IBannerService bannerService, IHostingEnvironment environment, IOptions<AppSettings> appSettings) :
              base(appSettings)
        {
            _bannerService = bannerService;
            _environment = environment;
        }

        #endregion

        /// <summary>
        /// Banner Index page
        /// </summary>
        /// <returns></returns>
        [Route("banner")]
        [Authorize(Policy = "PagePermission")]
        public IActionResult Index(string mode = "")
        {
            if (!string.IsNullOrEmpty(mode))
            {
                ViewBag.Message = mode;
            }
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetAllBanners()
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

            var eventData = await _bannerService.GetAllBannersRecords(skip / pageSize, pageSize, column, search, sort);
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

        /// <summary>
        /// Add Edit Banner Detail
        /// </summary>
        /// <param name="id">Banner Id</param>
        /// <returns></returns>
        [Authorize(Policy = "PagePermission")]
        public async Task<ActionResult> AddEditBanner(int id)
        {
            BannerDto model = new BannerDto();
            if (id > 0)
            {
                var bannerData = await _bannerService.GetBannerById(id);
                model.Id = bannerData.Id;
                model.PicPath = bannerData.PicPath;
                //model.Title = bannerData.Title;
                // model.Url = bannerData.Url;
                //model.ShowImage = _appSettings.MainSiteURL + "shared/" + _appSettings.BannerSharedPath + "/" + bannerData.PicPath; //"/assets/BannerImage/" + bannerData.PicPath;
                model.ShowImage = _appSettings.MainSiteURL + _appSettings.BannerSharedPath + "/" + bannerData.PicPath;
            }
            else
            {
                model.ShowImage = "/lib/defaultImage.png";
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "PagePermission")]
        public async Task<ActionResult> AddEditBanner(BannerDto bannerDto)
        {
            try
            {
                string msg = "";
                if (bannerDto.Image != null && bannerDto.Image.Length != 0)
                {
                    TimeSpan span = DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
                    string createdFileName = span.TotalSeconds + bannerDto.Image.FileName.Substring(bannerDto.Image.FileName.LastIndexOf("."));

                    //string newsImagePath = _appSettings.BannerImagePath;

                    //var path = Path.Combine(_environment.ContentRootPath, newsImagePath, createdFileName);
                    //using (var stream = new FileStream(path, FileMode.Create))
                    //{
                    //    await bannerDto.Image.CopyToAsync(stream);
                    //}
                    try
                    {
                        // 1056880-web1
                        //var path1 = Path.Combine(@"\\172.24.32.172\DynamicData\images\banner", createdFileName);
                        //var path1 = Path.Combine(@"D:\DynamicData\images\banner", createdFileName);
                        var path1 = Path.Combine(_appSettings.SaveasBannersWeb, createdFileName);
                        //using (NetworkConnection netConn = new NetworkConnection(@"\\172.24.32.172", new NetworkCredential("1056880-admin", "gti4eBrkwDTh")))
                        //{
                        using (var stream1 = new FileStream(path1, FileMode.Create))
                        {
                            await bannerDto.Image.CopyToAsync(stream1);
                        }

                        ////using (FileStream fs = System.IO.File.Create(path1))
                        ////{
                        ////    bannerDto.Image.CopyTo(fs);
                        ////    fs.Flush();
                        ////}

                        //}
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    bannerDto.PicPath = createdFileName;
                }
                else if (bannerDto.Id == 0)
                {
                    bannerDto.PicPath = string.Empty;
                }
                await _bannerService.AddEditBanner(bannerDto, HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (bannerDto.Id == 0)
                {
                    msg = "Added";
                }
                else
                {
                    msg = "Updated";
                }
                return RedirectToAction(nameof(Index), new { mode = msg });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Delete Banner
        /// </summary>
        /// <param name="Id">The Banner Id</param>
        /// <returns></returns>   
        [HttpGet]
        [Authorize(Policy = "PagePermission")]
        public async Task<ActionResult> DeleteBanner(int Id)
        {
            try
            {
                var bannerDataDto = await _bannerService.GetBannerById(Id);
                _bannerService.DeleteBanner(Id);

                string bannerImagePath = _appSettings.BannerImagePath;
                //var filePath = Path.Combine(Directory.GetCurrentDirectory(), bannerImagePath, bannerDataDto.PicPath);
                //var filePath = Path.Combine(@"D:\DynamicData\images\banner", bannerDataDto.PicPath);
                var filePath = Path.Combine(_appSettings.SaveasBannersWeb, bannerDataDto.PicPath);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                return Ok("Deleted");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
    }
}