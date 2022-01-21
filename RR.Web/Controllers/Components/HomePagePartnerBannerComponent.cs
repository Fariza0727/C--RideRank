using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RR.Core;
using RR.Dto;
using RR.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.Web.Controllers.Components
{
    [ViewComponent(Name = "HomePagePartnerBannerComponent")]
    //public class HomePagePartnerBannerComponent : ViewComponent
    //{
    //    #region Constructor

    //    private IConfiguration configuration;
    //    private readonly IBannerService _bannerRepository;
    //    public static AppSettings _appSettings;

    //    public HomePagePartnerBannerComponent(IBannerService bannerRepository,
    //                         IConfiguration config,
    //                          IOptions<AppSettings> appSettings)
    //    {
    //        _bannerRepository = bannerRepository;
    //        configuration = config;
    //        _appSettings = appSettings.Value;
    //    }

    //    #endregion

    //    /// <summary>
    //    /// This Method is used for news letter subscription.
    //    /// </summary>
    //    /// <returns></returns>
    //    public async Task<IViewComponentResult> InvokeAsync()
    //    {
    //        List<BannerDto> bannerList = new List<BannerDto>();
    //        var banners = await _bannerRepository.GetAllBannersRecords();

    //        foreach (var item in banners)
    //        {
    //            BannerDto dto = new BannerDto();
    //            dto.PicPath = item.PicPath;
    //            dto.Title = item.Title;
    //            dto.Url = item.Url;
    //            bannerList.Add(dto);
    //        }

    //        return View(await Task.FromResult(bannerList));
    //    }
    //}

    public class HomePagePartnerBannerComponent : ViewComponent
    {
        #region Constructor

        private IConfiguration configuration;
        private readonly IBannerService _bannerRepository;
        public static AppSettings _appSettings;

        public HomePagePartnerBannerComponent(IBannerService bannerRepository,
                             IConfiguration config,
                              IOptions<AppSettings> appSettings)
        {
            _bannerRepository = bannerRepository;
            configuration = config;
            _appSettings = appSettings.Value;
        }

        #endregion

        /// <summary>
        /// This Method is used for news letter subscription.
        /// </summary>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<SponsorDto> bannerList = new List<SponsorDto>();
            var banners = await _bannerRepository.GetAllSponserRecords();
            return View(await Task.FromResult(banners.ToList()));
        }
    }
}
