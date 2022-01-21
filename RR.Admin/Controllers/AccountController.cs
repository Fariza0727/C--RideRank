using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RR.AdminService;
using RR.Core;
using RR.Dto;
using RR.Service.Email;
using RR.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace RR.Admin.Controllers
{
    [ServiceFilter(typeof(AdminExceptionFilter))]
    public class AccountController : BaseController
    {
        #region Constructor

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterDto> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IUserService _userService;
        private readonly IPageDetailService _pageService;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterDto> logger,
            IEmailSender emailSender,
            IUserService userService,
            IPageDetailService pageService,
            IPageDetailService _pagesService,
            IOptions<AppSettings> appSettings) :
              base(appSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _userService = userService;
            _pageService = pageService;

        }

        #endregion

        #region Subadmin

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetUser()
        {
            var draw = Request.Form["draw"];
            var start = Request.Form["start"];
            var length = Request.Form["length"];

            var column = Convert.ToInt32(Request.Form["order[0][column]"].FirstOrDefault());
            var order = Request.Form["order[0][dir]"].FirstOrDefault();

            //Global search field
            var search = Request.Form["search[value]"].FirstOrDefault();

            int pageSize = !(string.IsNullOrEmpty(length)) ? Convert.ToInt32(length) : 0;
            int skip = (!(string.IsNullOrEmpty(start)) ? Convert.ToInt32(start) : 0) / pageSize;

            var userData = await Task.FromResult(_userManager.GetUsersInRoleAsync("Subadmin").Result);

            switch (column)
            {
                case 0:
                    if (order == "desc")
                        userData = userData.OrderByDescending(x => x.Email).ToList();
                    else
                        userData = userData.OrderBy(x => x.Email).ToList();
                    break;
            }

            var data = userData;
            int recordsTotal = userData.Count();

            return Json(new
            {
                draw,
                recordsFiltered = recordsTotal,
                recordsTotal,
                data
            });
        }


        public async Task<ActionResult> CreateSubadmin(string Id)
        {
            RegisterDto model = new RegisterDto();


            List<PageDto> items = await _pageService.GetAllPagesDetail();
            model.PageList = items;

            if (!string.IsNullOrEmpty(Id))
            {
                var userData = await _userManager.FindByIdAsync(Id);

                model.Id = userData.Id;
                model.UserName = userData.UserName;
                model.Email = userData.Email;
                model.Password = userData.PasswordHash;
                model.PageList = await _pageService.GetPermitedPages(userData.Id);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubadmin(RegisterDto registerDto)
        {
            
                try
                {
                    if (string.IsNullOrEmpty(registerDto.Id))
                    {
                        var user = new IdentityUser { UserName = registerDto.Email, Email = registerDto.Email };
                        var result = await _userManager.CreateAsync(user, registerDto.Password);

                        if (result.Errors.Count() > 0)
                        {
                            if (result.Errors.First().Code.ToString() == "DuplicateUserName")
                            {
                                return Content("Exist");
                            }
                            else
                            {
                                return Content("Failed");
                            }
                        }
                        var createRole = await _userManager.AddToRoleAsync(user, "Subadmin");
                        var userDetail = await _userManager.FindByNameAsync(registerDto.Email);
                        await _pageService.AddEditUserPagePermissionDetail(registerDto.PageList, userDetail.Id);
                        //await _signInManager.SignInAsync(user, isPersistent: false);                    

                        string emailBody = Utilities.GetEmailTemplateValue("SubAdmin/Body");
                        string emailSubject = Utilities.GetEmailTemplateValue("SubAdmin/Subject");
                        emailBody = emailBody.Replace("@@@title", "Rank Ride Account");
                        emailBody = emailBody.Replace("@@@UserName", userDetail.UserName);

                        await _emailSender.SendEmailAsync(userDetail.Email, emailSubject, emailBody
                           );

                        return Content("Inserted");
                    }
                    else
                    {
                        var userData = await _userManager.FindByIdAsync(registerDto.Id);
                        userData.UserName = registerDto.Email;
                        userData.Email = registerDto.Email;
                        await _pageService.AddEditUserPagePermissionDetail(registerDto.PageList, registerDto.Id);
                        var result = await _userManager.UpdateAsync(userData);
                        if (result.Errors.Count() > 0)
                        {
                            if (result.Errors.First().Code.ToString() == "DuplicateUserName")
                            {
                                return Content("Exist");
                            }
                            else
                            {
                                return Content("Failed");
                            }
                        }
                        return Content("Updated");
                    }
                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                }
           
            

        }

        [HttpDelete]
        public async Task<ActionResult> DeleteSubadmin(string Id)
        {
            try
            {
                var userData = await _userManager.FindByIdAsync(Id);
                _pageService.DeletePagePermission(userData.Id);
                var resutl = await _userManager.DeleteAsync(userData);
            }
            catch (Exception ex)
            {

                throw;
            }
            return Json("Deleted");
        }
        #endregion Subadmin End

        #region Login

        [Route("Login")]

        public IActionResult Login(string ReturnUrl = "")
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            LoginDto loginDto = new LoginDto();
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            loginDto.ReturnUrl = loginDto.ReturnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, loginDto.Rememberme, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return Content("<div class='alert alert-success alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Success!!!</strong> Please wait, redirecting to dashboard. If not redirected in 5 sec. <a id='dasboardURL' href='" + loginDto.ReturnUrl + "'>Tap here.</a></div>");
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { loginDto.ReturnUrl, RememberMe = loginDto.Rememberme });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Oops!!!</strong> Your account has been locked. Please contact to support.<a id='dasboardURL'></a></div>");
                }
                else
                {
                    return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Oops!!!</strong> Invalid Credentials. Please try again.<a id='dasboardURL'></a></div>");
                }
            }
            return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Oops!!!</strong> Something went wrong. Please provide valid information.<a id='dasboardURL'></a></div>");
        }

        [HttpPost]
        [Authorize(Policy = "PagePermission")]
        public async Task<IActionResult> ForgotPassword(ForgetPasswordDto passwordDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userService.GetUserDetail(passwordDto.UserId);

                    // if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                    if (user == null)
                    {
                        // Don't reveal that the user does not exist or is not confirmed
                        return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Oops!!!</strong> Either user not exists or email is not verified yet.</div>");
                    }

                    // For more information on how to enable account confirmation and password reset please 
                    // visit https://go.microsoft.com/fwlink/?LinkID=532713
                    var code = Guid.NewGuid();

                    #region Forget Password Request
                    // Add into Forget Password Request Table in RankRide DB
                    await _userService.AddForgetPasswordRequest(code, user.UserId);
                    #endregion

                    var callbackUrl1 = String.Format(_appSettings.MainSiteURL + "Account/ResetPassword?code={0}&email={1}", code.ToString(), user.Email);

                    var callbackUrl = callbackUrl1.Replace("Identity/", ""); // Request.Scheme + "://" + Request.Host.Value.ToString() + "/Account/ResetPassword?code=" + code + "&email=" + passwordDto.Email;

                    await _emailSender.SendEmailAsync(
                        user.Email,
                        "Reset Password",
                        $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    return Content("<div class='alert alert-success alert-dismissible' role='alert'><strong>Success!!!</strong> Password reset link has been sent on provided email address. Please check inbox. If not received please check in junk folder.</div>");
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
            return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Oops!!!</strong> Something went wrong. Please try again.</div>");
        }

        [HttpPost]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Login", "Account");
        }

        [Route("AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #endregion End Login

        #region Reset Password

        public IActionResult ResetPassword(string code, string email)
        {
            ResetPasswordDto passwordDto = new ResetPasswordDto();
            passwordDto.Code = code;
            passwordDto.Email = email;
            return View(passwordDto);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPassword)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(resetPassword.Email);
                resetPassword.Code = await _userManager.GeneratePasswordResetTokenAsync(user);
                if (user == null)
                {
                    return Content("notexits");
                }

                var result = await _userManager.ResetPasswordAsync(user, resetPassword.Code, resetPassword.Password);
                if (result.Succeeded)
                {
                    return Content("success");
                }

                return Content("failed");
            }
            return Content("error");
        }
        #endregion End Reset Password
    }
}