using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RR.Core;
using RR.Dto;
using RR.Service;
using RR.Service.User;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RR.WebApi.Controllers
{
     [Route("api/[controller]")]
     [ApiController]
     public class ProfileController : BaseController
     {
          private readonly UserManager<IdentityUser> _userManager;
          private readonly IUserService _userService;
          private readonly ICountryService _countryService;
          private readonly IStateService _stateService;
          private readonly ICityService _cityService;
          private readonly IHostingEnvironment _environment;
          private readonly ILogger<UserDetailDto> _logger;
          private readonly ITransactionService _transactionService;
          private readonly SignInManager<IdentityUser> _signInManager;
          private readonly ITransactionService _transService;
        private readonly IStoreShopifyService _shopifyService;

        public ProfileController(
             UserManager<IdentityUser> userManager,
             IUserService userService,
             ICountryService countryService,
             IStateService stateService,
             ICityService cityService,
             IHostingEnvironment environment,
             ILogger<UserDetailDto> logger,
             IOptions<AppSettings> appSettings,
             ITransactionService transactionService,
             SignInManager<IdentityUser> signInManager,
             ITransactionService transService, IStoreShopifyService shopifyService) :
              base(appSettings)
          {
               _userManager = userManager;
               _userService = userService;
               _countryService = countryService;
               _stateService = stateService;
               _cityService = cityService;
               _logger = logger;
               _environment = environment;
               _transactionService = transactionService;
               _signInManager = signInManager;
               _transService = transService;
            _shopifyService = shopifyService;
        }
          /// <summary>
          /// Get State List
          /// </summary>
          /// <param name="countryId"></param>
          /// <returns>State list</returns>
          [HttpGet]
          [Route("bindstate/{countryId}")]
          public async Task<OkObjectResult> GetStateList(int countryId)
          {
               var states = await _stateService.GetStates(countryId);
               var statesList = states.OrderBy(x => x.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
               statesList.Insert(0, new SelectListItem() { Text = "Select State", Value = "" });
               return Ok(new ApiResponse
               {
                    Data = statesList,
                    IpAddress = Helpers.GetIpAddress(),
                    Success = true,
                     Message = ""
               });
          }

          /// <summary>
          /// Restrict Player According to its State so that will show only 
          /// those player type which are eligible to that state.
          /// </summary>
          /// <param name="state">State Name</param>
          /// <param name="city">City Name</param>
          /// <param name="dob">Date Of Birth</param>
          /// <returns></returns>
          [HttpPost]
          [Route("restrictplayer")]
          public async Task<OkObjectResult> EnableValidPlayer([FromForm] string state, [FromForm] string zipcode,
               [FromForm] string city, [FromForm] string dob)
          {
               try
               {
                    var date = DateTime.ParseExact(dob, "dd/MM/yyyy", null);
                    var states = await _stateService.RestrictPlayer(state, date);
                    if (!string.IsNullOrEmpty(states.ErrorMessage))
                    {
                         return Ok(new ApiResponse
                         {
                              Message = states.ErrorMessage,
                              IpAddress = Helpers.GetIpAddress(),
                              Success = false
                         });
                    }
                    return Ok(new ApiResponse
                    {
                         Data = states,
                         IpAddress = Helpers.GetIpAddress(),
                         Success = true,
                         Message = ""
                    });
               }
               catch (Exception ex)
               {
                    return Ok(new ApiResponse
                    {
                         Message = ex.Message,
                         IpAddress = Helpers.GetIpAddress(),
                         Success = false
                    });
               }
          }

          /// <summary>
          /// Get City List
          /// </summary>
          /// <param name="stateId"></param>
          /// <param name="countryId"></param>
          /// <returns>City List</returns>
          [HttpGet]
          [Route("bindcity/{stateId}/{countryId}")]
          public async Task<OkObjectResult> GetCityList(int stateId, int countryId)
          {
               var cities = await _cityService.GetCitiesByStateId(stateId);
               if (cities == null && cities.Count() == 0)
                    cities = await _cityService.GetCitiesByCountryId(countryId);
               var cityList = cities.OrderBy(x => x.Name).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
               cityList.Insert(0, new SelectListItem() { Text = "Select City", Value = "" });
               return Ok(new ApiResponse
               {
                    Data = cityList,
                    IpAddress = Helpers.GetIpAddress(),
                    Success = true,
                    Message = ""
               });
          }

          /// <summary>
          /// Get User Profile
          /// </summary>
          /// <param name="userId">User Id</param>
          /// <returns></returns>
          [HttpPost]
          [Route("userprofile")]
          public async Task<OkObjectResult> GetUserProfile([FromForm] string userId)
          {
               try
               {
                    var userInfo = await _userService.GetUserDetail(userId);
          
                    if (userInfo != null)
                    {
                         userInfo.Avtar = _appSettings.MainSiteURL + userInfo.Avtar.TrimStart('/');
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

                         var playerType = roles.Where(x => Convert.ToString(x.Id) == userInfo.PlayerTypeId).FirstOrDefault();
                         var country = countries.Where(x => Convert.ToInt32(x.Id) == (userInfo.Country.HasValue ? userInfo.Country.Value : 0)).FirstOrDefault();
                         var state = await _stateService.GetStateById(userInfo.State);

                         userInfo.PlayerType = playerType != null ? !string.IsNullOrEmpty(playerType.Name) ? playerType.Name : "N/A" : "N/A";
                         userInfo.CountryName = country != null ? !string.IsNullOrEmpty(country.Name) ? country.Name : "N/A" : "N/A";
                         userInfo.StateName = !string.IsNullOrEmpty(state.Name) ? state.Name : "N/A";
                         userInfo.CityName = !string.IsNullOrEmpty(userInfo.City) ? userInfo.City : "";
                         userInfo.DateOfBirth = userInfo.DOB?.ToString("dd/MM/yyyy");
                    }
                    return Ok(new ApiResponse
                    {
                         Data = userInfo,
                         IpAddress = Helpers.GetIpAddress(),
                         Success = true,
                         Message = ""
                    });
               }
               catch (Exception ex)
               {
                    return Ok(new ApiResponse
                    {
                         Message = ex.Message,
                         IpAddress = Helpers.GetIpAddress(),
                         Success = false
                    });
               }

          }

          /// <summary>
          /// Edit profile information
          /// </summary>
          /// <param name="detailDto"></param>
          /// <param name="form"></param>
          /// <returns></returns>
          [HttpPost]
          [Route("editinformation")]
          public async Task<OkObjectResult> EditInformation([FromForm] UserProfileDetailDto userProfileDetailDto)
          {
               try
               {
                    var detailDto = new UserDetailDto
                    {
                         Address1 = userProfileDetailDto.Address1,
                         Address2 = userProfileDetailDto.Address2,
                         Address3 = userProfileDetailDto.Address3,
                         Banking = userProfileDetailDto.Banking,
                         City = userProfileDetailDto.City,
                         State = userProfileDetailDto.State,
                         Country = userProfileDetailDto.Country,
                         DOB = DateTime.ParseExact(userProfileDetailDto.DateOfBirth, "dd/MM/yyyy", null),
                         Email = userProfileDetailDto.Email,
                         FirstName = userProfileDetailDto.FirstName,
                         LastName = userProfileDetailDto.LastName,
                         IsActive = true,
                         PhoneNumber = userProfileDetailDto.PhoneNumber,
                         PlayerType = userProfileDetailDto.PlayerType,
                         PlayerTypeId = userProfileDetailDto.PlayerTypeId,
                         SubscriptionExpiryDate = userProfileDetailDto.SubscriptionExpiryDate,
                         UserId = userProfileDetailDto.UserId,
                         WalletToken = userProfileDetailDto.WalletToken,
                         UserName = userProfileDetailDto.UserName
                    };
                    var User = await _userService.GetUserDetail(detailDto.UserId);

                    var userNewRole = await _userService.GetRolesById(detailDto.PlayerTypeId);

                    string fileName = !string.IsNullOrEmpty(User.Avtar) ? User.Avtar : "";
                    fileName = fileName.Replace("/images/profilePicture/", "");

                    var file = HttpContext.Request.Form;

                    if (file.Files.Count > 0)
                    {
                         #region Save profile pic

                         fileName = DateTime.Now.Ticks + "_" + file.Files[0].FileName;

                         string imagePath = _appSettings.ProfilePicPath;

                         var path = Path.Combine(imagePath, fileName);

                         using (var stream = new FileStream(path, FileMode.Create))
                         {
                              await file.Files[0].CopyToAsync(stream);
                         }

                         #endregion
                    }
                    // var city = await _cityService.GetCityById(detailDto.City);
                    var state = await _stateService.GetStateById(detailDto.State);
                    var country = await _countryService.GetCountryById(detailDto.Country);

                    detailDto.Avtar = fileName;
                    detailDto.CityName = !string.IsNullOrEmpty(detailDto.City) ? detailDto.City : "";
                    detailDto.StateName = state != null ? !string.IsNullOrEmpty(state.Name) ? state.Name : "" : "";
                    detailDto.CountryName = country != null ? !string.IsNullOrEmpty(country.Name) ? country.Name : "" : "";

                    await _userService.AddEditUserDetail(detailDto);

                    var user = await _userManager.FindByIdAsync(detailDto.UserId);


                    await _userManager.RemoveFromRoleAsync(user, User.PlayerType);
                    await _userManager.AddToRoleAsync(user, userNewRole);


                #region Update ShopifyCustomer 
                
                if (User.ShopifyCustomerId > 0)
                {
                    var customr_ = await _shopifyService.GetCustomerAsync(User.ShopifyCustomerId.ToString());

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
                            Phone = customr_.Phone??customr_.DefaultAddress?.Phone,

                        }},
                        AcceptsMarketing = customr_.AcceptsMarketing ?? false,
                        AdminGraphQLAPIId = customr_.AdminGraphQLAPIId,
                        VerifiedEmail = customr_.VerifiedEmail ?? false,
                        Email = detailDto.Email,
                        FirstName = User.FirstName,
                        LastName = User.LastName,
                        Phone = customr_.Phone ?? customr_.DefaultAddress?.Phone,

                    });
                }
                #endregion
                return Ok(new ApiResponse
                    {
                         Message = "Profile updated successfully.",
                         Success = true,
                         IpAddress = Helpers.GetIpAddress()
                    });
               }
               catch (Exception ex)
               {
                    return Ok(new ApiResponse
                    {
                         Message = ex.Message,
                         Success = false,
                         IpAddress = Helpers.GetIpAddress()
                    });
               }
          }

          /// <summary>
          /// Upgrade Subscription
          /// </summary>
          /// <param name="payPalResponseDto">PayPal Response</param>
          /// <param name="transactionLiteApiDto">Transaction Details</param>
          /// <returns></returns>
          [Route("upgradeplan")]
          [HttpPost]
          public async Task<OkObjectResult> PostUpgradeSubscription([FromForm] PayPalResponseLiteDto payPalResponseLiteDto,
                                                            [FromForm] TransactionLiteApiDto transactionLiteApiDto)
          {
               try
               {
                    #region Get Fees  
                    switch (transactionLiteApiDto.PlayerType.ToUpper())
                    {
                         case "NOVICE PLAYER":
                              if (transactionLiteApiDto.PaymentMode == "yearly")
                                   transactionLiteApiDto.ContestFee = 85;
                              if (transactionLiteApiDto.PaymentMode == "quarterly")
                                   transactionLiteApiDto.ContestFee = 50;
                              else
                                   transactionLiteApiDto.ContestFee = 25;
                              break;
                         case "INTERMEDIATE PLAYER":
                              if (transactionLiteApiDto.PaymentMode == "yearly")
                                   transactionLiteApiDto.ContestFee = 200;
                              if (transactionLiteApiDto.PaymentMode == "quarterly")
                                   transactionLiteApiDto.ContestFee = 105;
                              else
                                   transactionLiteApiDto.ContestFee = 55;
                              break;
                         case "PRO PLAYER":
                              if (transactionLiteApiDto.PaymentMode == "yearly")
                                   transactionLiteApiDto.ContestFee = 100;
                              if (transactionLiteApiDto.PaymentMode == "quarterly")
                                   transactionLiteApiDto.ContestFee = 70;
                              else
                                   transactionLiteApiDto.ContestFee = 20;
                              break;
                    }
                    #endregion
                    var transactionDto = Helper.TransactionUtility.MakePayment(payPalResponseLiteDto, transactionLiteApiDto, transactionLiteApiDto.UserId);
                    await _transactionService.InsertTransactionDetail(transactionDto, new TransactionLiteDto() { });

                    var user = await _userManager.FindByIdAsync(transactionLiteApiDto.UserId);
                    var userInfo = await _userService.GetUserDetail(user != null ? user.Id : "");

                    userInfo.SubscriptionExpiryDate = (transactionLiteApiDto.PaymentMode != null ?
                                                       transactionLiteApiDto.PaymentMode == "Yearly" ?
                                                       DateTime.Now.AddYears(1) : DateTime.Now.AddMonths(4)
                                                       : userInfo.SubscriptionExpiryDate);
                    if (userInfo.PlayerType != null)
                    {
                         await _userManager.RemoveFromRoleAsync(user, userInfo.PlayerType.ToUpper().Trim());
                         await _userManager.AddToRoleAsync(user, transactionLiteApiDto.PlayerType);

                         userInfo.PlayerType = transactionLiteApiDto.PlayerType;
                         userInfo.Avtar = userInfo.Avtar.Replace("/images/profilePicture/", "");
                         await _userService.AddEditUserDetail(userInfo);
                         await _signInManager.SignOutAsync();
                    }

                    return Ok(new ApiResponse
                    {
                         Message = "Account has been upgraded successfully. Please re-login to your account.",
                         IpAddress = Helpers.GetIpAddress(),
                         Success = true
                    });
               }
               catch (Exception ex)
               {
                    return Ok(new ApiResponse
                    {
                         Message = ex.Message,
                         IpAddress = Helpers.GetIpAddress(),
                         Success = false
                    });
               }
          }

          /// <summary>
          /// Token Purchase
          /// </summary>
          /// <param name="payPalResponseDto">PayPal Response</param>
          /// <param name="transactionLiteApiDto">Transaction Details</param>
          /// <returns></returns>
          [HttpPost]
          [Route("tokenpurchase")]
          public async Task<OkObjectResult> PostTokenPurchase([FromForm] PayPalResponseLiteDto payPalResponseLiteDto,
                                                            [FromForm] TransactionLiteApiDto transactionLiteApiDto)
          {

               try
               {
                    var transactionDto = Helper.TransactionUtility.MakePayment(payPalResponseLiteDto, transactionLiteApiDto, transactionLiteApiDto.UserId);
                    var user = await _userManager.FindByIdAsync(transactionLiteApiDto.UserId);
                    var userInfo = await _userService.GetUserDetail(user != null ? user.Id : "");

                    userInfo.WalletToken = (userInfo.WalletToken.HasValue ? userInfo.WalletToken.Value : 0) + Convert.ToInt32(transactionDto.TokenCredit);
                    userInfo.Avtar = userInfo.Avtar.Replace("/images/profilePicture/", "");
                    await _userService.AddEditUserDetail(userInfo);
                    await _transactionService.InsertTransactionDetail(transactionDto, new TransactionLiteDto() { });

                    return Ok(new ApiResponse
                    {
                         IpAddress = Helpers.GetIpAddress(),
                         Message = "Token added to your account successfully.",
                         Success = true
                    });
               }
               catch (Exception ex)
               {
                    return Ok(new ApiResponse
                    {
                         IpAddress = Helpers.GetIpAddress(),
                         Message = ex.Message,
                         Success = false
                    });
               }
          }

          [HttpPost]
          [Route("transactionHistory")]
          public async Task<OkObjectResult> TransactionHistory([FromForm]string userId)
          {
               var transactionHistory = await _transService.TransactionHistory(userId);
               return Ok(new ApiResponse
               {
                    Data = transactionHistory,
                    Success = true,
                    IpAddress = Helpers.GetIpAddress(),
                    Message = ""
               });
          }
     }
}