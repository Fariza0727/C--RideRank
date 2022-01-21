using RR.Dto;
using System;
using System.Threading.Tasks;

namespace RR.Service
{
     public interface IContestWinnerService : IDisposable
     {
          /// <summary>
          /// Get all contest Winners by contest id.
          /// </summary>
          /// <param name="ContestId"></param>
          /// <returns></returns>    
          Task AddEditWinners(ContestAwardDto contestDto);
     }
}
