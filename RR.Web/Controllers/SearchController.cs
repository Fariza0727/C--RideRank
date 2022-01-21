using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RR.Core;
using RR.Service;
using System.Linq;
using System.Threading.Tasks;

namespace RR.Web.Controllers
{
     public class SearchController : BaseController
     {
          #region Constructor

          private readonly ISearchResultService _searchResult;

          public SearchController(ISearchResultService searchResult,
                                IOptions<AppSettings> appSettings) :
                base(appSettings)
          {
               _searchResult = searchResult;
          }

          #endregion

          /// <summary>
          /// Search Index
          /// </summary>
          /// <param name="keyword">The searching Keyword</param>
          /// <param name="page">Page Number</param>
          /// <returns>Results Of All Related Keyword Search on Search View</returns>
          public async Task<IActionResult> Index(string keyword, int? page)
          {
            if (!string.IsNullOrEmpty(keyword))
            {
                ViewBag.NewsPicPath = _appSettings.MainSiteURL + "shared/" + _appSettings.NewsSharedPath; //_appSettings.AdminSiteURL + "assets/NewsImage/";
                int pageNumber = (page ?? 0);
                var data = await _searchResult.GetSearchResults(keyword, pageNumber);
                pageNumber = (pageNumber == 0 ? 1 : pageNumber);
                data.PageNumber = pageNumber;
                data.Keyword = keyword;
                ViewBag.picpath = _appSettings.MainSiteURL;
                return View(data);
            }
            return RedirectToAction("Index", "Home");
          }
     }
}