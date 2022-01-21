using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RR.Core;
using RR.Dto;
using RR.Service;
using RR.Service.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.Web.Controllers
{
    [ViewComponent(Name = "HomePageSliderComponent")]
    public class HomePageSliderComponent : ViewComponent
    {
        #region Constructor

        private IConfiguration configuration;
        private readonly INewsService _newsservice;
        private readonly IBannerService _bannerService;
        public static AppSettings _appSettings;

        public HomePageSliderComponent(INewsService newsservice, IConfiguration config,
             IBannerService bannerService,
             IOptions<AppSettings> appSettings)
        {
            _newsservice = newsservice;
            configuration = config;
            _bannerService = bannerService;
            _appSettings = appSettings.Value;
        }

        #endregion

        /// <summary>
        /// Uet user profile detail 
        /// </summary>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            //ViewBag.BannerImagePath = _appSettings.MainSiteURL + "shared/" + _appSettings.BannerSharedPath;
            ViewBag.BannerImagePath = _appSettings.MainSiteURL + _appSettings.BannerSharedPath;
            var bannerList = await _bannerService.GetAllBannersRecords();
            return View(bannerList);
        }
    }
}
