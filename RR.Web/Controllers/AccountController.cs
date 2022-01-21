using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RR.Core;
using RR.Dto;
using RR.Service;
using RR.Service.Email;
using RR.Service.User;
using RR.Web.Helpers;
using RR.Web.Models;
using System;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace RR.Web.Controllers
{
    public class AccountController : BaseController
    {
        #region ctor

        private readonly IHostingEnvironment _environment;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterDto> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IUserService _userService;
        public readonly SessionHelperService _sessionHelperService;
        private readonly IStateService _stateService;
        private readonly ICountryService _countryService;
        private readonly IStoreShopifyService _shopifyService;

        public AccountController(
              UserManager<IdentityUser> userManager,
              SignInManager<IdentityUser> signInManager,
              ILogger<RegisterDto> logger,
              IEmailSender emailSender,
              IUserService userService,
               IHostingEnvironment environment,
                IOptions<AppSettings> appSettings,
                SessionHelperService sessionHelperService,
                IStateService stateService,
                ICountryService countryService, 
                IStoreShopifyService shopifyService) :
            base(appSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _userService = userService;
            _environment = environment;
            _sessionHelperService = sessionHelperService;
            _stateService = stateService;
            _countryService = countryService;
            _shopifyService = shopifyService;
        }
        #endregion

        /// <summary>
        /// Login Page
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("/Login")]
        public async Task<IActionResult> Index(string ReturnUrl = "")
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("MyAccount", "Profile");
            }
            LoginDto loginDto = new LoginDto();
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

       
        [AllowAnonymous]
        [Route("/AccessDenined")]
        public async Task<IActionResult> AccessDenined(string ReturnUrl = "")
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        /// <summary>
        /// Register
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("Register")]
        public IActionResult Register()
        {
            //return View();
            return Redirect("https://rankridestore.com/pages/memberships");
        }

        /// <summary>
        /// Register 
        /// </summary>
        /// <param name="registerDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = registerDto.Email, Email = registerDto.Email };
                var result = await _userManager.CreateAsync(user, registerDto.Password);
                if (result.Succeeded)
                {
                    UserDetailDto userDetail = new UserDetailDto();
                    userDetail.CreatedDate = DateTime.Now;
                    userDetail.FirstName = registerDto.FullName;
                    userDetail.Email = registerDto.Email;
                    userDetail.UserId = user.Id;
                    userDetail.IsActive = true;
                    userDetail.UserName = registerDto.UserName;

                    // Shopify Customer
                    var sCustomer = await _shopifyService.AddEditCustomerAsync(new SCustomerDto
                    {
                        UserId = userDetail.UserId,
                        FirstName = userDetail.FirstName,
                        LastName = userDetail.LastName,
                        Email = userDetail.Email,
                        Password = registerDto.Password,
                        ConfirmPassword = registerDto.ConfirmPassword,
                        Phone = userDetail.Password,
                        SendWelComeEmail = true,
                    });
                    userDetail.ShopifyCustomerId = sCustomer.Id;
                    await _userService.AddEditUserDetail(userDetail);
                    //await _userManager.AddToRoleAsync(user, "User");
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code },
                        protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(registerDto.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                  

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("MyAccount");
                }
            }
            return View();
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            loginDto.ReturnUrl = loginDto.ReturnUrl ?? Url.Content("~/MyAccount");

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);

                if (user == null)
                    return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Oops!</strong> This users email does not exist. Please try with another email address</div>");
                if (!(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Oops!</strong> Oops! Your Email has not been verified. Please verify your email first in order to access your account.</div>");
                }
                int countLock = await _userManager.GetAccessFailedCountAsync(user);
                int count = 4 - countLock;
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, loginDto.Rememberme, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    await _userService.UpdateLogedInStatus(user.Id, true);
                    _logger.LogInformation("User logged in.");
                    return Content("<div class='alert alert-success alert-dismissible' role='alert' style='background-color:#59ca7c;'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Success!!!</strong> Please wait, redirecting to dashboard. If not redirected in 5 sec. <a class='text-white' id='dasboardURL' href='" + loginDto.ReturnUrl + "'>Tap here.</a></div>");
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { loginDto.ReturnUrl, RememberMe = loginDto.Rememberme });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return Content("<div class='alert alert-danger alert-dismissible' role='alert' ><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Oops!</strong> Your account has been locked. Please contact to support Or Try After Sometime!!<a id='dasboardURL'></a></div>");
                }
                else
                {
                    return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Oops!</strong> Invalid Credentials." + count + " Attempts Remaining!Please try again.<a id='dasboardURL'></a></div>");
                }
            }
            return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Oops!</strong> Something went wrong. Please provide valid information.<a id='dasboardURL'></a></div>");
        }

        /// <summary>
        /// Forget Password
        /// </summary>
        /// <param name="passwordDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgetPasswordDto passwordDto)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(passwordDto.Email);

                if (user == null)
                {
                    return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button>if a user account exists, you’ll will receive a password reset link.</div>");
                }

                if (!(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button>if a user account exists, you’ll will receive a password reset link.</div>");
                }

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var userDetail = await _userService.GetUserDetail(user.Id);
                var callbackUrl1 = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { code, email = passwordDto.Email, area = "Identity" },
                    protocol: Request.Scheme);

                var callbackUrl = callbackUrl1.Replace("Identity/", "").Replace("ForgotPassword", "ResetPassword"); // Request.Scheme + "://" + Request.Host.Value.ToString() + "/Account/ResetPassword?code=" + code + "&email=" + passwordDto.Email;

                // Send Mail For Forget Password
                MailRequest(passwordDto.Email, callbackUrl, "ForgetPassword", userDetail.UserName);

                return Content("<div class='alert alert-success alert-dismissible'  style='background-color: #2ebc9b;' role='alert'><strong>Success!!!</strong> Password reset link has been sent on provided email address. Please check inbox. If not received please check in junk folder.</div>");
            }
            return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button>if a user account exists, you’ll will receive a password reset link.</div>");
        }

        public async void MailRequest(string toEmail, string callBackUrl, string title, string userName)
        {
            try
            {
                string emailBody = Utilities.GetEmailTemplateValue("PasswordRequest/Body");
                string emailSubject = Utilities.GetEmailTemplateValue("PasswordRequest/Subject");
                emailBody = emailBody.Replace("@@@UserName", userName);
                emailBody = emailBody.Replace("@@@Url", HtmlEncoder.Default.Encode(callBackUrl));
                await _emailSender.SendEmailAsync(toEmail, emailSubject, emailBody);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Sign out
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SignOut()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _userService.UpdateLogedInStatus(userId, false);
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Reset Password
        /// </summary>
        /// <param name="code"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public IActionResult ResetPassword(string code, string email)
        {
            ResetPasswordDto passwordDto = new ResetPasswordDto();
            passwordDto.Code = code;
            passwordDto.Email = email;
            return View(passwordDto);
        }

        /// <summary>
        /// Reset Password
        /// </summary>
        /// <param name="resetPassword"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPassword)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(resetPassword.Email);
                if (user == null)
                {
                    return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Oops!!!</strong> User does not exists.</div>");
                }

                var result = await _userManager.ResetPasswordAsync(user, resetPassword.Code, resetPassword.Password);
                if (result.Succeeded)
                {
                    return Content("<div class='alert alert-success alert-dismissible' style='background-color: #2ebc9b;' role='alert'><strong>Success!!!</strong> Password has been reset successfully.</div>");
                }
                else
                {
                    var checkCodeFromForgetPassRequest = await _userService.GetForgetPasswordRequest(resetPassword.Code);
                    if (checkCodeFromForgetPassRequest != null)
                    {
                        var removePassword = await _userManager.RemovePasswordAsync(user);
                        var updatePassword = await _userManager.AddPasswordAsync(user, resetPassword.Password);
                        if (updatePassword.Succeeded)
                        {
                            #region Delete Forget Password Request
                            await _userService.DeleteForgetPasswordRequest(checkCodeFromForgetPassRequest.Id);
                            #endregion

                            return Content("<div class='alert alert-success alert-dismissible'  style='background-color: #2ebc9b;' role='alert'><strong>Success!!!</strong> Password has been reset successfully.</div>");
                        }
                    }
                }

                StringBuilder sb = new StringBuilder();

                foreach (var error in result.Errors)
                {
                    sb.Append("<li>" + error.Description + "</li>"); // ModelState.AddModelError(string.Empty, error.Description);
                }
                return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button>" + sb.ToString() + "</div>");
            }
            return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Oops!!!</strong> Something went wrong. Please try again.</div>");
        }

        /// <summary>
        /// Become A Player
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("become-a-player")]
        public IActionResult PlayerRegister()
        {
            ///summary
            /// Updated on 2/1/2020
            ///summary

            
            return RedirectToAction("FreePlayerRegister");

            //ViewBag.Token = _appSettings.DefaultToken;
            //return View();
            //return Redirect("https://rankridestore.com/pages/memberships");
        }

        [HttpPost]
        public async Task<JsonResult> EnableValidPlayer(string state, string dob)
        {
            var date = Convert.ToDateTime(dob);
            var states = await _stateService.RestrictPlayer(state, date);

            return Json(states);
        }

        /// <summary>
        /// Become A Player
        /// </summary>
        /// <param name="aPlayerDto"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("become-a-player")]
        public async Task<IActionResult> PlayerRegister(BecomeAPlayerDto aPlayerDto, IFormCollection form)
        {
            try
            {
                var state = await _stateService.GetStateByCode(aPlayerDto.StateName);
                string fileName = "";
                if (ModelState.IsValid)
                {
                    if (form.Files.Count > 0)
                    {
                        #region Save profile pic

                        fileName = DateTime.Now.Ticks + form.Files[0].FileName;

                        string imagePath = _appSettings.ProfilePicPath;

                        var path = Path.Combine(_environment.ContentRootPath, imagePath, fileName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await form.Files[0].CopyToAsync(stream);
                        }

                        #endregion
                    }
                    aPlayerDto.FileName = fileName;
                    _sessionHelperService.UserDetail = aPlayerDto;

                    #region Create Identity User

                    /// Updated on 2/1/2020

                    #region Register User Directly Without Payment

                    var user = new IdentityUser { UserName = aPlayerDto.Email, Email = aPlayerDto.Email, EmailConfirmed = true };
                    var result = await _userManager.CreateAsync(user, aPlayerDto.Password);
                    if (result.Succeeded)
                    {
                        UserDetailDto userDetail = new UserDetailDto();
                        userDetail.Address1 = aPlayerDto.Address1;
                        userDetail.Address2 = aPlayerDto.Address2;
                        userDetail.Address3 = aPlayerDto.Address3;
                        userDetail.Avtar = fileName;
                        userDetail.Email = aPlayerDto.Email;
                        userDetail.PhoneNumber = aPlayerDto.PhoneNumber;
                        userDetail.FirstName = aPlayerDto.FirstName;
                        userDetail.LastName = aPlayerDto.SurName;
                        userDetail.UserId = user.Id;
                        userDetail.IsActive = true;
                        userDetail.State = state != null ? state.StateId : 0;
                        userDetail.Country = state != null ? state.CountryId : 0;
                        userDetail.City = aPlayerDto.CityName;
                        userDetail.DOB = aPlayerDto.DateOfBirth;
                        userDetail.UserName = aPlayerDto.UserName;
                        userDetail.WalletToken = Convert.ToInt32(_appSettings.DefaultToken);
                        //userDetail.PlayerType = "PLAYFORFREE";
                        userDetail.PlayerType = "NOVICE PLAYER";  
                        //aPlayerDto.PlayerType
                        userDetail.ZipCode = aPlayerDto.PostCode;

                        // Shopify Customer
                        //var sCustomer = await _shopifyService.AddEditCustomerAsync(new SCustomerDto
                        //{
                        //    UserId = userDetail.UserId,
                        //    FirstName = userDetail.FirstName,
                        //    LastName = userDetail.LastName,
                        //    Email = userDetail.Email,
                        //    Password = aPlayerDto.Password,
                        //    ConfirmPassword = aPlayerDto.ConfirmPassword,
                        //    Phone = userDetail.Password,
                        //    SendWelComeEmail = true,
                        //});
                        //userDetail.ShopifyCustomerId = sCustomer.Id;

                        await _userService.AddEditUserDetail(userDetail);

                        await _userManager.AddToRoleAsync(user, "NOVICE PLAYER");
                        //await _userManager.AddToRoleAsync(user, "PLAYFORFREE");
                        await _userManager.AddToRoleAsync(user, aPlayerDto.PlayerType);
                        _logger.LogInformation("User created a new account with password.");


                        // TEMP COMMENT THIS PROCESS AS PER CLIENT REQUIRMENT
                        // AND ADDED EmailConfirmed = true DURING USER CREATE
                        #region Email Confirmation
                        //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        //var callbackUrl = Url.Page(
                        //    "/Account/ConfirmEmail",
                        //    pageHandler: null,
                        //    values: new { userId = user.Id, code, area = "Identity" },
                        //    protocol: Request.Scheme);

                        //callbackUrl = callbackUrl.Replace("Identity/", ""); // Request.Scheme + "://" + Request.Host.Value.ToString() + "/Account/ResetPassword?code=" + code + "&email=" + passwordDto.Email;

                        //var path = Path.Combine(_environment.WebRootPath, "Templates", "Template.xml");

                        //string emailBody = Utilities.GetEmailTemplateValue("AccountActivation/Body", path);
                        //string emailSubject = Utilities.GetEmailTemplateValue("AccountActivation/Subject", path);
                        //emailBody = emailBody.Replace("@@@UserEmail", aPlayerDto.UserName);
                        //emailBody = emailBody.Replace("@@@Url", callbackUrl); //HtmlEncoder.Default.Encode(callbackUrl)

                        //await _emailSender.SendEmailAsync(
                        //    aPlayerDto.Email,
                        //    emailSubject,
                        //    emailBody);
                        #endregion

                        //await _signInManager.SignInAsync(user, isPersistent: false);
                        //return RedirectToAction("MyAccount");

                        var loginResult = await _signInManager.PasswordSignInAsync(aPlayerDto.Email, aPlayerDto.Password, true, lockoutOnFailure: true);
                        if (loginResult.Succeeded)
                        {
                            await _userService.UpdateLogedInStatus(user.Id, true);
                            _logger.LogInformation("User logged in.");
                        }
                            return Content("<div class='alert alert-success alert-dismissible' style='background:#3fa95f' role='alert'><strong>Congrats!!!</strong> Your account has been created successfully.</div>");

                        //return Content("<div class='alert alert-success alert-dismissible' style='background:#3fa95f' role='alert'><strong>Congrats!!!</strong> Your account has been created successfully. An email has been sent to your registered email address in order to access your account. Please check inbox. If not received please check in junk folder.</div>");
                    }
                    else
                    {
                        return Content("<div class='alert alert-danger alert-dismissible' role='alert' style='background: #e52424;'><strong>Sorry!!!</strong>The status is:" + result.ToString() + "</div>");
                    }
                    #endregion

                    #endregion
                    //TempData["isUpgrade"] = false;
                    //return RedirectToAction("Subscription", "Transaction");//, new { isUpgrade = false }
                }
            }
            catch (Exception ex)
            {
                return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Oops!!!</strong> " + ex.Message + "</div>");
            }


            return View();
        }

        /// <summary>
        /// Check Email Exists
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> IsAlreadySigned(string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            return Json(user != null ? false : true);
        }

        /// <summary>
        /// Validate Email Account
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            string Message = "Something went wrong while confirming email.";
            if (userId == null || code == null)
            {
                Message = "Something went wrong while confirming email.";
            }
            else
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    Message = "Something went wrong while confirming email.";
                }
                else
                {
                    var result = await _userManager.ConfirmEmailAsync(user, code);
                    if (result.Succeeded)
                    {
                        WelcomeMailRequest(user.Email, user.UserName);
                        Message = "Thank you for confirming your email.";
                        var userDetail = await _userService.GetUserDetail(user.Id);
                        if (userDetail.IsNotifyEmail)
                        {
                            ConstantContactHelper cctHelper = new ConstantContactHelper(_appSettings);
                            await cctHelper.CreateOrUpdateEmail(user.Email);
                        }
                    }
                }
            }
            ViewBag.Message = Message;
            return View();
        }

        /// <summary>
        /// Reset Password
        /// </summary>
        /// <param name="resetPassword"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ChangePassword(UserDetailDto userPasswordDto)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(userPasswordDto.Email);
                if (user == null)
                {
                    return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Oops!!!</strong> User does not exists.</div>");
                }

                var removePassword = await _userManager.RemovePasswordAsync(user);
                var updatePassword = await _userManager.AddPasswordAsync(user, userPasswordDto.Password);
                if (updatePassword.Succeeded)
                {
                    await _signInManager.SignOutAsync();
                    return Content("<div class='alert alert-success alert-dismissible'style='background-color:#59ca7c;' role='alert'><strong>Success!!!</strong> Password has been changed successfully.</div>");
                }

                StringBuilder sb = new StringBuilder();

                foreach (var error in updatePassword.Errors)
                {
                    sb.Append("<li>" + error.Description + "</li>"); // ModelState.AddModelError(string.Empty, error.Description);
                }
                return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button>" + sb.ToString() + "</div>");
            }
            return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Oops!!!</strong> Something went wrong. Please try again.</div>");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> IsUserExists(string UserName)
        {
            var userExist = await _userService.UserExistence(UserName);
            if (userExist != null && (userExist.Email != null || userExist.UserName != null))
            {
                return Json(false);
            }
            return Json(true);
        }

        [HttpPost]
        [Route("sentconfirmemail")]
        public async Task<JsonResult> SentConfirmEmail(string email)
        {
            try
            {

                var user = await _userManager.FindByEmailAsync(email);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Action(
                   "ConfirmEmail", "Account",
                   new { userId = user.Id, code = code },
                   protocol: Request.Scheme);

                callbackUrl = callbackUrl.Replace("Identity/", "").Replace("localhost:44331", "rankridefantasy.com/");


                string emailBody = Utilities.GetEmailTemplateValue("AccountActivation/Body");
                string emailSubject = Utilities.GetEmailTemplateValue("AccountActivation/Subject");
                emailBody = emailBody.Replace("@@@title", "Email verification");
                emailBody = emailBody.Replace("@@@UserEmail", user.Email);
                emailBody = emailBody.Replace("@@@Url", HtmlEncoder.Default.Encode(callbackUrl));
                await _emailSender.SendEmailAsync(user.Email, emailSubject, emailBody);

                return Json(new { status = true, message = "Success" });
            }
            catch (System.Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

       
        [AllowAnonymous]
        [Route("freeplayer/{referralCode?}")]
        public IActionResult FreePlayerRegister(string referralCode="")
        {
            ViewBag.Token = _appSettings.DefaultToken;
            BecomeAPlayerDto model = new BecomeAPlayerDto();
            model.ReferralCode = referralCode;
            return View(model);
        }

        #region manually send activation
        [AllowAnonymous]
        [Route("send-activation")]
        public async Task<IActionResult> SendActivationEmail()
        {
            var userList = await _userService.GetNoActivatedEmailUsers();
            foreach (var user in userList)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var callbackUrl = Url.Page(
                   "/Account/ConfirmEmail",
                   pageHandler: null,
                   new { userId = user.Id, code = code, area = "Identity" },
                   protocol: Request.Scheme);

                callbackUrl = callbackUrl.Replace("Identity/", "").Replace("send-activation", "Account/ConfirmEmail").Replace("https://localhost:44372/", "https://rankridefantasy.com/");

                ActivationMailRequest(user.Email, user.UserName, callbackUrl);
            }
            return View();
        }
        #endregion

        [HttpPost]
        [Route("freeplayer/{referralCode?}")]
        public async Task<IActionResult> FreePlayerRegister(BecomeAPlayerDto aPlayerDto, IFormCollection form)
        {
            try
            {
                string fileName = "";
                if (ModelState.IsValid)
                {
                    if (form.Files.Count > 0)
                    {
                        #region Save profile pic

                        fileName = DateTime.Now.Ticks + form.Files[0].FileName;

                        string imagePath = _appSettings.ProfilePicPath;

                        var path = Path.Combine(_environment.ContentRootPath, imagePath, fileName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await form.Files[0].CopyToAsync(stream);
                        }

                        #endregion
                    }
                    aPlayerDto.FileName = fileName;
                    _sessionHelperService.UserDetail = aPlayerDto;

                    #region Create Identity User

                    /// Updated on 2/1/2020

                    #region Register User Directly Without Payment

                    var user = new IdentityUser { UserName = aPlayerDto.Email, Email = aPlayerDto.Email, EmailConfirmed = false };
                    var result = await _userManager.CreateAsync(user, aPlayerDto.Password);
                    if (result.Succeeded)
                    {
                        var voucherHelper = new VoucherHelper(_appSettings);
                        if (!string.IsNullOrEmpty(aPlayerDto.ReferralCode))
                        {
                            var realRefCode = await _userService.GetRealReferralCode(aPlayerDto.ReferralCode);
                            if (!string.IsNullOrEmpty(realRefCode))
                            {
                                aPlayerDto.ReferralCode = realRefCode;
                            }
                            await voucherHelper.signupReferralCodeAsync(user, aPlayerDto.ReferralCode);
                            await _userService.UpdateReferredCustomers(aPlayerDto.ReferralCode);
                        }
                        // create referral code
                        
                        var ReferralCode = await voucherHelper.createReferalCodeAsync(user);

                        UserDetailDto userDetail = new UserDetailDto();
                        userDetail.ReferralCode = ReferralCode;

                        userDetail.Address1 = aPlayerDto.Address1;
                        //userDetail.Address2 = aPlayerDto.Address2;
                        userDetail.Address3 = aPlayerDto.Address3;
                        userDetail.Avtar = fileName;
                        userDetail.Email = aPlayerDto.Email;
                        userDetail.PhoneNumber = aPlayerDto.PhoneNumber;
                        userDetail.FirstName = aPlayerDto.FirstName;
                        userDetail.LastName = aPlayerDto.SurName;
                        userDetail.UserId = user.Id;
                        userDetail.IsActive = true;
                        userDetail.City = aPlayerDto.CityName;
                        userDetail.DOB = aPlayerDto.DateOfBirth;
                        userDetail.UserName = aPlayerDto.UserName;
                        userDetail.TeamName = aPlayerDto.TeamName;
                        userDetail.IsNotifyEmail = aPlayerDto.IsEmailNotify;
                        userDetail.IsNotifySms = aPlayerDto.IsSmsNotify;
                        //userDetail.CityName = aPlayerDto.CityName;
                        //userDetail.StateName = aPlayerDto.StateName;
                        userDetail.ZipCode = aPlayerDto.PostCode;
                        //userDetail.OptPhoneno = aPlayerDto.OptPhoneNo;
                        userDetail.WalletToken = Convert.ToInt32(_appSettings.DefaultToken);
                        //userDetail.PlayerType = "PLAYFORFREE";
                        userDetail.PlayerType = "NOVICE PLAYER";
                        await _userService.AddEditUserDetail(userDetail);

                        await _userManager.AddToRoleAsync(user, "NOVICE PLAYER");
                        //await _userManager.AddToRoleAsync(user, "PLAYFORFREE");
                        await _userManager.AddToRoleAsync(user, aPlayerDto.PlayerType);
                        _logger.LogInformation("User created a new account with password.");
                        // send activation mail
                        
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                        var callbackUrl = Url.Page(
                           "/Account/ConfirmEmail",
                           pageHandler: null,
                           new { userId = user.Id, code = code, area = "Identity" },
                           protocol: Request.Scheme);

                        callbackUrl = callbackUrl.Replace("Identity/", "").Replace("freeplayer", "Account/ConfirmEmail");

                        ActivationMailRequest(aPlayerDto.Email, userDetail.UserName, callbackUrl);
                        
                        /*var loginResult = await _signInManager.PasswordSignInAsync(aPlayerDto.Email, aPlayerDto.Password, true, lockoutOnFailure: true);
                        if (loginResult.Succeeded)
                        {
                            await _userService.UpdateLogedInStatus(user.Id, true);
                            _logger.LogInformation("User logged in.");
                        }*/
                        return Content("<div class='alert alert-success alert-dismissible' style='background:#3fa95f' role='alert'>Thank you for registering. You will receive an activation email shortly.</div>");

                    }
                    else
                    {
                        return Content("<div class='alert alert-danger alert-dismissible' role='alert' style='background: #e52424;'><strong>Sorry!!!</strong>The status is:" + result.ToString() + "</div>");
                    }
                    #endregion

                    #endregion
                    //TempData["isUpgrade"] = false;
                    //return RedirectToAction("Subscription", "Transaction");//, new { isUpgrade = false }
                }
            }
            catch (Exception ex)
            {
                return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Oops!!!</strong> " + ex.Message + "</div>");
            }


            return View();
        }
        public async void WelcomeMailRequest(string toEmail,  string userName)
        {
            try
            {
                string emailBody = Utilities.GetEmailTemplateValue("FreeAccountCreated/Body");
                string emailSubject = Utilities.GetEmailTemplateValue("FreeAccountCreated/Subject");
                emailBody = emailBody.Replace("@@@UserName", userName);
                
                await _emailSender.SendEmailAsync(toEmail, emailSubject, emailBody);
            }
            catch (Exception ex)
            {
            }
        }
        
        public async void ActivationMailRequest(string toEmail, string userName, string callbackUrl)
        {
            try{

                string emailBody = Utilities.GetEmailTemplateValue("AccountActivation/Body");
                string emailSubject = Utilities.GetEmailTemplateValue("AccountActivation/Subject");
                emailBody = emailBody.Replace("@@@UserName", userName);
                emailBody = emailBody.Replace("@@@Url", HtmlEncoder.Default.Encode(callbackUrl));
                await _emailSender.SendEmailAsync(toEmail, emailSubject, emailBody);
                
            }
            catch (System.Exception ex)
            {
                
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> SendReferralEmail(ReferralDto model)
        {
            try
            {
                string toEmail = model.Email;
                string friendName = model.FriendName;
                string referralCode = "";
                string userName = "";
                var user = User.Identity.IsAuthenticated ? await _userManager.GetUserAsync(HttpContext.User) : null;
                var userDetail = await _userService.GetUserDetail(user != null ? user.Id : "");
                if (userDetail != null)
                {
                    if (string.IsNullOrEmpty(userDetail.ReferralCode))
                    {
                        var voucherHelper = new VoucherHelper(_appSettings);
                        referralCode = await voucherHelper.createReferalCodeAsync(user);
                        userDetail.ReferralCode = referralCode;
                        await _userService.AddEditUserDetail(userDetail);
                        
                    }
                    userName = userDetail.FirstName + " " + userDetail.LastName;
                    referralCode = userDetail.ReferralCode;
                    
                }
                string signUpUrl = Url.Action("FreePlayerRegister", "Account", new { referralCode = referralCode }, Request.Scheme);
                string emailBody = Utilities.GetEmailTemplateValue("InviteEmailSent/Body");
                string emailSubject = Utilities.GetEmailTemplateValue("InviteEmailSent/Subject");
                emailSubject = emailSubject.Replace("@@@FriendName", userName);
                emailBody = emailBody.Replace("@@@UserName", userName);
                emailBody = emailBody.Replace("@@@FriendName", friendName);
                emailBody = emailBody.Replace("@@@SignUpUrl", signUpUrl);

                await _emailSender.SendEmailAsync(toEmail, emailSubject, emailBody);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }

            return Content("success");
        }
    }
}