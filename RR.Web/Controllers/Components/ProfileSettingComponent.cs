using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using RR.Core;
using RR.Service;
using RR.Service.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.Web.Controllers
{
     [ViewComponent(Name = "ProfileSettingComponent")]
     public class ProfileSettingComponent : ViewComponent
     {
          private readonly UserManager<IdentityUser> _userManager;
          private readonly IUserService _userService;
          private readonly ICountryService _countryService;
          private readonly IStateService _stateService;
          private readonly ICityService _cityService;
          private readonly AppSettings _appSettings;

          public ProfileSettingComponent(
             UserManager<IdentityUser> userManager,
             IUserService userService,
             ICountryService countryService,
             IStateService stateService,
             ICityService cityService,
             IOptions<AppSettings> appSettings)
          {
               _userManager = userManager;
               _userService = userService;
               _countryService = countryService;
               _stateService = stateService;
               _cityService = cityService;
               _appSettings = appSettings.Value;
          }

          /// <summary>
          /// Uet user profile detail 
          /// </summary>
          /// <returns></returns>
          public async Task<IViewComponentResult> InvokeAsync()
          {
               var user = await _userManager.GetUserAsync(HttpContext.User);
               var userInfo = await _userService.GetUserDetail(user != null ? user.Id : "");
               if (userInfo != null)
               {
                    var userDetail = userInfo;
                    var countries = await _countryService.GetCountries();
                    var roles = await _userService.GetRoles();
                    //userInfo.PlayerTypeList = roles.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
                    //userInfo.PlayerTypeList.Insert(0, new SelectListItem() { Text = "Select Plyer Type", Value = "" });
                    
                    userInfo.CountryList = countries.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
                    userInfo.CountryList.Insert(0, new SelectListItem() { Text = "Select Country", Value = "" });

                    if (userInfo.State.HasValue)
                    {
                         var states = await _stateService.GetStates(userInfo.Country.Value);
                         var statesList = states.OrderBy(x => x.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
                         statesList.Insert(0, new SelectListItem() { Text = "Select State", Value = "" });
                         userInfo.StateList = statesList;
                    }
                    else
                    {
                         userInfo.StateList = new List<SelectListItem>();
                         userInfo.StateList.Insert(0, new SelectListItem() { Text = "Select State", Value = "" });
                    }

                    //if (userInfo.City.HasValue)
                    //{
                    //     var cities = await _cityService.GetCitiesByStateId(userInfo.State.Value);
                    //     if (cities == null && cities.Count() == 0)
                    //          cities = await _cityService.GetCitiesByCountryId(userInfo.Country.Value);
                    //     var cityList = cities.OrderBy(x => x.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
                    //     cityList.Insert(0, new SelectListItem() { Text = "Select City", Value = "" });
                    //     userInfo.CityList = cityList;
                    //}
                    //else
                    //{
                    //     userInfo.CityList = new List<SelectListItem>();
                    //     userInfo.CityList.Insert(0, new SelectListItem() { Text = "Select City", Value = "" });
                    //}

                    var playerType = roles.Where(x => Convert.ToString(x.Id) == userInfo.PlayerTypeId).FirstOrDefault();
                    var country = countries.Where(x => Convert.ToInt32(x.Id) == (userInfo.Country.HasValue ? userInfo.Country.Value : 0)).FirstOrDefault();
                    var state = await _stateService.GetStateById(userInfo.State);
                    //var city = await _cityService.GetCityById(userInfo.City);
                    userInfo.PlayerType = playerType != null ? !string.IsNullOrEmpty(playerType.Name) ? playerType.Name : "" : "";
                    userInfo.CountryName = country != null ? !string.IsNullOrEmpty(country.Name) ? country.Name : "" : "";
                    userInfo.StateName = !string.IsNullOrEmpty(state.Name) ? state.Name : "";
                    userInfo.CityName = !string.IsNullOrEmpty(userInfo.City) ? userInfo.City : ""; //!string.IsNullOrEmpty(city.Name) ? city.Name : ""
                    if (string.IsNullOrEmpty(userInfo.ReferralCode))
                    {
                        var voucherHelper = new VoucherHelper(_appSettings);
                        var referralCode = await voucherHelper.createReferalCodeAsync(user);
                        userDetail.ReferralCode = referralCode;
                        await _userService.AddEditUserDetail(userDetail);
                        userInfo.ReferralCode = referralCode;
                    }

               }
               return View(userInfo);
          }
     }
}
