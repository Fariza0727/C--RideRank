using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RR.Core;
using RR.Dto;
using RR.Service;
using RR.Service.Email;
using RR.Service.User;
using RR.ThirdParty;
using RR.Web.Helpers;
using RR.Web.Models;
using System;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace RR.Web.Controllers
{
     public class TransactionController : Controller
     {
          public readonly SessionHelperService _sessionHelperService;
          private readonly UserManager<IdentityUser> _userManager;
          private readonly IUserService _userService;
          private readonly IContestService _contestService;
          private readonly PayPalAPI _paypalApi;
          private readonly ITransactionService _transactionService;
          private readonly IEventService _eventService;
          private readonly ILogger<RegisterDto> _logger;
          private readonly IEmailSender _emailSender;
          private readonly SignInManager<IdentityUser> _signInManager;
          private readonly IHostingEnvironment _environment;

          public TransactionController(UserManager<IdentityUser> userManager,
                IContestService contestService,
                PayPalAPI paypalApi,
                IUserService userService,
                ITransactionService transactionService,
                IEventService eventService,
                ILogger<RegisterDto> logger,
                IEmailSender emailSender,
                SessionHelperService sessionHelperService,
                SignInManager<IdentityUser> signInManager,
                IHostingEnvironment environment)

          {
               _userManager = userManager;
               _contestService = contestService;
               _paypalApi = paypalApi;
               _userService = userService;
               _transactionService = transactionService;
               _eventService = eventService;
               _emailSender = emailSender;
               _logger = logger;
               _sessionHelperService = sessionHelperService;
               _signInManager = signInManager;
               _environment = environment;
          }


          [Authorize(Roles = "TM, PTM,FTM, NTM")]
          [Route("checkout/{teamId}/{contestId}/{eventId}/{plan}")]
          public async Task<IActionResult> Checkout(int contestId, int teamId, int eventId, string plan = "")
          {
               var contestDetail = (contestId > 0 ? await _contestService.GetContestById(contestId) : new ContestLiteDto());


               var user = await _userManager.GetUserAsync(HttpContext.User);

               var userInfo = await _userService.GetUserDetail(user != null ? user.Id : "");

               var eventDetail = (eventId > 0 ? await _eventService.GetEventById(eventId) : new EventDto());

               var teamTransaction = new TransactionLiteDto()
               {
                    ContestId = contestId,
                    EventId = eventId,
                    TeamId = teamId
               };

               if (teamId > 0 && contestId > 0)
               {
                    teamTransaction.PaymentMadeFor = "contest";
               }
               else
               {
                    teamTransaction.PaymentMadeFor = "tokenpurchase";
               }


               if (contestId > 0)
               {
                    var getJoinedUserConest = await _contestService.JoinedUserContest(contestId, eventId);

                    teamTransaction.ContestFee = contestDetail.JoiningFee;
                    teamTransaction.AwardType = contestDetail.AwardTypeId;

                    if (contestDetail.EntryFeeType == "Token")
                    {
                         //If wallet has proper balace for joining the amount
                         if (getJoinedUserConest.Item1.Count > 0 && getJoinedUserConest.Item1.FirstOrDefault(x => x.ContestId == contestId && x.UserId == user.Id) != null)
                         {
                              return RedirectToAction("getcontestofevent", "contest", new
                              {
                                   eventId = teamTransaction.EventId,
                                   /*eventName = eventDetail.Title,*/
                                   contestId = teamTransaction.ContestId
                              });
                         }
                         else
                         {
                              if (userInfo.WalletToken == contestDetail.JoiningFee || contestDetail.JoiningFee < userInfo.WalletToken || contestDetail.JoiningFee == 0)
                              {
                                   var transactionDto = new TransactionDto
                                   {
                                        UserId = userInfo.UserId,
                                        TextMessage = "Approved For Token Purchased of given contest",
                                        TokenCredit = Convert.ToInt32(teamTransaction.ContestFee),
                                        TransactionType = (byte)Enums.TransactionType.Token,
                                        TransactionId = string.Concat("TOK", teamTransaction.ContestId, "CON"),
                                        Description = "Successfully joined the contest!"
                                   };

                                   userInfo.WalletToken = userInfo.WalletToken - Convert.ToInt32(transactionDto.TokenCredit);
                                   userInfo.Avtar = userInfo.Avtar.Replace("/images/profilePicture/", "");
                                   await _userService.AddEditUserDetail(userInfo);

                                   var joinedContest = new TransactionLiteDto
                                   {
                                        ContestId = contestId,
                                        UserId = (userInfo != null ? userInfo.UserId : string.Empty),
                                        TeamId = teamId
                                   };
                                   await Task.FromResult(InsertTransaction(transactionDto, joinedContest));

                                   //await BindTransactionData(new PayPalResponseDto() { }, teamTransaction);
                                   return RedirectToAction("getcontestofevent", "contest", new
                                   {
                                        eventId = teamTransaction.EventId,
                                        /*eventName = eventDetail.Title,*/
                                        contestId = teamTransaction.ContestId
                                   });
                              }
                              else
                              {
                                   teamTransaction.PaymentMadeFor = "tpcj";
                                   var requiredToken = contestDetail.JoiningFee - userInfo.WalletToken;
                                   if (requiredToken <= 100)
                                   {
                                        teamTransaction.ContestFee = 10;
                                        teamTransaction.TokenWillGet = "You will get 100 Tokens";
                                   }
                                   else if (requiredToken > 100 && requiredToken <= 1000)
                                   {
                                        teamTransaction.ContestFee = 85;
                                        teamTransaction.TokenWillGet = "You will get 1000 Tokens";
                                   }
                                   else
                                   {
                                        teamTransaction.ContestFee = 350;
                                        teamTransaction.TokenWillGet = "You will get 5000 Tokens";
                                   }
                                   // update 3 Feb 2020
                                   //return View(teamTransaction);
                                   return RedirectToAction("getcontestofevent", "contest", new
                                   {
                                        eventId = teamTransaction.EventId,
                                        /*eventName = eventDetail.Title,*/
                                        contestId = teamTransaction.ContestId
                                   });
                              }
                         }
                    }
               }
               else
               {
                    teamTransaction.ContestFee = GetTokenFee(plan);
                    teamTransaction.IsToken = true;
               }

               // update 3 Feb 2020
               //return View(teamTransaction);
               return RedirectToAction("getcontestofevent", "contest", new
               {
                    eventId = teamTransaction.EventId,
                    /*eventName = eventDetail.Title,*/
                    contestId = teamTransaction.ContestId
               });
          }


          public async Task<IActionResult> Subscription(string plan, bool isUpgrade)
          {
               if (TempData["isUpgrade"] != null)
                    isUpgrade = Convert.ToBoolean(TempData["isUpgrade"]);

               var teamTransaction = new TransactionLiteDto();
               if (_sessionHelperService.UserDetail != null && !isUpgrade)
               {
                    teamTransaction.PaymentMadeFor = "register";
                    teamTransaction.ContestFee = GetPlanFee(_sessionHelperService.UserDetail.PlayerType, _sessionHelperService.UserDetail.PlanType);

                    teamTransaction.PaymentMode = _sessionHelperService.UserDetail.PlanType;
                    teamTransaction.IsUpgrade = isUpgrade;
                    teamTransaction.PlayerType = _sessionHelperService.UserDetail.PlayerType;
               }
               else
               {
                    teamTransaction.PaymentMadeFor = "upgrade";
                    teamTransaction.ContestFee = GetUpgradeFee(plan);

                    teamTransaction.PaymentMode = (plan == "itm-q" ? "Quarterly" : "Yearly");
                    teamTransaction.IsUpgrade = isUpgrade;
                    teamTransaction.PlayerType = (isUpgrade ? plan == "ptm" ? "PRO PLAYER" : "INTERMEDIATE PLAYER" : string.Empty);
               }
               return View(await Task.FromResult(teamTransaction));
          }

          [HttpPost]
          public async Task<IActionResult> PostCheckout(TransactionLiteDto transactionLiteDto)
          {
               try
               {
                    transactionLiteDto.ExpiryDate = transactionLiteDto.ExpiryMonth.ToString() + transactionLiteDto.ExpiryYear.ToString();
                    var userInfo = new UserDetailDto();
                    var becomeAPlayer = new BecomeAPlayerDto();
                    var user = new IdentityUser();

                    if (_sessionHelperService.UserDetail == null)
                    {
                         user = await _userManager.GetUserAsync(HttpContext.User);
                         userInfo = await _userService.GetUserDetail(user != null ? user.Id : "");
                    }
                    else
                    {
                         becomeAPlayer = _sessionHelperService.UserDetail;
                         _sessionHelperService.UserId = user.Id;
                    }

                    var payPalDto = new PayPalDto
                    {
                         Address = new AddressDto
                         {
                              Address1 = (userInfo != null ? userInfo.Address1 : becomeAPlayer.Address1),
                              Address2 = (userInfo != null ? userInfo.Address2 : becomeAPlayer.Address2)
                         },
                         Amount = Convert.ToDecimal(transactionLiteDto.ContestFee),
                         CreditCardDetails = new CreditCardDto
                         {
                              CardNumber = transactionLiteDto.CardNumber,
                              CVV = transactionLiteDto.CVV,
                              ExpiryDate = transactionLiteDto.ExpiryDate
                         },
                         InvNumber = Guid.NewGuid().ToString(),
                         PONumber = Guid.NewGuid().ToString()
                    };

                    var data = _paypalApi.PayNow(payPalDto);
                    if (data.IsSuccess)
                    {
                         switch (transactionLiteDto.PaymentMadeFor)
                         {
                              case "register":
                                   #region Register block
                                   if (data.Result == 0)
                                   {
                                        try
                                        {
                                             #region Make transaction entry for registration

                                             var transactionDto = new TransactionDto
                                             {
                                                  AuthCode = data.AuthCode,
                                                  ResponseMessage = data.RespMsg,
                                                  TransactionDebit = Convert.ToDecimal(transactionLiteDto.ContestFee),
                                                  TransactionId = data.PnRef,
                                                  TokenCredit = 0,
                                                  TextMessage = "Made payment for account create",
                                                  TransactionType = (byte)Enums.TransactionType.Paypal,
                                                  UserId = _sessionHelperService.UserId,
                                                  Description = "User is successfuly registered!!"
                                             };

                                             await Task.FromResult(InsertTransaction(transactionDto, new TransactionLiteDto() { }));

                                             #endregion
                                             #region Create user in DB

                                             user = new IdentityUser
                                             {
                                                  Id = _sessionHelperService.UserId,
                                                  UserName = becomeAPlayer.Email,
                                                  Email = becomeAPlayer.Email
                                             };
                                             var result = await _userManager.CreateAsync(user, becomeAPlayer.Password);
                                             if (result.Succeeded)
                                             {
                                                  UserDetailDto userDetail = new UserDetailDto();
                                                  userDetail.Address1 = becomeAPlayer.Address1;
                                                  userDetail.Address2 = becomeAPlayer.Address2;
                                                  userDetail.Address3 = becomeAPlayer.Address3;
                                                  userDetail.Avtar = becomeAPlayer.FileName;
                                                  userDetail.Email = becomeAPlayer.Email;
                                                  userDetail.PhoneNumber = becomeAPlayer.PhoneNumber;
                                                  userDetail.FirstName = becomeAPlayer.FirstName;
                                                  userDetail.UserId = _sessionHelperService.UserId;
                                                  userDetail.IsActive = true;
                                                  userDetail.DOB = becomeAPlayer.DateOfBirth;
                                                  userDetail.UserName = becomeAPlayer.UserName;
                                                  userDetail.SubscriptionExpiryDate = (transactionLiteDto.PaymentMode != null ?
                                          transactionLiteDto.PaymentMode == "Yearly" ?
                                          DateTime.Now.AddYears(1) : DateTime.Now.AddMonths(4)
                                          : userInfo.SubscriptionExpiryDate);

                                                  userDetail.PlayerType = transactionLiteDto.PlayerType;
                                                  //userInfo.Avtar = userInfo.Avtar.Replace("/images/profilePicture/", "");
                                                  await _userService.AddEditUserDetail(userDetail);
                                                  await _userManager.AddToRoleAsync(user, becomeAPlayer.PlayerType);
                                                  _logger.LogInformation("User created a new account with password.");

                                                  var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                                                  var callbackUrl = Url.Page(
                                                      "/Account/ConfirmEmail",
                                                      pageHandler: null,
                                                      values: new { userId = user.Id, code, area = "Identity" },
                                                      protocol: Request.Scheme);

                                                  callbackUrl = callbackUrl.Replace("Identity/", ""); // Request.Scheme + "://" + Request.Host.Value.ToString() + "/Account/ResetPassword?code=" + code + "&email=" + passwordDto.Email;

                                                  var path = Path.Combine(_environment.WebRootPath, "Templates", "Template.xml");

                                                  string emailBody = Utilities.GetEmailTemplateValue("AccountActivation/Body", path);
                                                  string emailSubject = Utilities.GetEmailTemplateValue("AccountActivation/Subject", path);
                                                  emailBody = emailBody.Replace("@@@UserEmail", becomeAPlayer.UserName);
                                                  emailBody = emailBody.Replace("@@@Url", HtmlEncoder.Default.Encode(callbackUrl));

                                                  await _emailSender.SendEmailAsync(
                                                      becomeAPlayer.Email,
                                                      emailSubject,
                                                      emailBody);
                                             }
                                             else
                                             {
                                                  return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Oops!!!</strong>" + result.Errors + "</div>");
                                             }
                                             #endregion
                                        }
                                        catch (Exception ex)
                                        {
                                             return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Oops!!!</strong>" + ex.StackTrace + "</div>");
                                        }

                                        TempData["message"] = "Account has been created successfully. Please check you inbox to activate your account.";
                                   }
                                   else
                                   {
                                        return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Oops!!!</strong>" + data.RespMsg + "</div>");
                                   }
                                   #endregion
                                   break;
                              case "upgrade":
                                   #region Upgrade block
                                   if (data.Result == 0)
                                   {
                                        var transactionDto = new TransactionDto
                                        {
                                             AuthCode = data.AuthCode,
                                             ResponseMessage = data.RespMsg,
                                             TransactionDebit = Convert.ToDecimal(transactionLiteDto.ContestFee),
                                             TransactionId = data.PnRef,
                                             TokenCredit = 0,
                                             TextMessage = "Amount paid for plan upgrade.",
                                             TransactionType = (byte)Enums.TransactionType.Paypal,
                                             UserId = (String.IsNullOrEmpty(_sessionHelperService.UserId) ? user.Id : _sessionHelperService.UserId),
                                             Description = "User successfuly upgraded plan.!!"
                                        };
                                        await Task.FromResult(InsertTransaction(transactionDto, new TransactionLiteDto() { }));

                                        user = await _userManager.GetUserAsync(HttpContext.User);
                                        userInfo = await _userService.GetUserDetail(user != null ? user.Id : "");

                                        userInfo.SubscriptionExpiryDate = (transactionLiteDto.PaymentMode != null ?
                                     transactionLiteDto.PaymentMode == "Yearly" ?
                                     DateTime.Now.AddYears(1) : DateTime.Now.AddMonths(4)
                                     : userInfo.SubscriptionExpiryDate);
                                        if (userInfo.PlayerType != null)
                                        {
                                             await _userManager.RemoveFromRoleAsync(user, userInfo.PlayerType.ToUpper().Trim());
                                             await _userManager.AddToRoleAsync(user, transactionLiteDto.PlayerType);

                                             userInfo.PlayerType = transactionLiteDto.PlayerType;
                                             userInfo.Avtar = userInfo.Avtar.Replace("/images/profilePicture/", "");
                                             await _userService.AddEditUserDetail(userInfo);
                                             await _signInManager.SignOutAsync();
                                             _logger.LogInformation("User logged out.");
                                        }

                                        TempData["message"] = "Account has been upgraded successfully. Please re-login to your account.";
                                   }
                                   else
                                   {
                                        return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Oops!!!</strong>" + data.RespMsg + "</div>");
                                   }
                                   #endregion
                                   break;
                              case "contest":
                                   #region contest block
                                   if (data.Result == 0)
                                   {
                                        var transactionDto = new TransactionDto
                                        {
                                             AuthCode = data.AuthCode,
                                             ResponseMessage = data.RespMsg,
                                             TransactionDebit = Convert.ToDecimal(transactionLiteDto.ContestFee),
                                             TransactionId = data.PnRef,
                                             TokenCredit = transactionLiteDto.TokenCount,
                                             TextMessage = "",
                                             TransactionType = (byte)Enums.TransactionType.Paypal,
                                             UserId = (String.IsNullOrEmpty(_sessionHelperService.UserId) ? user.Id : _sessionHelperService.UserId),
                                             Description = "User is successfully joined the contest!"
                                        };

                                        var joinedContest = new TransactionLiteDto
                                        {
                                             ContestId = transactionLiteDto.ContestId,
                                             UserId = (userInfo != null ? userInfo.UserId : string.Empty),
                                             TeamId = transactionLiteDto.TeamId
                                        };
                                        await Task.FromResult(InsertTransaction(transactionDto, joinedContest));
                                        TempData["message"] = "Contest joined successfully.";
                                   }
                                   else
                                   {
                                        return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Oops!!!</strong>" + data.RespMsg + "</div>");
                                   }
                                   #endregion
                                   break;
                              case "tokenpurchase":
                                   #region token purchase block
                                   if (data.Result == 0)
                                   {

                                        var transactionDto = new TransactionDto
                                        {
                                             AuthCode = data.AuthCode,
                                             ResponseMessage = data.RespMsg,
                                             TransactionDebit = Convert.ToDecimal(transactionLiteDto.ContestFee),
                                             TransactionId = data.PnRef,
                                             TokenCredit = transactionLiteDto.ContestFee == 10 ? 100 :
                                                         transactionLiteDto.ContestFee == 85 ? 1000 : 5000,
                                             TextMessage = "Approved For Token Purchased",
                                             TransactionType = (byte)Enums.TransactionType.Paypal,
                                             UserId = (String.IsNullOrEmpty(_sessionHelperService.UserId) ? user.Id : _sessionHelperService.UserId),
                                             Description = "Token purchased successfully"
                                        };
                                        userInfo.WalletToken = (userInfo.WalletToken.HasValue ? userInfo.WalletToken.Value : 0) + Convert.ToInt32(transactionDto.TokenCredit);
                                        userInfo.Avtar = userInfo.Avtar.Replace("/images/profilePicture/", "");
                                        await _userService.AddEditUserDetail(userInfo);
                                        await Task.FromResult(InsertTransaction(transactionDto, new TransactionLiteDto() { }));

                                        TempData["message"] = "Token added to your account successfully.";
                                   }
                                   else
                                   {
                                        return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Oops!!!</strong>" + data.RespMsg + "</div>");
                                   }
                                   #endregion
                                   break;
                              case "tpcj":
                                   #region token purchase block
                                   if (data.Result == 0)
                                   {
                                        var transactionDto = new TransactionDto
                                        {
                                             AuthCode = data.AuthCode,
                                             ResponseMessage = data.RespMsg,
                                             TransactionDebit = Convert.ToDecimal(transactionLiteDto.ContestFee),
                                             TransactionId = data.PnRef,
                                             TokenCredit = transactionLiteDto.ContestFee == 10 ? 100 :
                                                         transactionLiteDto.ContestFee == 85 ? 1000 : 5000,
                                             TextMessage = "Approved For Token Purchased",
                                             TransactionType = (byte)Enums.TransactionType.Paypal,
                                             UserId = (String.IsNullOrEmpty(_sessionHelperService.UserId) ? user.Id : _sessionHelperService.UserId),
                                             Description = "Token purchased successfully"
                                        };
                                        await Task.FromResult(InsertTransaction(transactionDto, new TransactionLiteDto() { }));
                                        userInfo.WalletToken = (userInfo.WalletToken.HasValue ? userInfo.WalletToken.Value : 0) + Convert.ToInt32(transactionDto.TokenCredit);


                                        //Join contest
                                        var contestDetail = (transactionLiteDto.ContestId > 0 ? await _contestService.GetContestById(transactionLiteDto.ContestId) : new ContestLiteDto());
                                        transactionDto = new TransactionDto
                                        {
                                             TextMessage = "Approved For Token based contest joining",
                                             TokenCredit = Convert.ToInt32(contestDetail.JoiningFee),
                                             TransactionType = (byte)Enums.TransactionType.Token,
                                             TransactionId = string.Concat("TOK", transactionLiteDto.ContestId, "CON"),
                                             Description = "Successfully joined the contest!"
                                        };

                                        userInfo.WalletToken = userInfo.WalletToken - Convert.ToInt32(transactionDto.TokenCredit);
                                        userInfo.Avtar = userInfo.Avtar.Replace("/images/profilePicture/", "");
                                        await _userService.AddEditUserDetail(userInfo);

                                        var joinedContest = new TransactionLiteDto
                                        {
                                             ContestId = transactionLiteDto.ContestId,
                                             UserId = (userInfo != null ? userInfo.UserId : string.Empty),
                                             TeamId = transactionLiteDto.EventId
                                        };
                                        await Task.FromResult(InsertTransaction(transactionDto, joinedContest));

                                        TempData["message"] = "Contest joined successfully.";
                                   }
                                   else
                                   {
                                        return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Oops!!!</strong>" + data.RespMsg + "</div>");
                                   }
                                   #endregion
                                   break;
                         }
                    }
                    else
                    {
                         return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Oops!!!</strong> Payment failed due to technical error, Please try again.</div>");
                    }
               }
               catch (Exception ex)
               {
                    return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Oops!!!</strong>" + ex.Message + "</div>");
               }
               return Content("<div class='alert alert-success alert-dismissible' role='alert' style='background-color:#59ca7c;'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Success!!!</strong> Please wait, we are processing your request.</div>");
          }

          public async Task InsertTransaction(TransactionDto transactionDto, TransactionLiteDto joinedContest)
          {
               // Add Transaction Detail into Transaction and JoinedContest
               await _transactionService.InsertTransactionDetail(transactionDto, joinedContest);
          }

          public async Task BindTransactionData(PayPalResponseDto data, TransactionLiteDto transactionLiteDto)
          {
               var userInfo = new UserDetailDto();
               var becomeAPlayer = new BecomeAPlayerDto();
               var user = new IdentityUser();
               if (_sessionHelperService.UserDetail != null)
               {
                    user = await _userManager.GetUserAsync(HttpContext.User);
                    userInfo = await _userService.GetUserDetail(user != null ? user.Id : "");
                    if (userInfo.PlayerType != null)
                    {
                         await _userManager.RemoveFromRoleAsync(user, userInfo.PlayerType);
                         await _userManager.AddToRoleAsync(user, transactionLiteDto.PlayerType);
                    }
               }
               else
               {
                    //userInfo = JsonConvert.DeserializeObject<UserDetailDto>(_sessionHelperService.UserDetail);
               }

               var transactionDto = new TransactionDto();

               userInfo.SubscriptionExpiryDate = (transactionLiteDto.PaymentMode != null ?
                        transactionLiteDto.PaymentMode == "Yearly" ?
                        DateTime.Now.AddYears(1) : DateTime.Now.AddMonths(4)
                        : userInfo.SubscriptionExpiryDate);

               userInfo.PlayerType = transactionLiteDto.PlayerType;

               if (data.IsSuccess)
               {
                    transactionDto = new TransactionDto
                    {
                         AuthCode = data.AuthCode,
                         ResponseMessage = data.RespMsg,
                         TransactionDebit = Convert.ToDecimal(transactionLiteDto.ContestFee),
                         TransactionId = data.PnRef,
                         TokenCredit = transactionLiteDto.TokenCount,
                         TextMessage = transactionLiteDto.TokenCount > 0 ? "Approved For Token Purchased" : "",
                         TransactionType = (byte)Enums.TransactionType.Paypal,
                         UserId = (String.IsNullOrEmpty(_sessionHelperService.UserId) ? user.Id : _sessionHelperService.UserId),
                         Description = transactionLiteDto.ContestId > 0 ? "User is successfully joined the contest!" : "User is successfuly registered!!"
                    };

                    //userInfo.WalletToken = userInfo.WalletToken + transactionDto.TokenCredit;
                    //await _userService.AddEditUserDetail(userInfo);
               }
               else
               {
                    transactionDto = new TransactionDto
                    {
                         TextMessage = "Approved For Token Purchased of given contest",
                         TokenCredit = Convert.ToInt32(transactionLiteDto.ContestFee),
                         TransactionType = (byte)Enums.TransactionType.Token,
                         TransactionId = string.Concat("TOK", transactionLiteDto.ContestId, "CON"),
                         Description = transactionLiteDto.ContestId > 0 ? "User is successfully joined the contest!" : "User is successfuly registered!!"
                    };

                    userInfo.WalletToken = userInfo.WalletToken - Convert.ToInt32(transactionDto.TokenCredit);
                    userInfo.Avtar = userInfo.Avtar.Replace("/images/profilePicture/", "");
                    await _userService.AddEditUserDetail(userInfo);

               }
               var joinedContest = new TransactionLiteDto
               {
                    ContestId = transactionLiteDto.ContestId,
                    UserId = (userInfo != null ? userInfo.UserId : string.Empty),
                    TeamId = transactionLiteDto.TeamId
               };
               await Task.FromResult(InsertTransaction(transactionDto, joinedContest));
          }

          public IActionResult RegisterSubscribePlayer()
          {
               return View();
          }

          [HttpPost]
          public async Task<IActionResult> RegisterSubscribePlayer(BecomeAPlayerDto aPlayerDto)
          {
               try
               {
                    string fileName = "";
                    if (ModelState.IsValid)
                    {
                         #region Create Identity User
                         var user = new IdentityUser
                         {
                              Id = _sessionHelperService.UserId,
                              UserName = aPlayerDto.Email,
                              Email = aPlayerDto.Email
                         };
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
                              userDetail.UserId = _sessionHelperService.UserId;
                              userDetail.IsActive = true;
                              userDetail.DOB = aPlayerDto.DateOfBirth;
                              userDetail.UserName = aPlayerDto.UserName;
                              await _userService.AddEditUserDetail(userDetail);
                              await _userManager.AddToRoleAsync(user, aPlayerDto.PlayerType);
                              _logger.LogInformation("User created a new account with password.");

                              var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                              var callbackUrl = Url.Page(
                                  "/Account/ConfirmEmail",
                                  pageHandler: null,
                                  values: new { userId = user.Id, code, area = "Identity" },
                                  protocol: Request.Scheme);

                              callbackUrl = callbackUrl.Replace("Identity/", ""); // Request.Scheme + "://" + Request.Host.Value.ToString() + "/Account/ResetPassword?code=" + code + "&email=" + passwordDto.Email;

                              await _emailSender.SendEmailAsync(
                                  aPlayerDto.Email,
                                  "Confirm your email",
                                  $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                              //await _signInManager.SignInAsync(user, isPersistent: false);
                              //return RedirectToAction("MyAccount");
                              return Content("<div class='alert alert-success alert-dismissible' role='alert'><strong>Congrats!!!</strong> Your account has been created successfully. An email has been sent to your registered email address in order to access your account. Please check inbox. If not received please check in junk folder.</div>");
                              #endregion
                         }
                    }
               }
               catch (Exception ex)
               {
                    return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Oops!!!</strong> " + ex.Message + "</div>");
               }

               return View();
          }

          public decimal GetPlanFee(string accountType, string planType)
          {
               decimal fee = 0;
               try
               {
                    switch (accountType.ToUpper())
                    {
                         case "NOVICE PLAYER":
                              fee = 25;
                              break;
                         case "INTERMEDIATE PLAYER":
                              if (planType.ToLower() == "yearly")
                                   fee = 200;
                              else
                                   fee = 55;
                              break;
                         case "PRO PLAYER":
                              fee = 20;
                              break;
                    }
               }
               catch (Exception ex)
               {
               }
               return fee;
          }

          public decimal GetTokenFee(string planType)
          {
               decimal fee = 0;
               try
               {
                    switch (planType)
                    {
                         case "jSsUE":
                              fee = 10;
                              break;
                         case "BlLML":
                              fee = 85;
                              break;
                         case "smAmE":
                              fee = 350;
                              break;
                         default:
                              fee = 350;
                              break;
                    }
               }
               catch (Exception ex)
               {
               }
               return fee;
          }

          public decimal GetUpgradeFee(string planType)
          {
               decimal fee = 0;
               try
               {
                    switch (planType)
                    {
                         case "itm-y":
                              fee = 200;
                              break;
                         case "itm-q":
                              fee = 55;
                              break;
                         case "ptm":
                              fee = 50;
                              break;
                    }
               }
               catch (Exception ex)
               {
               }
               return fee;
          }
     }
}