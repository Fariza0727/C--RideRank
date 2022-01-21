using RR.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.Service
{
     public interface IRiderService : IDisposable
     {
          /// <summary>
          /// Get All Riders
          /// </summary>
          /// <param name="start">The Start Page</param>
          /// <param name="length">The Page Size</param>
          /// <param name="searchStr">The Search Keyword</param>
          /// <param name="sort">The Order of page</param>
          /// <returns>List Of 10 Riders Along</returns>
          Task<Tuple<IEnumerable<RiderDto>, int>> GetAllRiders(int start, int length, int column, string searchStr = "", string sort = "");

        Task<Tuple<IEnumerable<RiderDto>, int>> GetAllRidersShort(int start, int length, int column, string searchStr = "", string sort = "");

        Task<Tuple<IEnumerable<RiderEventDto>, int, decimal?>> GetAllRiderEvents(int riderid, int start, int length, int column, string searchStr = "", string sort = "");
            /// <summary>
            /// Get complete riders
            /// </summary>
            /// <param name="userId"></param>
            /// <returns></returns>
        Task<IEnumerable<RiderDto>> GetCompleteRiders(string userId = "");

          /// <summary>
          /// Get Rider By Id
          /// </summary>
          /// <param name="id">A Rider Id</param>
          /// <returns>The RiderDto</returns>
          Task<RiderDto> GetRiderById(int id);
        Task<string> UpdateRedisCache();

        Task<string> GetRiderPic(int riderId, string basePath = "");
     }
}
