using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RR.Core;
using RR.Dto;
using RR.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.Web.Controllers
{
     [ViewComponent(Name = "TopRecentComponent")]
     public class TopRecentComponent : ViewComponent
     {
          #region Constructor

          private AppSettings _appSettings;
          private readonly INewsService _newsservice;

          public TopRecentComponent(INewsService newsservice,
               IOptions<AppSettings> appSettings)
          {
               _newsservice = newsservice;
               _appSettings = appSettings.Value;
          }

          #endregion

          /// <summary>
          /// Invoke Method
          /// </summary>
          /// <returns>Get All Top Recent News On View</returns>
          public async Task<IViewComponentResult> InvokeAsync()
          {
               var newsResult = await _newsservice.GetTopRecentNews();
               IEnumerable<NewsDto> recent = newsResult.Item1;
               IEnumerable<NewsDto> top = newsResult.Item2.OrderByDescending(x => x.NewsDate);
            //ViewBag.NewsPicPath = _appSettings.MainSiteURL + "shared/" + _appSettings.NewsSharedPath;// _appSettings.AdminSiteURL + "assets/NewsImage/";
            ViewBag.NewsPicPath = _appSettings.MainSiteURL + _appSettings.NewsSharedPath;
            return View(new Tuple<IEnumerable<NewsDto>, IEnumerable<NewsDto>>(recent, top));
          }
     }
}
