using Microsoft.EntityFrameworkCore;
using RR.Core;
using RR.Data;
using RR.Dto;
using RR.Mapper;
using RR.Repo;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RR.AdminService
{
    public class TransactionService : ITransactionService
    {
        #region Constructor
        private readonly IRepository<Transaction, RankRideContext> _repoTransaction;
        private readonly IRepository<JoinedContest, RankRideContext> _repoJoinedContest;

        public TransactionService(IRepository<Transaction, RankRideContext> repoTransaction,
             IRepository<JoinedContest, RankRideContext> repoJoinedContest)
        {
            _repoTransaction = repoTransaction;
            _repoJoinedContest = repoJoinedContest;
        }
        #endregion
        /// <summary>
        /// Get All Transaction Details
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="column"></param>
        /// <param name="searchStr"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public async Task<Tuple<IEnumerable<UserTransactionDto>, int>> GetAllTransactionHistory(int start, int length, int column, string searchStr = "", string sort = "")
        {
            int count = 0;
            // var predicate = PredicateBuilder.True<Transaction>()
            //.And(x => (searchStr == "" || x.User.UserDetail.Select(y => y.UserName).ToString().Contains(searchStr.ToLower())))
            //.And(x => (searchStr == "" || x.User.UserDetail.Select(y => y.WalletToken).ToString().Contains(searchStr.ToLower())));

            var transactionHistory = _repoTransaction.Query()
               .Includes(y => y.Include(u => u.User)
                 .ThenInclude(ud => ud.UserDetail));

            transactionHistory = transactionHistory.Filter(
                x => ( x.User.UserDetail.FirstOrDefault().UserName.Contains(searchStr.ToLower()) ||
                x.TransactionDebit.ToString().Contains(searchStr.ToLower()) ||
                x.ResponseMessage.Contains(searchStr.ToLower()) || 
                x.TransactionDate.ToString().Contains(searchStr.ToLower()) ||
                x.TokenCredit.ToString().Contains(searchStr.ToLower()) ||
                x.Description.Contains(searchStr.ToLower()) ||
                x.TextMessage.Contains(searchStr.ToLower()) ||
                x.User.UserDetail.FirstOrDefault().WalletToken.ToString().Contains(searchStr.ToLower())
                )
              );

            switch (column)
            {
                case 0:
                    transactionHistory = (sort == "desc" ? transactionHistory.OrderBy(x => x.OrderByDescending(xx => xx.User.UserDetail.FirstOrDefault().UserName)) : transactionHistory.OrderBy(x => x.OrderBy(xx => xx.User.UserDetail.FirstOrDefault().UserName)));
                    break;
                case 1:
                    transactionHistory = (sort == "desc" ? transactionHistory.OrderBy(x => x.OrderByDescending(xx => xx.TransactionDebit)) : transactionHistory.OrderBy(x => x.OrderBy(xx => xx.TransactionDebit)));
                    break;
                case 2:
                    transactionHistory = (sort == "desc" ? transactionHistory.OrderBy(x => x.OrderByDescending(xx => xx.ResponseMessage)) : transactionHistory.OrderBy(x => x.OrderBy(xx => xx.ResponseMessage)));
                    break;
                case 3:
                    transactionHistory = (sort == "desc" ? transactionHistory.OrderBy(x => x.OrderByDescending(xx => xx.TransactionDate)) : transactionHistory.OrderBy(x => x.OrderBy(xx => xx.TransactionDate)));
                    break;
                case 4:
                    transactionHistory = (sort == "desc" ? transactionHistory.OrderBy(x => x.OrderByDescending(xx => xx.TokenCredit)) : transactionHistory.OrderBy(x => x.OrderBy(xx => xx.TokenCredit)));
                    break;
                case 5:
                    transactionHistory = (sort == "desc" ? transactionHistory.OrderBy(x => x.OrderByDescending(xx => xx.Description)) : transactionHistory.OrderBy(x => x.OrderBy(xx => xx.Description)));
                    break;
                case 6:
                    transactionHistory = (sort == "desc" ? transactionHistory.OrderBy(x => x.OrderByDescending(xx => xx.User.UserDetail.FirstOrDefault().WalletToken)) : transactionHistory.OrderBy(x => x.OrderBy(xx => xx.User.UserDetail.FirstOrDefault().WalletToken)));
                    break;
                default:
                    transactionHistory = (sort == "desc" ? transactionHistory.OrderBy(x => x.OrderByDescending(xx => xx.TransactionDate)) : transactionHistory.OrderBy(x => x.OrderBy(xx => xx.TransactionDate)));
                    break;
            }
            var transaction = (new Tuple<IEnumerable<UserTransactionDto>, int>(transactionHistory.GetPage(start, length, out count).Select(item => new UserTransactionDto
            {
                UserName = GetUserName(item.User),
                TransactionAmount = item.TransactionDebit,              
                ResponseMessage = string.IsNullOrEmpty(item.Description) ? item.TextMessage : item.Description,
                date = item.TransactionDate.ToShortDateString(),
                TokenCredit = item.TokenCredit,
                CurrentWallet=item.User.UserDetail.FirstOrDefault()?.WalletToken??0
            }), count));

             return await Task.FromResult(transaction);
        }
        /// <summary>
        /// Dispose User Service
        /// </summary>
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

        private string GetUserName(Data.AspNetUsers user)
        {
            string username = "";

            if (user == null)
                return username;

            if (user.UserDetail?.Count > 0)
                username = user.UserDetail.FirstOrDefault().UserName;

            if (string.IsNullOrEmpty(username))
                username = user.UserName.Split('@')[0];

            return username;
        }
    }
}
