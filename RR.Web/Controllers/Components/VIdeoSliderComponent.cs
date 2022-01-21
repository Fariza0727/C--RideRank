using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RR.AdminData;
using RR.Core;
using RR.Dto;
using RR.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.Web.Controllers.Components
{

    [ViewComponent(Name = "VideoSliderComponent")]
    public class VideoSliderComponent : ViewComponent
    {
        #region Constructor

        public static AppSettings _appSettings;
        private readonly IRepository<VideoSlider, RankRideAdminContext> _repoVSlider;
        public VideoSliderComponent(IRepository<VideoSlider, RankRideAdminContext> repoVSlider,
                              IOptions<AppSettings> appSettings)
        {
            _repoVSlider = repoVSlider;
            _appSettings = appSettings.Value;
        }

        #endregion

        /// <summary>
        /// This Method is used for news letter subscription.
        /// </summary>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var bannerList = _repoVSlider.Query().Filter(d=>d.IsActive).Get().Select(r=> new VideoSliderDto
            {
                Id = r.Id,
                IsUrl = !string.IsNullOrEmpty(r.VideoUrl),
                VideoPath = string.Concat(_appSettings.BannerSharedPath, r.VideoPath),
                VideoUrl= r.VideoUrl
            });

            return View(await Task.FromResult(bannerList));
        }
    }
}
