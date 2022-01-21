using RR.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.AdminService
{
     public interface IContestWinnerService : IDisposable
     {
          /// <summary>
          /// Get all contest Winners by contest id.
          /// </summary>
          /// <param name="ContestId"></param>
          /// <returns></returns>
          Task<IEnumerable<ContestWinnerDto>> GetContestWinner(long ContestId = 0);
          Task AddEditWinners(ContestAwardDto contestDto, string userId);
          Task AddEditWinnerMTT(ContestAwardDto contestDto, string userId);
          Task DeleteContestWinner(long id);
          Task DeleteContestWinnerByContestId(long id, bool isNonPaidMember = false);
     }
}
