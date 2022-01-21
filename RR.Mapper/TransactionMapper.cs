using RR.Data;
using RR.Dto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RR.Mapper
{
     public class TransactionMapper
     {
          public static IEnumerable<T> Map<T>(IEnumerable<Transaction> transactions)
          {
               return transactions.Select(p => Map<T>(p));
          }

          public static T Map<T>(Transaction transaction)
          {
               if (typeof(T) == typeof(TransactionDto))
               {
                    return (T)Convert.ChangeType(Map(transaction), typeof(T));
               }
               if (typeof(T) == typeof(UserTransactionDto))
               {
                    return (T)Convert.ChangeType(MapUserTransactionDto(transaction), typeof(T));
               }
               return (T)Convert.ChangeType(Map(transaction), typeof(T));
          }

          public static TransactionDto Map(Transaction transaction)
          {
               return new TransactionDto
               {
                    AuthCode = transaction.AuthCode,
                    ResponseMessage = transaction.ResponseMessage,
                    TransactionDebit = Convert.ToDecimal(transaction.TransactionDebit),
                    TransactionId = transaction.TransactionId,
                    TransactionType = transaction.TransactionType,
                    Description = transaction.Description
               };
          }

          public static UserTransactionDto MapUserTransactionDto(Transaction transaction)
          {
               return new UserTransactionDto
               {
                    CurrentWallet = (transaction.User.UserDetail.FirstOrDefault(x => x.UserId == transaction.User.Id) != null ?
                    transaction.User.UserDetail.FirstOrDefault(x => x.UserId == transaction.User.Id).WalletToken : 0),
                    ResponseMessage = transaction.ResponseMessage,
                    TransactionAmount = Convert.ToDecimal(transaction.TransactionDebit),
                    TransactionDate = transaction.TransactionDate,
                    UserName = (transaction.User.UserDetail.FirstOrDefault(x => x.UserId == transaction.User.Id) != null ?
                    transaction.User.UserDetail.FirstOrDefault(x => x.UserId == transaction.User.Id).UserName : ""),
                    TransactionType = transaction.TransactionType,
                    TransactionId = transaction.TransactionId,
                    Avtar = !string.IsNullOrEmpty(transaction.User.UserDetail.FirstOrDefault(x => x.UserId == transaction.User.Id).Avtar) ? (transaction.User.UserDetail.FirstOrDefault(x => x.UserId == transaction.User.Id).Avtar != "/images/RR/user-n.png" ? ("https://rankridefantasy.com/images/profilePicture/" + transaction.User.UserDetail.FirstOrDefault(x => x.UserId == transaction.User.Id).Avtar) : "https://rankridefantasy.com/images/RR/user-n.png") : "https://rankridefantasy.com/images/RR/user-n.png"
               };
          }
     }
}
