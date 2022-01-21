using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RR.Core;
using RR.Dto;
using RR.Service;
using RR.Service.User;
using System;
using System.Threading.Tasks;

namespace RR.WebApi.Controllers
{
     [Route("api/[controller]")]
     [ApiController]
     public class TransactionController : Controller
     {
          private readonly UserManager<IdentityUser> _userManager;
          private readonly IUserService _userService;
          private readonly IContestService _contestService;
          private readonly ITransactionService _transactionService;

          public TransactionController(UserManager<IdentityUser> userManager,
               IUserService userService,
               IContestService contestService,
               ITransactionService transactionService)
          {
               _userManager = userManager;
               _userService = userService;
               _contestService = contestService;
               _transactionService = transactionService;
          }

          [HttpPost]
          [Route("purchase-token")]
          public async Task<OkObjectResult> PurchaseToken([FromForm] string userId, [FromForm] int token)
          {
               var userInfo = await _userService.GetUserDetail(userId);
               userInfo.WalletToken = Convert.ToInt32(userInfo.WalletToken) + token;
               await _userService.AddEditUserDetail(userInfo);

               var userDetail = await _userService.GetUserDetail(userId);

               return Ok(new ApiResponse
               {
                    Success = true,
                    Data = $"Wallet Tokens:{userDetail.WalletToken}",
                    IpAddress = Helpers.GetIpAddress(),
                    Message = ""
               });
          }

          [HttpPost]
          [Route("wallet-deduction")]
          public async Task<IActionResult> WalletDeduction([FromForm] string userId, [FromForm] int contestId, [FromForm] int teamId, [FromForm] int eventId)
          {
               var contestDetail = (contestId > 0 ? await _contestService.GetContestById(contestId) : new ContestLiteDto());

               var userInfo = await _userService.GetUserDetail(userId);

               var teamTransaction = new TransactionLiteDto()
               {
                    ContestId = contestId,
                    EventId = eventId,
                    TeamId = teamId
               };

               if (contestDetail.EntryFeeType == "Token")
               {
                    //If wallet has proper balace for joining the amount
                    if (userInfo.WalletToken == contestDetail.JoiningFee || contestDetail.JoiningFee < userInfo.WalletToken)
                    {
                         var transactionDto = new TransactionDto
                         {
                              UserId = userInfo.UserId,
                              TextMessage = "Approved For CowBoy Coins Purchased of given contest",
                              TokenCredit = Convert.ToInt32(teamTransaction.ContestFee),
                              TransactionType = (byte)Enums.TransactionType.Token,
                              TransactionId = string.Concat("TOK", teamTransaction.ContestId, "CON"),
                              Description = "Successfully joined the contest!"
                         };

                         userInfo.WalletToken = userInfo.WalletToken - Convert.ToInt32(contestDetail.JoiningFee);
                         await _userService.AddEditUserDetail(userInfo);

                         var joinedContest = new TransactionLiteDto
                         {
                              ContestId = contestId,
                              UserId = (userInfo != null ? userInfo.UserId : string.Empty),
                              TeamId = teamId
                         };
                         await _transactionService.InsertTransactionDetail(transactionDto, joinedContest);

                         return Ok(new ApiResponse
                         {
                              Success = true,
                              Data = transactionDto,
                              IpAddress = Helpers.GetIpAddress(),
                              Condition = "Approved",
                              Message = ""
                         });
                    }
                    return Ok(new ApiResponse
                    {
                         Message = $"Your wallet cowboy coins are not sufficient,You need to add minimum {contestDetail.JoiningFee - userInfo.WalletToken} cowboy coins!",
                         Success = true,
                         IpAddress = Helpers.GetIpAddress(),
                         Condition = "Not Approved"
                    });
               }
               else
               {
                    return Ok(new ApiResponse
                    {
                         Message = $"You just need to do transaction for joining contest!",
                         Success = true,
                         IpAddress = Helpers.GetIpAddress(),
                         Condition = "Payement"
                    });
               }
          }

          [HttpPost]
          [Route("payment-checkout")]
          public async Task<OkObjectResult> PostPaymentCheckout([FromForm] string userId, [FromForm] PayPalResponseLiteDto payPalResponseLiteDto, [FromForm] TransactionLiteApiDto transactionLiteApiDto)
          {
               var userInfo = await _userService.GetUserDetail(userId);
               if (userInfo != null)
               {
                    var transactionDto = Helper.TransactionUtility.MakePayment(payPalResponseLiteDto, transactionLiteApiDto, userId);

                    await _transactionService.InsertTransactionDetail(transactionDto, new TransactionLiteDto() { });

                    userInfo.WalletToken = (userInfo.WalletToken.HasValue ? userInfo.WalletToken.Value : 0) + Convert.ToInt32(transactionDto.TokenCredit);


                    //Join contest
                    var contestDetail = (transactionLiteApiDto.ContestId > 0 ? await _contestService.GetContestById(transactionLiteApiDto.ContestId) : new ContestLiteDto());
                    transactionDto = new TransactionDto
                    {
                         TextMessage = "Approved For cowboy coins based contest joining",
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
                    return Ok(new ApiResponse
                    {
                         Message = "Contest Join Successfully!!",
                         Success = true,
                         IpAddress = Helpers.GetIpAddress()
                    });
               }
               else
               {
                    return Ok(new ApiResponse
                    {
                         Message = "User is not available!!",
                         Success = true,
                         IpAddress = Helpers.GetIpAddress()
                    });
               }
          }
     }
}