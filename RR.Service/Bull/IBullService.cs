using RR.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.Service
{
     public interface IBullService : IDisposable
     {
          /// <summary>
          /// Get All Bulls
          /// </summary>
          /// <param name="start">The Start Page</param>
          /// <param name="length">The Page Size</param>
          /// <param name="searchStr">The Search Keyword</param>
          /// <param name="sort">The Order of page</param>
          /// <returns>List Of 10 Bulls Along</returns>
          Task<Tuple<IEnumerable<BullDto>, int>> GetAllBulls(int start, int length, int column, string searchStr = "", string sort = "");

        Task<Tuple<IEnumerable<BullEventDto>, int>> GetBullEvents(int bullId,int start, int length, int column, string searchStr = "", string sort = "");

        /// <summary>
        /// Get all bulls
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IEnumerable<BullDto>> GetCompleteBulls(string userId = "");

          /// <summary>
          /// Get Bull Record By Id
          /// </summary>
          /// <param name="id">A Bull Id</param>
          /// <returns>The BullDto</returns>
          Task<BullDto> GetBullById(int id);

          /// <summary>
          /// Get Bull Record By Id
          /// </summary>
          /// <param name="id">A DB Id</param>
          /// <returns>The BullDto</returns>
          Task<BullDto> GetBullRecordById(int id);
        Task<string> UpdateRedisCache();
        Task<string> GetBullPic(int bullId, string baseUrl = "");
    }
}
