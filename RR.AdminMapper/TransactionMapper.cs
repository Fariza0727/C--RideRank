using RR.Data;
using RR.Dto;
using RR.StaticData;
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
               return (T)Convert.ChangeType(MapUserTransactionDto(transaction), typeof(T));
          }

   

        public static UserTransactionDto MapUserTransactionDto(Transaction transaction)
        {
            return new UserTransactionDto
            {
                CurrentWallet = (transaction.User.UserDetail.FirstOrDefault(x => x.UserId == transaction.UserId) != null ?
                 transaction.User.UserDetail.FirstOrDefault(x => x.UserId == transaction.UserId).WalletToken : 0),
                ResponseMessage = transaction.TextMessage,
                TransactionAmount = Convert.ToDecimal(transaction.TransactionDebit),
                TransactionDate = transaction.TransactionDate,
                UserName = (transaction.User.UserDetail.FirstOrDefault(x => x.UserId == transaction.UserId) != null ?
                 transaction.User.UserDetail.FirstOrDefault(x => x.UserId == transaction.UserId).UserName : "")
            };
        }
    }
}
