using RR.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.AdminService
{
     public interface ITransactionService : IDisposable
     {
        /// <summary>
        /// Get All Transaction Details
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="column"></param>
        /// <param name="searchStr"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        Task<Tuple<IEnumerable<UserTransactionDto>, int>> GetAllTransactionHistory(int start, int length, int column, string searchStr = "", string sort = "");
     }
}