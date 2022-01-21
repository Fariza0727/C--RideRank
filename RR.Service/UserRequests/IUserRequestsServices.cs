using RR.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RR.Service
{
   public interface IUserRequestsServices : IDisposable
    {
        /// <summary>
        /// Sent request to admin
        /// </summary>
        /// <param name="requestsDto"></param>
        /// <returns>UserRequestsDto</returns>
        Task<UserRequestsDto> SentRequest(UserRequestsDto requestsDto);

        /// <summary>
        /// Sent request to admin
        /// </summary>
        /// <param name="AspUserId"></param>
        /// <returns>UserRequestsDto</returns>
        Task<IEnumerable<UserRequestsDto>> GetRequests(string AspUserId);
    }
}
