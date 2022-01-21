using RR.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.Service
{
    public interface IContestUserWinnerService : IDisposable
    {
        /// <summary>
        /// save contest winner based on team rank
        /// </summary>
        /// <param name="userWinnerDto"></param>
        /// <returns></returns>
        Task SaveContestWinner(List<ContestUserWinnerDto> userWinnerDto);

        /// <summary>
        /// Get users winnings
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<ContestUserWinnerDto>> GetUserWinnings(string userId);

        /// <summary>
        /// Set user wallet token on winning.
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="contestId"></param>
        /// <param name="contestWinnerId"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        Task AddEditTokenInUserWallet(string UserID, long contestId, long contestWinnerId, string operation);

        /// <summary>
        /// Add/deduct token from wallet.
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="Token"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task UpdateUserWallet(string UserID, int Token, string type);
    }
}
