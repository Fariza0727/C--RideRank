using RR.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.StaticService
{
     public interface IBullService : IDisposable
     {
          /// <summary>
          /// Add Edit Bulls
          /// </summary>
          /// <param name="bullDto">List Of BullDto</param>
          /// <returns></returns>
          void AddEditBulls(BullDto bullDto);

          /// <summary>
          /// Get All Bulls
          /// </summary>
          /// <param name="start">The Start Page</param>
          /// <param name="length">The Page Size</param>
          /// <param name="searchStr">The Search Keyword</param>
          /// <param name="sort">The Order of page</param>
          /// <returns>List Of 10 Bulls Along</returns>
          Task<Tuple<IEnumerable<BullDto>, int>> GetAllBulls(int start, int length, int column, string searchStr = "", string sort = "");
     }
}
