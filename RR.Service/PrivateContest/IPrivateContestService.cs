using RR.Dto;
using System;
using System.Threading.Tasks;

namespace RR.Service
{
     public interface IPrivateContestService : IDisposable
     {
          /// <summary>
          /// Add/Edit private contest. 
          /// </summary>
          /// <param name="privateContestDto">PrivateContestDto</param>
          /// <returns></returns>
          Task<PrivateContestDto> AddEditContest(PrivateContestDto privateContestDto);
     }
}
