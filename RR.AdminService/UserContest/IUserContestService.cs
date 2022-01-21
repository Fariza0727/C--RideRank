using RR.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RR.AdminService
{
     public interface IUserContestService : IDisposable
     {
          /// <summary>
          /// Get all contests by event id.
          /// </summary>
          /// <param name="start"></param>
          /// <param name="length"></param>
          /// <param name="searchStr"></param>
          /// <param name="sort"></param>
          /// <param name="eventId"></param>
          /// <param name="column"></param>
          /// <returns></returns>
          Task<Tuple<List<UserContestDto>, int>> GetAlluserbyContest(int start, int length, int column, string searchStr = "", string sort = "", long contestId = 0);

          /// <summary>
          /// returns all contest winners by contest id
          /// </summary>
          /// <param name="start"></param>
          /// <param name="length"></param>
          /// <param name="column"></param>
          /// <param name="searchStr"></param>
          /// <param name="sort"></param>
          /// <param name="contestId"></param>
          /// <returns></returns>
          Task<IEnumerable<UserContestWinnerDto>> GetContestWinnerUsers(long contestId = 0);


     }
}
