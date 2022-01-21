using RR.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.Service
{
     public interface ITransactionService : IDisposable
     {
          Task InsertTransactionDetail(TransactionDto transactionDto, TransactionLiteDto joinedContest);
          Task<IEnumerable<UserTransactionDto>> TransactionHistory(string userId);
     }
}