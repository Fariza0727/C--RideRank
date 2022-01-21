using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using RR.Core;
using RR.Dto;
using RR.Service;
using RR.Service.Email;
using RR.Service.User;
using RR.Web.Models;
using ShopifySharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace RR.Web.Controllers
{
    public class HomeController : BaseController
     {
          #region Constructor

          private readonly IEmailSender _emailSender;
          private readonly ICMSService _cmsService;
        private readonly IUserService _userService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly INewsService _newsService;
        private readonly IStoreShopifyService _shopifyService;
        private readonly INewsLetterService _newsLetterService;
          private readonly ICountryService _countryService;
          private readonly IStateService _stateService;
        private readonly IHostingEnvironment _Env;

        public HomeController(ICMSService cmsService,
                                INewsService newsService,
                                IUserService userService,
                                UserManager<IdentityUser> userManager,
                                IConfiguration config,
                                IStoreShopifyService shopifyService,
                                INewsLetterService newsLetterService,
                                IEmailSender emailSender,
                                IOptions<AppSettings> appSettings,
                                ICountryService countryService,
                                IStateService stateService,
                                IHostingEnvironment _env) :
                base(appSettings)
          {
               _cmsService = cmsService;
               _newsService = newsService;
            _shopifyService = shopifyService;
            _newsLetterService = newsLetterService;
               _emailSender = emailSender;
               _countryService = countryService;
               _stateService = stateService;
                _Env = _env;
            _userService = userService;
            _userManager = userManager;
        }

        #endregion

        /// <summary>
        /// Home Page
        /// </summary>
        /// <returns>Home Page</returns>
        public async Task<IActionResult> Index()
        {
            #region
            //string emailBody = Utilities.GetEmailTemplateValue("AccountCreated/Body");
            //string emailSubject = Utilities.GetEmailTemplateValue("AccountCreated/Subject");
            //emailBody = emailBody.Replace("@@@Title", "Reset Password Request");
            //emailBody = emailBody.Replace("@@@Email", "jim.deerman@rankridefantasy.com");
            //emailBody = emailBody.Replace("@@@Password", "asdf");
            //await _emailSender.SendEmailAsync("jim.deerman@rankridefantasy.com", emailSubject, emailBody);
            #endregion

            //string hookServer = "https://rankridefantasy.com/";
            #region UserMange
            //var customer = new List<SCustomer>();
            ////var customer = await _shopifyService.GetCustomersAsync();
            //var missinguser = new List<string>();

            //foreach (var customer_ in customer)
            //{
            //    var user = await _userManager.FindByEmailAsync(customer_.Email);
            //    var isSuccess = true;
            //    var isMailRequired = false;
            //    string password_ = ExtensionsHelper.GeneratePassword(8, 4);

            //    var email = customer_.Email;
            //    var name = string.Concat(customer_.FirstName, " ", customer_.LastName);
            //    if (string.IsNullOrEmpty(name))
            //        name = email.Split("@")[0];

            //    if (user == null)
            //    {
            //        user = new IdentityUser
            //        {
            //            UserName = customer_.Email,
            //            Email = customer_.Email,
            //            EmailConfirmed = true,
            //            PhoneNumber = customer_.Phone ?? customer_.DefaultAddress?.Phone
            //        };
            //        var result = await _userManager.CreateAsync(user, password_);
            //        isSuccess = result.Succeeded;
            //        isMailRequired = result.Succeeded;
            //    }


            //    if (isSuccess)
            //    {
            //        var userDetail = await _userService.GetUserDetail(user.Id);
            //        if (userDetail == null)
            //        {
            //            userDetail = new UserDetailDto();
            //            userDetail.CreatedDate = DateTime.Now;
            //            userDetail.FirstName = customer_.FirstName;
            //            userDetail.LastName = customer_.LastName;
            //            userDetail.Email = customer_.Email;
            //            userDetail.UserId = user.Id;
            //            userDetail.IsActive = true;
            //            userDetail.UserName = name;
            //            userDetail.ShopifyCustomerId = customer_.Id;
            //            userDetail.ShopifyMembership = customer_.Tags;
            //            userDetail.IsPaidMember = true;
            //            userDetail.PhoneNumber = customer_.Phone ?? customer_.DefaultAddress?.Phone;
            //            // Both fieds removed from apis request
            //            // we removed requried attr and replaced the null value with static/default
            //            userDetail.PlayerType = "PLAYFORFREE";
            //            //await _userService.AddEditUserDetail(userDetail, true);
            //            //await _userManager.AddToRoleAsync(user, userDetail.PlayerType);

            //            if (isMailRequired)
            //            {
            //                #region
            //                //string emailBody = Utilities.GetEmailTemplateValue("AccountCreated/Body");
            //                //string emailSubject = Utilities.GetEmailTemplateValue("AccountCreated/Subject");
            //                //emailBody = emailBody.Replace("@@@Title", "Account Registration");
            //                //emailBody = emailBody.Replace("@@@Email", user.Email);
            //                //emailBody = emailBody.Replace("@@@Password", password_);
            //                //await _emailSender.SendEmailAsync(user.Email, emailSubject, emailBody);
            //                #endregion
            //            }

            //        }
            //    }
            //}
            #endregion
            //await _shopifyService.AddEditWebhookAsync(new SWebhook
            //{
            //    Id = 798627004497,
            //    Address = $"{hookServer}customercreate",
            //    Topic = "customers/create",
            //    Format = "json",
            //});
            //await _shopifyService.AddEditWebhookAsync(new SWebhook
            //{
            //    Id = 798627070033,
            //    Address = $"{hookServer}customerupdate",
            //    Topic = "customers/update",
            //    Format = "json",
            //});
            //await _shopifyService.AddEditWebhookAsync(new SWebhook
            //{
            //    Id = 800024526929,
            //    Address = $"{hookServer}customerdelete",
            //    Topic = "customers/delete",
            //    Format = "json",
            //});
            var user = User.Identity.IsAuthenticated ? await _userManager.GetUserAsync(HttpContext.User) : null;

            var userDetail = await _userService.GetUserDetail(user != null ? user.Id : "");
            string referralCode = "";
            if (userDetail != null)
            {
                referralCode = userDetail.ReferralCode;
                if (string.IsNullOrEmpty(referralCode))
                {
                    var voucherHelper = new VoucherHelper(_appSettings);
                    referralCode = await voucherHelper.createReferalCodeAsync(user);
                    userDetail.ReferralCode = referralCode;
                    await _userService.AddEditUserDetail(userDetail);
                }
            }
            ViewBag.ReferralCode = referralCode;
            ViewBag.ReferralLink = Url.Action("FreePlayerRegister", "Account", new { referralCode = referralCode }, Request.Scheme);
            return View();
          }
        [Route("ctct/{code?}")]
        public async Task<IActionResult> CCTAuth(string code="")
        {
            ConstantContactHelper cctHelper = new ConstantContactHelper(_appSettings);
            var response = await cctHelper.GetAccessToken(code);
            var dto = JsonConvert.DeserializeObject<CCTokenInfo>(response);
            ViewBag.AccessToken = dto.access_token;
            ViewBag.RefreshToken = dto.refresh_token;
            
            return View();
        }
        /// <summary>
        /// Privacy
        /// </summary>
        /// <returns>Privacy View</returns>
        public IActionResult Privacy()
          {
               return View();
          }

          /// <summary>
          /// Error
          /// </summary>
          /// <returns>Error View</returns>
          [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
          public IActionResult Error()
          {
               return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
          }

          /// <summary>
          /// Page Action
          /// </summary>
          /// <param name="page">Page Name</param>
          /// <returns>Page Content on View</returns>
          [Route("page/{page}")]
          public async Task<IActionResult> PageAction(string page)
          {
               CmsDto model = new CmsDto();
               try
               {
                    model = await _cmsService.GetPageContentByPageName(page);
               }
               catch (Exception)
               {
                    return View("~/Views/Error/PageNotFound.cshtml");
               }
               ViewBag.Title = model.MetaTitle;
               ViewBag.Description = model.MetaDescription;
               ViewBag.Keywords = model.MetaKeyword;
               return View(model);
          }

          /// <summary>
          /// Page Action
          /// </summary>
          /// <param name="page">Page Name</param>
          /// <returns>Page Content on View</returns>
          [Route("pageApi/{page}")]
          public async Task<IActionResult> PageActionApi(string page)
          {
               CmsDto model = new CmsDto();
               model = await _cmsService.GetPageContentByPageName(page);
               ViewBag.Title = model.MetaTitle;
               ViewBag.Description = model.MetaDescription;
               ViewBag.Keywords = model.MetaKeyword;
               return View(model);
          }
         
          /// <summary>
          /// News
          /// </summary>
          /// <returns>News Detail On View</returns>
          [Route("News")]
          public async Task<IActionResult> News()
          {
            //ViewBag.NewsPicPath = _appSettings.MainSiteURL + "shared/" + _appSettings.NewsSharedPath; //_appSettings.AdminSiteURL + "assets/NewsImage/";
            ViewBag.NewsPicPath = _appSettings.MainSiteURL + _appSettings.NewsSharedPath;
            IEnumerable<NewsDto> model = await _newsService.GetNews();
               return View(model);
          }

          /// <summary>
          /// News Detail
          /// </summary>
          /// <param name="title">The Title</param>
          /// <param name="id">An Id</param>
          /// <returns>Specific News Detail View</returns>
          [Route("news/{title}/{id}")]
          public async Task<IActionResult> NewsDetail(string title, int id)
          {
               title = title.Replace("-", " ");
            ViewBag.NewsPicPath = _appSettings.MainSiteURL + _appSettings.NewsSharedPath;
            NewsDto dto = await _newsService.GetNewsDetail(title, id);

               return View(dto);
          }

          /// <summary>
          /// Get Top Recent News
          /// </summary>
          /// <returns>Top and Recent News List in Partial View</returns>
          public async Task<IActionResult> GetTopRecentNews()
          {
               var newsResult = await _newsService.GetTopRecentNews();
               IEnumerable<NewsDto> recent = newsResult.Item1;
               IEnumerable<NewsDto> top = newsResult.Item2;
               ViewBag.NewsPicPath = _appSettings.MainSiteURL + "shared/" + _appSettings.NewsSharedPath; //_appSettings.AdminSiteURL + "assets/NewsImage/";
               return PartialView(new Tuple<IEnumerable<NewsDto>, IEnumerable<NewsDto>>(recent, top));
          }

          /// <summary>
          /// Contact Us
          /// </summary>
          /// <returns>Contact Us View</returns>
          [Route("contact-us")]
          public IActionResult ContactUs()
          {
               ContactDto model = new ContactDto();
               return View(model);
          }
        
        [Route("about-us")]
        public IActionResult AboutUs()
        {
            return View();
        }
        [Route("rules-scoring")]
        public IActionResult RulesScoring()
        {
            return View();
        }

        /// <summary>
        /// This method is used to end mail of the user 
        /// who has sended a query from contact us page
        /// </summary>
        /// <param name="model">The ContactDto</param>
        /// <returns>Success Or Failure Alert Message</returns>
          [HttpPost]
          [Route("contact-us")]
          public async Task<IActionResult> ContactUs(ContactDto model)
          {
               
               try
               {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user == null)
                    {
                        return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Oops!!!</strong> Registered user can submitted query successfully. Please contact to support.</div>");
                    }
                    string toEmail = _appSettings.ToEmailAddress;
                    string emailBody = Utilities.GetEmailTemplateValue("Contactus/Body");
                    string emailSubject = Utilities.GetEmailTemplateValue("Contactus/Subject");
                    emailBody = emailBody.Replace("@@@title", "Rankride Enquiry");
                    emailBody = emailBody.Replace("@@@Name", model.Name);
                    emailBody = emailBody.Replace("@@@Phone", model.Phone);
                    emailBody = emailBody.Replace("@@@Email", model.Email);
                    emailBody = emailBody.Replace("@@@Message", model.Message);
                    await _emailSender.SendEmailAsync(toEmail, emailSubject, emailBody);
                    return Content("<div class='alert alert-success alert-dismissible'  style='background-color: #2ebc9b;' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Success!!!</strong> Thank you. Your query has been submitted successfully. We will respond back as soon as possible. </div>");
               }
               catch (Exception ex)
               {
                    return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Oops!!!</strong> Something went wrong. Please contact to support.</div>");
               }
          }

          [HttpPost]
          public async Task<IActionResult> Subscribe(SubscribeDto model)
          {
               try
               {
                    await _newsLetterService.AddNewsLetter(model);
               }
               catch (Exception ex)
               {
                    return Content(ex.Message);
               }

               return Content("success");
          }

          [Route("rr-store")]
          public IActionResult RRStore()
          {
               return View();
          }

          [Route("thank-you")]
          public IActionResult Thankyou()
          {
               ViewBag.Message = TempData["message"];
               ViewBag.Code = TempData["code"] != null ? TempData["code"] : "";
               return View();
          }

        [Route("home/last-event-standing-freeplay-component")]
        public IActionResult LastEventStandingFreePlayComponent(int count)
        {
            return ViewComponent("LastEventStandingFreePlayComponent", new { count });
        }
        
        [Route("home/playoff-standings-component")]
        public IActionResult PlayOffStandingsComponent()
        {
            return ViewComponent("PlayOffStandingsComponent");
        }
        
        [Route("home/home-page-upcoming-event-component")]
        public IActionResult HomePageUpcomingEventComponent()
        {
            return ViewComponent("HomePageUpcomingEventComponent");
        }
        [Route("home/home-page-top-referral-component")]
        public IActionResult HomePageTopReferralComponent()
        {
            return ViewComponent("HomePageTopReferralComponent");
        }
    }
}
