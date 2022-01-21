using Microsoft.EntityFrameworkCore;
using RR.Data;
using RR.Dto;
using RR.Mapper;
using RR.Repo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.Service
{
    public class TransactionService : ITransactionService
    {
        private readonly IRepository<Transaction, RankRideContext> _repoTransaction;
        private readonly IRepository<JoinedContest, RankRideContext> _repoJoinedContest;

        public TransactionService(IRepository<Transaction, RankRideContext> repoTransaction,
             IRepository<JoinedContest, RankRideContext> repoJoinedContest)
        {
            _repoTransaction = repoTransaction;
            _repoJoinedContest = repoJoinedContest;
        }

        public async Task InsertTransactionDetail(TransactionDto transactionDto, TransactionLiteDto joinedContest)
        {
            var transaction = new Transaction();
            transaction = new Transaction
            {
                UserId = transactionDto.UserId,
                AuthCode = transactionDto.AuthCode,
                ResponseMessage = transactionDto.ResponseMessage,
                TransactionDebit = transactionDto.TransactionDebit,
                TransactionId = transactionDto.TransactionId,
                TransactionType = transactionDto.TransactionType,
                Description = transactionDto.Description,
                TransactionDate = DateTime.Now,
                IsActive = true
            };
            _repoTransaction.InsertGraph(transaction);

            var transactionId = transaction.Id;

            if (joinedContest != null && joinedContest.ContestId > 0)
            {
                var result = new JoinedContest
                {
                    ContestId = joinedContest.ContestId,
                    CreatedDate = DateTime.Now,
                    UserId = joinedContest.UserId,
                    IsActive = true,
                    IsDelete = false,
                    PaymentTxnId = transactionId,
                    TeamId = joinedContest.TeamId,
                    CreatedBy = joinedContest.UserId,
                    UpdatedBy = joinedContest.UserId
                };

                _repoJoinedContest.InsertGraph(result);
            }


            if (transactionDto.TokenCredit > 0)
            {
                var transactionNew = new Transaction
                {
                    UserId = transactionDto.UserId,
                    TransactionId = transactionDto.TransactionId,
                    TransactionType = 1,
                    TextMessage = transactionDto.TextMessage,
                    TokenCredit = transactionDto.TokenCredit,
                    TransactionDate = DateTime.Now,
                    IsActive = true,
                    Description = string.Empty
                };
                await _repoTransaction.InsertAsync(transactionNew);
            }
        }

        public async Task<IEnumerable<UserTransactionDto>> TransactionHistory(string userId)
        {
            var transactionHistory = _repoTransaction.Query()
                 .Filter(x => x.UserId == userId)
                 .Includes(y => y.Include(u => u.User)
                 .ThenInclude(ud => ud.UserDetail))
                 .Get();

            return await Task.FromResult(TransactionMapper.Map<UserTransactionDto>(transactionHistory));
        }

        public void Dispose()
        {
            if (_repoTransaction != null)
            {
                _repoTransaction.Dispose();
            }
            if (_repoJoinedContest != null)
            {
                _repoJoinedContest.Dispose();
            }
        }
    }
}
