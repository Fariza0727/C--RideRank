using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RR.Dto;
using RR.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.Web.Controllers.Components
{
     [ViewComponent(Name = "SearchComponent")]
     public class SearchComponent : ViewComponent
     {
          #region Constructor

          private IConfiguration configuration;
          private readonly ISearchResultService _searchResult;

          public SearchComponent(ISearchResultService searchResult,
                               IConfiguration config)
          {
               _searchResult = searchResult;
               configuration = config;
          }

          #endregion

          /// <summary>
          /// This Method is used for news letter subscription.
          /// </summary>
          /// <returns></returns>
          public async Task<IViewComponentResult> InvokeAsync()
          {
               SearchDto model = new SearchDto();
               return View(await Task.FromResult(model));
          }
     }
}
