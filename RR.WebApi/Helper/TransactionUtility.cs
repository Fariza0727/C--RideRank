using RR.Core;
using RR.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.WebApi.Helper
{
     public class TransactionUtility
     {
          public static TransactionDto MakePayment(PayPalResponseLiteDto payPalResponseLiteDto, TransactionLiteApiDto transactionLiteApiDto, string userId)
          {
               switch (transactionLiteApiDto.PaymentMadeFor)
               {
                    case "register":
                         #region Make transaction entry for registration

                         var transactionDto = new TransactionDto
                         {
                              AuthCode = "",
                              ResponseMessage = payPalResponseLiteDto.State,
                              TransactionDebit = Convert.ToDecimal(transactionLiteApiDto.ContestFee),//Registration Fees
                              TransactionId = payPalResponseLiteDto.Id,
                              TokenCredit = 0,
                              TextMessage = "Made payment for account create",
                              TransactionType = (byte)Enums.TransactionType.Paypal,
                              UserId = userId,
                              Description = "User is successfuly registered!!"
                         };
                         return transactionDto;
                    #endregion
                    case "tokenpurchase":
                         #region token purchase block  
                         transactionDto = new TransactionDto
                         {
                              AuthCode = "",
                              ResponseMessage = payPalResponseLiteDto.State,
                              TransactionDebit = Convert.ToDecimal(transactionLiteApiDto.ContestFee),
                              TransactionId = payPalResponseLiteDto.Id,
                              TokenCredit = transactionLiteApiDto.ContestFee == 10 ? 100 :
                                          transactionLiteApiDto.ContestFee == 85 ? 1000 : 5000,
                              TextMessage = "Approved For CowBoy Coins Purchased",
                              TransactionType = (byte)Enums.TransactionType.Paypal,
                              UserId = userId,
                              Description = "CowBoy Coins purchased successfully"
                         };
                         return transactionDto;
                    #endregion

                    case "contest":
                         #region contest block         
                         transactionDto = new TransactionDto
                         {
                              AuthCode = "",
                              ResponseMessage = payPalResponseLiteDto.State,
                              TransactionDebit = Convert.ToDecimal(transactionLiteApiDto.ContestFee),
                              TransactionId = payPalResponseLiteDto.Id,
                              TokenCredit = transactionLiteApiDto.TokenCount,
                              TextMessage = "",
                              TransactionType = (byte)Enums.TransactionType.Paypal,
                              UserId = userId,
                              Description = "User is successfully joined the contest!"
                         };
                         return transactionDto;
                    #endregion

                    case "upgrade":
                         #region Upgrade block   
                         transactionDto = new TransactionDto
                         {
                              AuthCode = "",
                              ResponseMessage = payPalResponseLiteDto.State,
                              TransactionDebit = Convert.ToDecimal(transactionLiteApiDto.ContestFee),
                              TransactionId = payPalResponseLiteDto.Id,
                              TokenCredit = 0,
                              TextMessage = "Amount paid for plan upgrade.",
                              TransactionType = (byte)Enums.TransactionType.Paypal,
                              UserId = userId,
                              Description = "User successfuly upgraded plan.!!"
                         };
                         return transactionDto;
                    #endregion

                    default:
                         #region token purchase Contest Join block  
                         transactionDto = new TransactionDto
                         {
                              AuthCode = "",
                              ResponseMessage = payPalResponseLiteDto.State,
                              TransactionDebit = Convert.ToDecimal(transactionLiteApiDto.ContestFee),
                              TransactionId = payPalResponseLiteDto.Id,
                              TokenCredit = transactionLiteApiDto.ContestFee == 10 ? 100 :
                                          transactionLiteApiDto.ContestFee == 85 ? 1000 : 5000,
                              TextMessage = "Approved For CowBoy Coins Purchased",
                              TransactionType = (byte)Enums.TransactionType.Paypal,
                              UserId = userId,
                              Description = "CowBoy Coins purchased successfully"
                         };
                         return transactionDto;
                         #endregion
               }
          }
     }
}
