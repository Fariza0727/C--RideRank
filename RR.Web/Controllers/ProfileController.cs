using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RR.Core;
using RR.Dto;
using RR.Service;
using RR.Service.BlobStorage;
using RR.Service.User;

namespace RR.Web.Controllers
{
    [Authorize]
    public class ProfileController : BaseController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserService _userService;
        private readonly ICountryService _countryService;
        private readonly IStateService _stateService;
        private readonly ICityService _cityService;
        private readonly IHostingEnvironment _environment;
        private readonly ILogger<UserDetailDto> _logger;
        private readonly IFavoriteBullsRidersService _favoriteBullsRidersService;
        private readonly IStoreShopifyService _shopifyService;
        private readonly IContestUserWinnerService _contestUWService;
        private readonly IBlobStorageService _blobStorageService;

        public ProfileController(
           UserManager<IdentityUser> userManager,
           IUserService userService,
           ICountryService countryService,
           IStateService stateService,
           ICityService cityService,
           IHostingEnvironment environment,
           ILogger<UserDetailDto> logger,
           IFavoriteBullsRidersService favoriteBullsRidersService,
           IStoreShopifyService shopifyService,
           IContestUserWinnerService contestUWService,
           IBlobStorageService blobStorageService,
           IOptions<AppSettings> appSettings) :
            base(appSettings)
        {
            _userManager = userManager;
            _userService = userService;
            _countryService = countryService;
            _stateService = stateService;
            _cityService = cityService;
            _logger = logger;
            _favoriteBullsRidersService = favoriteBullsRidersService;
            _shopifyService = shopifyService;
            _environment = environment;
            _contestUWService = contestUWService;
            _blobStorageService = blobStorageService;
        }

        /// <summary>
        /// MyAccount
        /// </summary>
        /// <returns></returns>
        [Route("MyAccount")]
        public async Task<IActionResult> MyAccount()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.isPaidMember = await _userService.IsPaidMember(userId);
            var userDetail = await _userService.GetUserDetail(userId);
            ViewBag.UserName = userDetail.UserName;
            ViewBag.TeamName = userDetail.TeamName;
            ViewBag.UserAvatar = !string.IsNullOrEmpty(userDetail.Avtar) ? (userDetail.Avtar.Contains("https://") ? userDetail.Avtar :  (userDetail.Avtar != "/images/RR/user-n.png" ? (_appSettings.MainSiteURL + "/images/profilePicture/" + userDetail.Avtar) : _appSettings.MainSiteURL + "/images/home/team-icon.png")) : _appSettings.MainSiteURL + "/images/home/team-icon.png";
            return View();
        }

        /// <summary>
        /// Get State List
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns>State list</returns>
        [Route("bindState/{countryId}")]
        public async Task<IActionResult> GetStateList(int countryId)
        {
            var states = await _stateService.GetStates(countryId);
            var statesList = states.OrderBy(x => x.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            statesList.Insert(0, new SelectListItem() { Text = "Select State", Value = "" });
            return Json(statesList);
        }

        /// <summary>
        /// Get City List
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="countryId"></param>
        /// <returns>City List</returns>
        [Route("bindCity/{stateId}/{countryId}")]
        public async Task<IActionResult> GetCityList(int stateId, int countryId)
        {
            var cities = await _cityService.GetCitiesByStateId(stateId);
            if (cities == null && cities.Count() == 0)
                cities = await _cityService.GetCitiesByCountryId(countryId);
            var cityList = cities.OrderBy(x => x.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            cityList.Insert(0, new SelectListItem() { Text = "Select City", Value = "" });
            return Json(cityList);
        }

        /// <summary>
        /// Edit profile information
        /// </summary>
        /// <param name="detailDto"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> EditInformation(UserDetailDto detailDto, IFormCollection form)
        {
            try
            {
                var User = await _userService.GetUserDetail(detailDto.UserId);

                var userNewRole = await _userService.GetRolesById(detailDto.PlayerTypeId);

                string fileName = !string.IsNullOrEmpty(User.Avtar) ? User.Avtar : "";
                fileName = fileName.Replace("/images/profilePicture/", "");
                if (form.Files.Count > 0)
                {
                    #region Save profile pic

                    fileName = DateTime.Now.Ticks + "_" + form.Files[0].FileName;

                    /*string imagePath = _appSettings.ProfilePicPath;

                    var path = Path.Combine(_environment.ContentRootPath, imagePath, fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await form.Files[0].CopyToAsync(stream);
                    }*/
                    fileName = await _blobStorageService.UploadAvatarAsync(form, fileName);

                    #endregion
                }
                //var city = await _cityService.GetCityById(detailDto.City);
                var state = await _stateService.GetStateById(detailDto.State);
                var country = await _countryService.GetCountryById(detailDto.Country);

                detailDto.Avtar = fileName;
                detailDto.CityName = !string.IsNullOrEmpty(detailDto.City) ? detailDto.City : "";
                detailDto.StateName = state != null ? !string.IsNullOrEmpty(state.Name) ? state.Name : "" : "";
                detailDto.CountryName = country != null ? !string.IsNullOrEmpty(country.Name) ? country.Name : "" : "";
                if (detailDto.IsNotifyEmail)
                {
                    ConstantContactHelper cctHelper = new ConstantContactHelper(_appSettings);
                    await cctHelper.Resubscribed(detailDto.Email);
                }
                else
                {
                    ConstantContactHelper cctHelper = new ConstantContactHelper(_appSettings);
                    await cctHelper.Unsubscribed(detailDto.Email);
                }

                await _userService.AddEditUserDetail(detailDto);

                var user = await _userManager.GetUserAsync(HttpContext.User);
                var userInfo = await _userService.GetUserDetail(user != null ? user.Id : "");

                var customr_ = await _shopifyService.GetCustomerAsync(User.ShopifyCustomerId.ToString());
                if (userInfo.ShopifyCustomerId > 0)
                {
                    await _shopifyService.AddEditCustomerAsync(new SCustomerDto
                    {
                        Id = User.ShopifyCustomerId,
                        Addresses = new List<SAddressDto>() { new SAddressDto
                        {
                            Id = customr_.DefaultAddress?.Id,
                            Address1 = detailDto.Address1,
                            City = detailDto.CityName,
                            Country = detailDto.CountryName,
                            Zip = detailDto.ZipCode,
                            CustomerId = customr_.DefaultAddress?.CustomerId,
                            IsDefault = customr_.DefaultAddress?.Default,
                            Phone = userInfo.PhoneNumber??customr_.Phone,
                            
                        }},
                        AcceptsMarketing = customr_.AcceptsMarketing??false,
                        AdminGraphQLAPIId = customr_.AdminGraphQLAPIId,
                        VerifiedEmail = customr_.VerifiedEmail??false,
                        Email = detailDto.Email,
                        FirstName = userInfo.FirstName,
                        LastName = userInfo.LastName,
                        Phone = userInfo.PhoneNumber?? customr_.Phone,

                    });
                }

                await _userManager.RemoveFromRoleAsync(user, userInfo.PlayerType);
                await _userManager.AddToRoleAsync(user, userNewRole);

                _logger.LogInformation("User account information updated successfully.");
                return Content("<div class='alert alert-success alert-dismissible' style='background-color: #2ebc9b;' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'></button><strong>Success!!!</strong> Profile updated successfully.</div>");
            }
            catch (Exception ex)
            {
                return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'></button><strong>Oops!!! Something went wrong. Got following error :- " + ex.Message + "</div>");
            }
        }


        /// <summary>
        /// Remove Favorite Bull
        /// </summary>
        /// <param name="bullId">bull Id</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> RemoveFavoriteBull(int bullId)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                try
                {
                    await _favoriteBullsRidersService.RemoveUserFavoriteBulls(userId, bullId);
                    return Json(new { status = true, message = "Successfully removed favorite bull" });
                }
                catch (Exception ex)
                {
                    return Json(new { status = false, message = "Oops! something wrong" });
                }
            }

            return Json(new { status = false, message = "Oops! invalid user" });

        }

        /// <summary>
        /// Remove Favorite Rider  
        /// </summary>
        /// <param name="riderId">rider Id</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> RemoveFavoriteRider(int riderId)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                try
                {
                    await _favoriteBullsRidersService.RemoveUserFavoriteRider(userId, riderId);
                    return Json(new { status = true, message = "Successfully removed favorite rider" });
                }
                catch (Exception ex)
                {
                    return Json(new { status = false, message = "Oops! something wrong" });
                }
            }
            return Json(new { status = false, message = "Oops! invalid user" });
        }

        [HttpPost]
        [Route("GetCurrentUserWinningsAjax")]
        public async Task<JsonResult> GetCurrentUserWinningsAjax()
        {

            var draw = Request.Form["draw"];
            var start = Request.Form["start"];
            var length = Request.Form["length"];
            //Global search field

            var contestDetail = await _contestUWService.GetUserWinnings(_userManager.GetUserId(HttpContext.User));

            var data = contestDetail.OrderByDescending(x => x.CreatedDate).Skip(Convert.ToInt32(start)).Take(Convert.ToInt32(length));
            int recordsTotal = contestDetail.Count();
            var temp = new Tuple<ContestUserWinnerDto, ContestUserWinnerDto, ContestUserWinnerDto>[3];
            temp[0] = new Tuple<ContestUserWinnerDto, ContestUserWinnerDto, ContestUserWinnerDto>(data.Count() > 0 ? data.ElementAt(0) : null, data.Count() > 1 ? data.ElementAt(1) : null, data.Count() > 2 ? data.ElementAt(2) : null);
            temp[1] = new Tuple<ContestUserWinnerDto, ContestUserWinnerDto, ContestUserWinnerDto>(data.Count() > 3 ? data.ElementAt(3) : null, data.Count() > 4 ? data.ElementAt(4) : null, data.Count() > 5 ? data.ElementAt(5) : null);
            temp[2] = new Tuple<ContestUserWinnerDto, ContestUserWinnerDto, ContestUserWinnerDto>(data.Count() > 6 ? data.ElementAt(6) : null, data.Count() > 7 ? data.ElementAt(7) : null, data.Count() > 8 ? data.ElementAt(8) : null);



            return Json(new
            {
                draw,
                recordsFiltered = recordsTotal,
                recordsTotal,
                data = temp,
            });
        }
    }
}