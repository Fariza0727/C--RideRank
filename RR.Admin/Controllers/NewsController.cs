using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RR.AdminService;
using RR.Core;
using RR.Dto;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;


namespace RR.Admin.Controllers
{
    [Authorize(Roles = "Admin,Subadmin")]    
    public class NewsController : BaseController
    {
        #region Constructor

        private readonly INewsService _newsService;
        private IHostingEnvironment _env;

        public NewsController(INewsService newsService,
            IHostingEnvironment env,
             IOptions<AppSettings> appSettings) :
            base(appSettings)
        {
            _newsService = newsService;
            _env = env;
        }

        #endregion

        /// <summary>
        /// News Index Page
        /// </summary>
        /// <returns></returns>        
        [Route("news")]
        [Authorize(Policy = "PagePermission")]
        public IActionResult Index(string mode ="" )
        {
            if (!string.IsNullOrEmpty(mode))
            {
                ViewBag.Message = mode;
            }
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetAllNews()
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

            var eventData = await _newsService.GetAllNewsRecords(skip / pageSize, pageSize, column, search, sort);

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
        /// Add Edit News Detail
        /// </summary>
        /// <param name="id">News Id</param>
        /// <returns></returns>
        [Authorize(Policy = "PagePermission")]
        public async Task<ActionResult> AddEditNews(int id)
        {
            NewsDto model = new NewsDto();
            if (id > 0)
            {
                var newsData = await _newsService.GetNewsById(id);
                model.Id = newsData.Id;
                model.NewsContent = newsData.NewsContent;
                model.NewsDate = newsData.NewsDate;
                model.Title = newsData.Title;
                model.PicPath = newsData.PicPath;
                model.NewsTag = newsData.NewsTag;
                model.VideoPath = newsData.VideoPath;
                model.VideoUrl = newsData.VideoUrl;
                //model.ShowImage = _appSettings.MainSiteURL + "shared/" + _appSettings.NewsSharedPath + "/" + newsData.PicPath; // "/assets/NewsImage/" + newsData.PicPath;
                if(!string.IsNullOrEmpty(newsData.VideoPath))
                model.ShowVideo =_appSettings.MainSiteURL +_appSettings.NewsSharedPath + "/" + newsData.VideoPath; // "/assets/NewsImage/" + newsData.PicPath;
                else
                model.ShowImage = _appSettings.MainSiteURL + _appSettings.NewsSharedPath + "/" + newsData.PicPath; // "/assets/NewsImage/" + newsData.PicPath;

            }
            else
            {
                model.NewsDate = DateTime.Now;
                model.ShowImage = "/lib/defaultImage.png";
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddEditNews(NewsDto newsDto)
        {
            try
            {
                if(newsDto.Id == 0 && newsDto.Image == null && string.IsNullOrEmpty(newsDto.VideoUrl))
                {
                    ModelState.AddModelError("Image", "Media file requried");
                    return View(newsDto);
                }

                string msg = "";
                if (newsDto.Image != null && newsDto.Image.Length != 0)
                {
                  
                    TimeSpan span = DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
                    string createdFileName = span.TotalSeconds + newsDto.Image.FileName.Substring(newsDto.Image.FileName.LastIndexOf("."));
                    
                    //string newsImagePath = _appSettings.NewsImagePath;
                    //var webRoot = _env.WebRootPath;
                    //var path = Path.Combine(webRoot, newsImagePath, createdFileName);

                    //var path = Path.Combine(@"D:\DynamicData\images\news", createdFileName);
                    var path = Path.Combine(_appSettings.SaveasNewsWeb, createdFileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await newsDto.Image.CopyToAsync(stream);
                    }

                    if (newsDto.Image.ContentType.Contains("video"))
                        newsDto.VideoPath = createdFileName;
                    else
                        newsDto.PicPath = createdFileName;
                    
                }
                else if (newsDto.Id == 0)
                {
                    newsDto.PicPath = string.Empty;
                }
                await _newsService.AddEditNews(newsDto, HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

                if (newsDto.Id == 0)
                {
                    msg = "Added";
                }else
                {
                    msg = "Updated";
                }

                return RedirectToAction(nameof(Index) , new {mode= msg  });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Delete News
        /// </summary>
        /// <param name="Id">The News Id</param>
        /// <returns></returns>        
        [HttpPost]
        [Authorize(Policy = "PagePermission")]
        public async Task<ActionResult> DeleteNews(int Id)
        {
            try
            {
                var newsDataDto = await _newsService.GetNewsById(Id);
                _newsService.DeleteNews(Id);

                string newsImagePath = _appSettings.NewsImagePath;
                //var filePath = Path.Combine(Directory.GetCurrentDirectory(), newsImagePath, newsDataDto.PicPath);
                //var filePath = Path.Combine(@"D:\DynamicData\images\news", newsDataDto.PicPath);
                var filePath = Path.Combine(_appSettings.SaveasNewsWeb, newsDataDto.PicPath);

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