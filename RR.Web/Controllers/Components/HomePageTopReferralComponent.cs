using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RR.Dto;
using RR.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.Web.Controllers
{
     [ViewComponent(Name = "HomePageTopReferralComponent")]
     public class HomePageTopReferralComponent : ViewComponent
     {
          #region Constructor

          private IConfiguration configuration;
          private readonly ITeamService _teamService;

          public HomePageTopReferralComponent(ITeamService teamService, IConfiguration config)
          {
                _teamService = teamService;
               configuration = config;
          }

          #endregion

          /// <summary>
          /// This Method is used for getting all 
          /// relevent news on home page
          /// </summary>
          /// <returns>List of all news on home page</returns>
          public async Task<IViewComponentResult> InvokeAsync()
          {
                var topReferred = await _teamService.GetTopReferred(5);
            
               return View(topReferred);
          }
     }
}
