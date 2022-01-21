using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RR.Core;
using RR.Dto;
using RR.Dto.Contest;
using RR.Service;
using RR.Service.User;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.WebApi.Controllers
{
     [Route("api/[controller]")]
     [ApiController]
     public class ContestController : Controller
     {
          #region Constructor     
          private readonly IContestService _contestService;
          private readonly IEventService _eventService;
          private readonly IUserService _userService;
          private readonly UserManager<IdentityUser> _userManager;
          private readonly ITransactionService _transactionService;
          private readonly ITeamService _teamService;
          private readonly IContestUserWinnerService _contestUWService;

          public ContestController(IContestService contestService,
               IEventService eventService,
               IUserService userService,
                UserManager<IdentityUser> userManager,
                ITeamService teamService,
                ITransactionService transactionService,
                IContestUserWinnerService contestUWService)
          {
               _teamService = teamService;
               _contestService = contestService;
               _eventService = eventService;
               _userManager = userManager;
               _userService = userService;
               _transactionService = transactionService;
               _contestUWService = contestUWService;
          }

          #endregion

          /// <summary>
          /// Get all contests of specific event
          /// </summary>
          /// <param name="userId">UserId</param>
          /// <param name="eventId">EventId</param>
          /// <param name="priceTo">Price To</param>
          /// <param name="priceFrom">Price From</param>
          /// <param name="priceFilter">Price Filter</param>
          /// <returns></returns>
          [HttpPost]
          [Route("getcontestofevent")]
          public async Task<OkObjectResult> PostContestOfEvent([FromForm] string userId, [FromForm] int eventId,
               [FromForm] int priceTo = 0, [FromForm] int priceFrom = 0, [FromForm] int priceFilter = 1)
          {
               try
               {
                    var eventDetail = await _eventService.GetEventById(eventId);

                    if ((priceTo > 0 || priceFrom > 0) && priceFilter > 0)
                    {
                         var contest = await _contestService.FilterContest(eventId, priceFrom, priceTo, priceFilter);
                         return Ok(new ApiResponse
                         {
                              Data = contest,
                              IpAddress = Helpers.GetIpAddress(),
                              Success = true,
                              Message = "Filtered Contest Of event"
                         });
                    }
                    else
                    {
                         var contest = await _contestService.GetContestOfEvent(eventId, eventDetail.Title.Replace(" ", "-").ToLower());
                         return Ok(new ApiResponse
                         {
                              Data = contest.Item1,
                              IpAddress = Helpers.GetIpAddress(),
                              Success = true,
                              Message = "Contest Of event"
                         });
                    }
               }
               catch (Exception ex)
               {
                    return Ok(new ApiResponse
                    {
                         IpAddress = Helpers.GetIpAddress(),
                         Success = false,
                         Message = ex.Message
                    });
               }
          }

          /// <summary>
          /// all awards of that particular contest is fetch from here
          /// </summary>
          /// <param name="contestId">Contest Id</param>
          /// <param name="eventId">Event Id</param>
          /// <returns></returns>
          [HttpGet]
          [Route("contestawards/{contestId}/{eventId}")]
          public async Task<OkObjectResult> GetContestAwards(int contestId, int eventId)
          {
               try
               {
                    var contestAward = await _contestService.GetContestAwards(contestId, eventId);
                    return Ok(new ApiResponse
                    {
                         Data = contestAward,
                         IpAddress = Helpers.GetIpAddress(),
                         Success = true,
                         Message = ""
                    });
               }
               catch (Exception ex)
               {
                    return Ok(new ApiResponse
                    {
                         IpAddress = Helpers.GetIpAddress(),
                         Success = false,
                         Message = ex.Message
                    });
               }
          }

          /// <summary>
          /// Joined User Contest
          /// </summary>
          /// <param name="contestId">Contest Id</param>
          /// <param name="eventId">Event Id</param>
          /// <returns></returns>
          [HttpGet]
          [Route("joinedusercontest/{contestId}/{eventId}")]
          public async Task<OkObjectResult> GetJoinedUserContest(long contestId, int eventId)
          {
               try
               {
                    var joinedUsers = await _contestService.JoinedUserContest(contestId, eventId);
                    return Ok(new ApiResponse
                    {
                         Data = joinedUsers,
                         Success = true,
                         IpAddress = Helpers.GetIpAddress(),
                         Message = ""
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
          /// contest which the user has joined.
          /// </summary>
          /// <param name="userId">UserId</param>
          /// <param name="contestId">ContestId</param>
          /// <param name="teamId">TeamId</param>
          /// <returns></returns>
          [HttpGet]
          [Route("usercontest/{userId}/{contestId}/{teamId}")]
          public async Task<OkObjectResult> GetContestOfUser(string userId, int contestId, int teamId)
          {
               try
               {
                    var user = await _userManager.FindByIdAsync(userId);
                    var userInfo = await _userService.GetUserDetail(user != null ? user.Id : "");

                    var contestDetail = await _contestService.GetContestById(contestId);

                    var joinedContestDetailOfUser = await _contestService.
                         GetContestDetailOfCurrentUser(contestDetail.EventId, contestId, teamId);
                    return Ok(new ApiResponse
                    {
                         Data = new Tuple<IEnumerable<RiderContestLiteDto>,
                         IEnumerable<BullContestLiteDto>, string, string, decimal>(
                                joinedContestDetailOfUser.Item1,
                                joinedContestDetailOfUser.Item2,
                                userInfo.UserName,
                                contestDetail.Title,
                                contestDetail.JoiningFee),
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
          /// Join Private Contest
          /// </summary>
          /// <param name="joinPrivateContestDto">JoinPrivateContestDto</param>
          /// <returns></returns>
          [HttpPost]
          [Route("joinprivatecontest")]
          public async Task<OkObjectResult> JoinPrivateContest([FromForm] JoinPrivateContestDto joinPrivateContestDto)
          {
               try
               {
                    if (!string.IsNullOrEmpty(joinPrivateContestDto.Code))
                    {
                         var privateContest = await _contestService.GetContestByUniqueCode(joinPrivateContestDto.Code);

                         if (privateContest != null)
                         {
                              if (string.IsNullOrEmpty(privateContest.Message))
                              {
                                   return Ok(new ApiResponse
                                   {
                                        Message = "Please wait, redirecting to team formation page.",
                                        Success = true,
                                        IpAddress = Helpers.GetIpAddress(),
                                        RedirectPath = "/team-formation"
                                   });
                              }
                              else
                              {
                                   return Ok(new ApiResponse
                                   {
                                        Message = "This contest has been filled up.",
                                        IpAddress = Helpers.GetIpAddress(),
                                        Success = false
                                   });
                              }
                         }
                         return Ok(new ApiResponse
                         {
                              Message = "There is no such Contest is available!!",
                              IpAddress = Helpers.GetIpAddress(),
                              Success = false
                         });
                    }
                    else
                    {
                         return Ok(new ApiResponse
                         {
                              Message = "Please enter private contest code.!!",
                              IpAddress = Helpers.GetIpAddress(),
                              Success = false
                         });
                    }
               }
               catch (Exception ex)
               {
                    return Ok(new ApiResponse
                    {
                         IpAddress = Helpers.GetIpAddress(),
                         Success = false,
                         Message = ex.Message
                    });
               }
          }

          /// <summary>
          /// Get All Player Points of events
          /// </summary>
          /// <returns></returns>
          [HttpPost]
          [Route("conteststandings")]
          public async Task<OkObjectResult> GetPlayerPoints([FromForm] string userId)
          {
               var contestDetail = await _contestService.GetContestStandingsApi(userId);

               return Ok(new ApiResponse
               {
                    Data = contestDetail,
                    Success = true,
                    IpAddress = Helpers.GetIpAddress(),
                    Message = ""
               });
          }

          /// <summary>
          /// Join Contest via token or amount
          /// </summary>
          /// <param name="payPalResponseDto">PaypalResponseDto</param>
          /// <param name="transactionLiteApiDto">TransactionDetails</param>
          /// <returns></returns>
          [HttpPost]
          [Route("joincontest")]
          public async Task<OkObjectResult> PostJoinContest([FromForm] PayPalResponseLiteDto payPalResponseLiteDto,
                                                           [FromForm] TransactionLiteApiDto transactionLiteApiDto)
          {
               try
               {
                    var contestDetail = (transactionLiteApiDto.ContestId > 0 ? await _contestService.GetContestById(transactionLiteApiDto.ContestId) : new ContestLiteDto());
                    var user = await _userManager.FindByIdAsync(transactionLiteApiDto.UserId);
                    var userInfo = await _userService.GetUserDetail(user != null ? user.Id : "");
                    if (contestDetail.EntryFeeType == "Token")
                    {
                         //If wallet has proper balace for joining the amount
                         if (userInfo.WalletToken == contestDetail.JoiningFee || contestDetail.JoiningFee < userInfo.WalletToken)
                         {
                              var transactionDto = new TransactionDto
                              {
                                   UserId = userInfo.UserId,
                                   TextMessage = "Approved For Token Purchased of given contest",
                                   TokenCredit = Convert.ToInt32(transactionLiteApiDto.ContestFee),
                                   TransactionType = (byte)Enums.TransactionType.Token,
                                   TransactionId = string.Concat("TOK", transactionLiteApiDto.ContestId, "CON"),
                                   Description = "Successfully joined the contest!"
                              };

                              userInfo.WalletToken = userInfo.WalletToken - Convert.ToInt32(transactionDto.TokenCredit);
                              userInfo.Avtar = userInfo.Avtar.Replace("/images/profilePicture/", "");
                              await _userService.AddEditUserDetail(userInfo);

                              var joinedContest = new TransactionLiteDto
                              {
                                   ContestId = transactionLiteApiDto.ContestId,
                                   UserId = (userInfo != null ? userInfo.UserId : string.Empty),
                                   TeamId = transactionLiteApiDto.TeamId
                              };
                              await _transactionService.InsertTransactionDetail(transactionDto, joinedContest);
                         }
                         else
                         {
                              transactionLiteApiDto.PaymentMadeFor = "tpcj";
                              var requiredToken = contestDetail.JoiningFee - userInfo.WalletToken;
                              if (requiredToken <= 100)
                              {
                                   transactionLiteApiDto.ContestFee = 10;
                                   transactionLiteApiDto.TokenWillGet = "You will get 100 CowBoy Coins";
                              }
                              else if (requiredToken > 100 && requiredToken <= 1000)
                              {
                                   transactionLiteApiDto.ContestFee = 85;
                                   transactionLiteApiDto.TokenWillGet = "You will get 1000 CowBoy Coins";
                              }
                              else
                              {
                                   transactionLiteApiDto.ContestFee = 350;
                                   transactionLiteApiDto.TokenWillGet = "You will get 5000 CowBoy Coins";
                              }
                              var transactionDto = Helper.TransactionUtility.MakePayment(payPalResponseLiteDto, transactionLiteApiDto, transactionLiteApiDto.UserId);

                              await _transactionService.InsertTransactionDetail(transactionDto, new TransactionLiteDto() { });
                              userInfo.WalletToken = (userInfo.WalletToken.HasValue ? userInfo.WalletToken.Value : 0) + Convert.ToInt32(transactionDto.TokenCredit);

                              //Join Contest
                              transactionDto = new TransactionDto
                              {
                                   TextMessage = "Approved For Token based contest joining",
                                   TokenCredit = Convert.ToInt32(contestDetail.JoiningFee),
                                   TransactionType = (byte)Enums.TransactionType.Token,
                                   TransactionId = string.Concat("TOK", transactionLiteApiDto.ContestId, "CON"),
                                   Description = "Successfully joined the contest!"
                              };

                              userInfo.WalletToken = userInfo.WalletToken - Convert.ToInt32(transactionDto.TokenCredit);
                              userInfo.Avtar = userInfo.Avtar.Replace("/images/profilePicture/", "");
                              await _userService.AddEditUserDetail(userInfo);

                              var joinedContest = new TransactionLiteDto
                              {
                                   ContestId = transactionLiteApiDto.ContestId,
                                   UserId = (userInfo != null ? userInfo.UserId : string.Empty),
                                   TeamId = transactionLiteApiDto.EventId
                              };
                              await _transactionService.InsertTransactionDetail(transactionDto, joinedContest);
                         }
                    }
                    else
                    {
                         transactionLiteApiDto.PaymentMadeFor = "contest";
                         transactionLiteApiDto.ContestFee = contestDetail.JoiningFee;
                         var transactionDto = Helper.TransactionUtility.MakePayment(payPalResponseLiteDto, transactionLiteApiDto, transactionLiteApiDto.UserId);
                         var joinedContest = new TransactionLiteDto
                         {
                              ContestId = transactionLiteApiDto.ContestId,
                              UserId = (userInfo != null ? userInfo.UserId : string.Empty),
                              TeamId = transactionLiteApiDto.TeamId
                         };
                         await _transactionService.InsertTransactionDetail(transactionDto, joinedContest);
                    }
                    return Ok(new ApiResponse
                    {
                         Message = "Contest joined successfully.",
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

          [HttpPost]
          [Route("contestHistory")]
          public async Task<OkObjectResult> ContestHistory([FromForm]string userId)
          {
               var contestDetail = await _contestService.GetContestOfCurrentUser(userId);

               return Ok(new ApiResponse
               {
                    Data = contestDetail,
                    Success = true,
                    IpAddress = Helpers.GetIpAddress(),
                    Message = ""
               });
          }

          [HttpPost]
          [Route("awardWinnings")]
          public async Task<OkObjectResult> AwardWinnings([FromForm]string userId)
          {
               var contestDetail = await _contestUWService.GetUserWinnings(userId);
               return Ok(new ApiResponse
               {
                    Data = contestDetail,
                    Message = (contestDetail.Count > 0 ? "" : "OOPS!!! YOU DIDN'T WIN ANY CONTEST YET. JOIN MORE CONTESTS FOR WINNING PRIZES."),
                    Success = (contestDetail.Count > 0 ? true : false),
                    IpAddress = Helpers.GetIpAddress()
               });
          }
     }
}