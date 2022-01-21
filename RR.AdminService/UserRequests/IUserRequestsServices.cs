using RR.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RR.AdminService
{
   public interface IUserRequestsServices : IDisposable
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="column"></param>
        /// <param name="searchStr"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        Task<Tuple<IEnumerable<UserRequestsLiteDto>, int>> GetAllRequests(int start, int length, int column, string searchStr = "", string sort = "");

        /// <summary>
        /// Get Request 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<UserRequestsLiteDto> GetRequest(long Id);

        /// <summary>
        /// Update Request
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>UserRequestsLiteDto</returns>
        Task<UserRequestsLiteDto> UpdateRequest(long Id);

    }
}
