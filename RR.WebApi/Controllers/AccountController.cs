using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RR.Core;
using RR.Dto;
using RR.Service;
using RR.Service.Email;
using RR.Service.User;
using RR.WebApi.Models;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RR.WebApi.Controllers
{
     [Route("api/[controller]")]
     [ApiController]
     public class AccountController : BaseController
     {

          #region
          private readonly IHostingEnvironment _environment;
          private readonly UserManager<IdentityUser> _userManager;
          private readonly IEmailSender _emailSender;
          private readonly IUserService _userService;
          private readonly ITransactionService _transactionService;
          private readonly SignInManager<IdentityUser> _signInManager;
          private readonly IPasswordRequestService _passwordRequestService;
        private readonly IStoreShopifyService _shopifyService;

        public AccountController(
               IHostingEnvironment environment,
                UserManager<IdentityUser> userManager,
                IEmailSender emailSender,
                IUserService userService,
                ITransactionService transactionService,
                SignInManager<IdentityUser> signInManager,
                IPasswordRequestService passwordRequestService,
                IOptions<AppSettings> appSettings,
                IStoreShopifyService storeShopifyService)
               : base(appSettings)
          {
               _environment = environment;
               _userManager = userManager;
               _emailSender = emailSender;
               _userService = userService;
               _transactionService = transactionService;
               _signInManager = signInManager;
               _passwordRequestService = passwordRequestService;
            _shopifyService = storeShopifyService;
        }
          #endregion 

          [Route("userlogin")]
          [HttpPost]
          public async Task<OkObjectResult> PostUserLogin([FromForm] LoginDto loginDto)
          {
               if (ModelState.IsValid)
               {
                    loginDto.ReturnUrl = loginDto.ReturnUrl ?? Url.Content("~/MyAccount");
                    var user = await _userManager.FindByEmailAsync(loginDto.Email);
                    if (user == null)
                    {
                         return Ok(new ApiResponse
                         {
                              Message = "Invalid email id or password",
                              RedirectPath = loginDto.ReturnUrl,
                              Success = false,
                              Data = 0,
                              IpAddress = Helpers.GetIpAddress(),
                              TotalItems = 0
                         });
                    }
                    var userInfo = await _userService.GetUserDetail(user != null ? user.Id : "");
                    

                    //if (!(await _userManager.IsEmailConfirmedAsync(user)))
                    //{
                    //     return Ok(new ApiResponse
                    //     {
                    //          Message = "Your Email is not verified. Please verify your email first!! In order to access your account.",
                    //          Success = false,
                    //          Data = 0,
                    //          RedirectPath = loginDto.ReturnUrl,
                    //          IpAddress = Helpers.GetIpAddress(),
                    //          TotalItems = 0
                    //     });
                    //}

                    if (userInfo != null)
                    {
                         userInfo.Avtar = _appSettings.MainSiteURL + userInfo.Avtar;
                         var result = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, loginDto.Rememberme, lockoutOnFailure: true);
                         if (result.Succeeded)
                         {
                              if (userInfo.IsActive)
                              {
                                   userInfo.Avtar = _appSettings.MainSiteURL + userInfo.Avtar;
                                   return Ok(new ApiResponse
                                   {
                                        Message = "User is logged in",
                                        Success = true,
                                        Data = userInfo,
                                        IpAddress = Helpers.GetIpAddress(),
                                        RedirectPath = loginDto.ReturnUrl,
                                        TotalItems = 0
                                   });
                              }
                              else
                              {
                                   return Ok(new ApiResponse
                                   {
                                        Data = userInfo,
                                        Message = "User is not active you need to do transaction for activation user.",
                                        Success = false,
                                        IpAddress = Helpers.GetIpAddress(),
                                        RedirectPath = loginDto.ReturnUrl,
                                        TotalItems = 0
                                   });
                              }
                         }
                         if (result.RequiresTwoFactor)
                         {
                              return Ok(new ApiResponse
                              {
                                   RedirectPath = "./LoginWith2fa",
                                   Success = true,
                                   Data = userInfo,
                                   IpAddress = Helpers.GetIpAddress(),
                                   Message = ""
                              });
                         }
                         if (result.IsLockedOut)
                         {
                              return Ok(new ApiResponse
                              {
                                   Message = "Your account has been locked. Please contact to support Or Try After Sometime!!",
                                   Success = false,
                                   IpAddress = Helpers.GetIpAddress(),
                              });
                         }
                         return Ok(new ApiResponse
                         {
                              Success = false,
                              Message = "Invalid Credentials.",
                              IpAddress = Helpers.GetIpAddress(),
                         });
                    }
                    else
                    {
                         return Ok(new ApiResponse
                         {
                              Success = false,
                              Message = "Something Wen Wrong Please try again later.",
                              IpAddress = Helpers.GetIpAddress(),
                         });
                    }
               }
               else
               {
                    return Ok(new ApiResponse
                    {
                         Success = false,
                         Message = "Validation Error Occurs.",
                         IpAddress = Helpers.GetIpAddress()
                    });
               }
          }

          [Route("userregistration")]
          [HttpPost]
          public async Task<OkObjectResult> PostUserRegistration([FromForm] RegisterUserDto registerUserDto)
          {
               try
               {
                    string fileName = "";


                    #region Register block
                    var userInfo = new UserDetailDto();
                    var user = new IdentityUser
                    {
                         UserName = registerUserDto.Email,
                         Email = registerUserDto.Email
                    };
                    var check = await IsValid(registerUserDto);

                    if (string.IsNullOrEmpty(check))
                    {
                         var form = HttpContext.Request.Form;
                         if (form.Files.Count > 0)
                         {
                              #region Save profile pic

                              fileName = DateTime.Now.Ticks + form.Files[0].FileName;
                              
                              string imagePath = _appSettings.ProfilePicPath;

                              var path = Path.Combine(imagePath, fileName);

                              using (var stream = new FileStream(path, FileMode.Create))
                              {
                                   await form.Files[0].CopyToAsync(stream);
                              }

                              #endregion
                         }
                         registerUserDto.FileName = fileName;
                         var result = await _userManager.CreateAsync(user, registerUserDto.Password);
                         if (result.Succeeded)
                         {
                            UserDetailDto userDetail = new UserDetailDto();
                            userDetail.Address1 = registerUserDto.Address1;
                            userDetail.Address2 = registerUserDto.Address2;
                            userDetail.Address3 = registerUserDto.Address3;
                            userDetail.Avtar = registerUserDto.FileName;
                            userDetail.Email = registerUserDto.Email;
                            userDetail.PhoneNumber = registerUserDto.PhoneNumber;
                            userDetail.FirstName = registerUserDto.FullName;
                            userDetail.DOB = DateTime.ParseExact(registerUserDto.DateOfBirth, "dd/MM/yyyy", null);
                            userDetail.UserName = registerUserDto.UserName;
                            userDetail.UserId = user.Id;

                        // Shopify Customer
                            //var sCustomer = await _shopifyService.AddEditCustomerAsync(new SCustomerDto
                            //{
                            //    UserId = userDetail.UserId,
                            //    FirstName = userDetail.FirstName,
                            //    LastName = userDetail.LastName,
                            //    Email = userDetail.Email,
                            //    Password = registerUserDto.Password,
                            //    ConfirmPassword = registerUserDto.ConfirmPassword,
                            //    Phone = userDetail.Password,
                            //    SendWelComeEmail = true,
                            //});

                            //userDetail.ShopifyCustomerId = sCustomer.Id;

                            // Both fieds removed from apis request
                            // we removed requried attr and replaced the null value with static/default
                            registerUserDto.PlayerType = "NOVICE PLAYER";
                            /************/

                            userDetail.PlayerType = registerUserDto.PlayerType;
                            userDetail.IsActive = false;

                            await _userService.AddEditUserDetail(userDetail);
                            await _userManager.AddToRoleAsync(user, registerUserDto.PlayerType);

                            return Ok(new ApiResponse
                            {
                                Message = "User is registered successfully!!",
                                Success = true,
                                Data = user.Id,
                                RedirectPath = "",
                                IpAddress = Helpers.GetIpAddress()
                            });
                         }
                         else
                         {
                              var errorMessage = string.Empty;
                              foreach (var item in result.Errors)
                              {
                                   errorMessage += "," + item.Code;
                              }
                              return Ok(new ApiResponse
                              {
                                   Message = errorMessage.ToString().Remove(0, 1),
                                   Success = false,
                                   Data = 0,
                                   RedirectPath = "",
                                   IpAddress = Helpers.GetIpAddress()
                              });
                         }
                    }
                    else
                    {
                         return Ok(new ApiResponse
                         {
                              Message = check,
                              Success = false,
                              RedirectPath = "",
                              IpAddress = Helpers.GetIpAddress()
                         });
                    }
                    #endregion   
               }
               catch (Exception ex)
               {
                    return Ok(new ApiResponse
                    {
                         Message = ex.Message,
                         Success = false,
                         Data = 0,
                         RedirectPath = "",
                         IpAddress = Helpers.GetIpAddress()
                    });
               }
          }

          [HttpGet]
          public async Task<string> IsValid(RegisterUserDto registerUserDto)
          {
               //bool response = true;
               string msg = "";

               //Check email registered
               var userNameExists = await _userService.UserExistence(registerUserDto.UserName);
               if (!string.IsNullOrEmpty(userNameExists.UserName))
               {
                    msg = msg + "User with this UserName already exists.";
                    //response = false;
               }
               return msg;
          }

          [Route("getsubscriptionfees")]
          [HttpPost]
          public async Task<OkObjectResult> GetSubscriptionFees([FromForm] string playerType)
          {
               try
               {
                    #region Get Fees
                    SubscriptionFeesDto subscriptionFees = new SubscriptionFeesDto();
                    switch (playerType.ToUpper())
                    {
                         case "NOVICE PLAYER":

                              subscriptionFees.YearlyFees = 85;
                              subscriptionFees.QuarterlyFees = 50;
                              subscriptionFees.MonthlyFees = 25;
                              break;
                         case "INTERMEDIATE PLAYER":
                              subscriptionFees.YearlyFees = 200;
                              subscriptionFees.QuarterlyFees = 105;
                              subscriptionFees.MonthlyFees = 55;
                              break;
                         case "PRO PLAYER":
                              subscriptionFees.YearlyFees = 100;
                              subscriptionFees.QuarterlyFees = 70;
                              subscriptionFees.MonthlyFees = 20;
                              break;
                    }
                    return await Task.FromResult(Ok(new ApiResponse
                    {
                         IpAddress = Helpers.GetIpAddress(),
                         Data = subscriptionFees,
                         Success = true,
                         Message = ""
                    }));
                    #endregion

               }
               catch (Exception ex)
               {
                    return await Task.FromResult(Ok(new ApiResponse
                    {
                         IpAddress = Helpers.GetIpAddress(),
                         Message = ex.Message,
                         Success = false
                    }));
               }
          }

          [HttpPost]
          [Route("getcurrentusersubscription")]
          public async Task<OkObjectResult> GetCurrentUserSubscription([FromForm] string userId = "")
          {
               try
               {
                    var userSubscription = await _userService.GetCurrentUserSubscription(userId);
                    return Ok(new ApiResponse
                    {
                         Data = userSubscription,
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
                         Success = true
                    });
               }

          }

          [Route("registercheckout")]
          [HttpPost]
          public async Task<OkObjectResult> PostRegistrationCheckout([FromForm] string userId, [FromForm] PayPalResponseLiteDto payPalResponseLiteDto, [FromForm] TransactionLiteApiDto transactionLiteApiDto)
          {
               var user = await _userManager.FindByIdAsync(userId);
               var userInfo = await _userService.GetUserDetail(user != null ? user.Id : "");
               userInfo.Avtar = _appSettings.MainSiteURL + userInfo.Avtar;
               if (userInfo != null)
               {
                    var transactionDto = Helper.TransactionUtility.MakePayment(payPalResponseLiteDto, transactionLiteApiDto, user.Id);

                    await _transactionService.InsertTransactionDetail(transactionDto, new TransactionLiteDto() { });

                    userInfo.IsActive = true;
                    userInfo.SubscriptionExpiryDate = (transactionLiteApiDto.PaymentMode != null ?
                 transactionLiteApiDto.PaymentMode == "Yearly" ?
                DateTime.Now.AddYears(1) : DateTime.Now.AddMonths(4)
                : userInfo.SubscriptionExpiryDate);

                    userInfo.PaymentMode = transactionLiteApiDto.PaymentMode;

                    await _userService.AddEditUserDetail(userInfo);

                    return Ok(new ApiResponse
                    {
                         Message = "Payment Successfully Done for user registration",
                         Success = true,
                         Data = user.Id,
                         RedirectPath = Url.Content("~/MyAccount"),
                         IpAddress = Helpers.GetIpAddress()
                    });
               }
               else
               {
                    return Ok(new ApiResponse
                    {
                         Message = "This userId is not exist",
                         Success = false,
                         Data = user.Id,
                         RedirectPath = Url.Content("~/MyAccount"),
                         IpAddress = Helpers.GetIpAddress()
                    });
               }
          }

          [Route("forgetpassword")]
          [HttpPost]
          public async Task<OkObjectResult> PostForgotPassword([FromForm] ForgetPasswordDto forgetPasswordDto)
          {
               if (ModelState.IsValid)
               {
                    var user = await _userManager.FindByEmailAsync(forgetPasswordDto.Email);

                    if (user == null)
                    {
                         return Ok(new ApiResponse
                         {
                              Message = "your Email Address s not registered with us",
                              RedirectPath = "",
                              Success = false,
                              Data = 0,
                              IpAddress = Helpers.GetIpAddress(),
                              TotalItems = 0
                         });
                    }

                    //if (!(await _userManager.IsEmailConfirmedAsync(user)))
                    //{
                    //     // Don't reveal that the user does not exist or is not confirmed
                    //     return Ok(new ApiResponse
                    //     {
                    //          Message = "Your Email is not verified yet.",
                    //          Success = false,
                    //          Data = 0,
                    //          RedirectPath = "",
                    //          IpAddress = Helpers.GetIpAddress(),
                    //          TotalItems = 0
                    //     });
                    //}


                    var userDetail = await _userService.UserExistence(user.Email);
                    const string alphanumericCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                                                         "abcdefghijklmnopqrstuvwxyz" +
                                                         "0123456789";
                    string code = Helpers.GetRandomString(5, alphanumericCharacters);

                    // Send Mail For Forget Password     
                    string emailBody = Utilities.GetEmailTemplateValue("PasswordRequest/Body");
                    string emailSubject = Utilities.GetEmailTemplateValue("PasswordRequest/Subject");
                    emailBody = emailBody.Replace("@@@title", "ForgetPassword");
                    emailBody = emailBody.Replace("@@@Email", userDetail.UserName);
                    emailBody = emailBody.Replace("@@@Url", code);
                    await _emailSender.SendEmailAsync(forgetPasswordDto.Email, emailSubject, emailBody);

                    var passwordRequest = new PasswordRequestDto
                    {
                         Code = code,
                         Email = forgetPasswordDto.Email,
                         IsUsed = false
                    };

                    await _passwordRequestService.AddEditPasswordRequest(passwordRequest);
                    return Ok(new ApiResponse
                    {
                         Message = "Password reset code has been sent on provided email address." +
                                   "Please check inbox.If not received please check in junk folder.",
                         Success = true,
                         Data = 0,
                         RedirectPath = "",
                         IpAddress = Helpers.GetIpAddress(),
                         TotalItems = 0
                    });


               }
               return Ok(new ApiResponse
               {
                    Message = "Something went wrong. Please try again.",
                    Success = true,
                    Data = 0,
                    RedirectPath = "",
                    IpAddress = Helpers.GetIpAddress(),
                    TotalItems = 0
               });
          }

          [Route("resetPassword")]
          [HttpPost]
          public async Task<OkObjectResult> PostResetPassword([FromForm] ResetPasswordApiDto resetPasswordApiDto)
          {
               var passwordRequestExistence = await _passwordRequestService.GetPasswordRequest(resetPasswordApiDto.Code);
               if (passwordRequestExistence != null && !passwordRequestExistence.IsUsed)
               {
                    var user = await _userManager.FindByEmailAsync(passwordRequestExistence.Email);
                    if (user == null)
                    {
                         return Ok(new ApiResponse
                         {
                              Message = "User Doesn't exist",
                              Data = 0,
                              RedirectPath = "",
                              Success = false,
                              IpAddress = Helpers.GetIpAddress()
                         });
                    }
                    await _passwordRequestService.AddEditPasswordRequest(new PasswordRequestDto
                    {
                         Code = resetPasswordApiDto.Code,
                         IsUsed = true,
                         Email = passwordRequestExistence.Email
                    });

                    resetPasswordApiDto.Code = await _userManager.GeneratePasswordResetTokenAsync(user);

                    var result = await _userManager.ResetPasswordAsync(user, resetPasswordApiDto.Code, resetPasswordApiDto.Password);
                    if (result.Succeeded)
                    {
                         return Ok(new ApiResponse
                         {
                              Message = "Password has been reset successfully.",
                              Data = user,
                              RedirectPath = "",
                              Success = true,
                              IpAddress = Helpers.GetIpAddress()
                         });
                    }
               }
               return Ok(new ApiResponse
               {
                    Message = "Password has not been reset.",
                    Data = 0,
                    RedirectPath = "",
                    Success = true,
                    IpAddress = Helpers.GetIpAddress()
               });
          }

          /// <summary>
          /// Reset Password
          /// </summary>
          /// <param name="resetPassword"></param>
          /// <returns></returns>
          [Route("changePassword")]
          [HttpPost]
          public async Task<IActionResult> ChangePassword(UserDetailDto userPasswordDto)
          {
               if (ModelState.IsValid)
               {
                    var user = await _userManager.FindByEmailAsync(userPasswordDto.Email);
                    if (user == null)
                    {
                         return Ok(new ApiResponse
                         {
                              Message = "User Doesn't exist",
                              Data = 0,
                              RedirectPath = "",
                              Success = false,
                              IpAddress = Helpers.GetIpAddress()
                         });
                    }

                    var removePassword = await _userManager.RemovePasswordAsync(user);
                    var updatePassword = await _userManager.AddPasswordAsync(user, userPasswordDto.Password);
                    if (updatePassword.Succeeded)
                    {
                         await _signInManager.SignOutAsync();
                         return Ok(new ApiResponse
                         {
                              Message = "Password has been changed successfully.",
                              Data = 0,
                              RedirectPath = "",
                              Success = false,
                              IpAddress = Helpers.GetIpAddress()
                         });
                    }

                    StringBuilder sb = new StringBuilder();

                    foreach (var error in updatePassword.Errors)
                    {
                         sb.Append("<li>" + error.Description + "</li>"); // ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return Ok(new ApiResponse
                    {
                         Message = sb.ToString(),
                         Data = 0,
                         RedirectPath = "",
                         Success = false,
                         IpAddress = Helpers.GetIpAddress()
                    });
               }
               return Ok(new ApiResponse
               {
                    Message = "Something went wrong. Please try again.",
                    Data = 0,
                    RedirectPath = "",
                    Success = false,
                    IpAddress = Helpers.GetIpAddress()
               });
          }

          [Route("signout")]
          [HttpPost]
          public async Task<OkObjectResult> SignOut()
          {
               await _signInManager.SignOutAsync();
               return Ok(new ApiResponse
               {
                    Message = "User logged out.",
                    Success = true
               });
          }

          /// <summary>
          /// This method is used to end mail of the user 
          /// who has sended a query from contact us page
          /// </summary>
          /// <param name="model">The ContactDto</param>
          /// <returns>Success Or Failure Alert Message</returns>
          [HttpPost]
          [Route("contact-us")]
          public async Task<IActionResult> ContactUs([FromForm] ContactDto model)
          {
               try
               {
                    string toEmail = _appSettings.ToEmailAddress;
                    string emailBody = Utilities.GetEmailTemplateValue("Contactus/Body");
                    string emailSubject = Utilities.GetEmailTemplateValue("Contactus/Subject");
                    emailBody = emailBody.Replace("@@@title", "Rankride Enquiry");
                    emailBody = emailBody.Replace("@@@Name", model.Name)
                                .Replace("@@@Phone", model.Phone)
                                .Replace("@@@Email", model.Email)
                                .Replace("@@@Message", model.Message);
                    await _emailSender.SendEmailAsync(toEmail, emailSubject, emailBody);
                    return Ok(new ApiResponse
                    {
                         Message = "Success,Thank you. Your query has been submitted successfully. We will respond back as soon as possible.",
                         Success = true,
                         IpAddress = Helpers.GetIpAddress()
                    });
               }
               catch (Exception ex)
               {
                    return Ok(new ApiResponse
                    {
                         Message = "Oops,Something went wrong. Please contact to support.",
                         Success = true,
                         IpAddress = Helpers.GetIpAddress()
                    });
               }
          }

        [HttpGet]
        [Route("/")]
        public IActionResult Swagger()
        {
            return Redirect("/swagger");
        }
    }
     //// PUT api/values/5
     //[HttpPut("{id}")]
     //public void Put(int id, [FromBody] string value)
     //{
     //}

     //// DELETE api/values/5
     //[HttpDelete("{id}")]
     //public void Delete(int id)
     //{
     //}
}
