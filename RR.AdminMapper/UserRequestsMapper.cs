using RR.Data;
using RR.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RR.AdminMapper
{
   public class UserRequestsMapper
    {
        /// <summary>
        /// User Requests to UserRequestsDto
        /// </summary>
        /// <param name="news">List Of UserRequests</param>
        /// <returns>List Of UserRequestsDto</returns>
        public static IEnumerable<UserRequestsDto> Map(IEnumerable<UserRequests> requests)
        {
            return requests.Select(p => MapDto(p));
        }

        /// <summary>
        /// Map Dto
        /// </summary>
        /// <param name="newsDto">The UserRequests</param>
        /// <returns>The UserRequestsDto</returns>
        public static UserRequestsDto MapDto(UserRequests requests)
        {
            return new UserRequestsDto
            {
                Id = requests.Id,
                IsApproved = requests.IsApproved,
                CreatedBy = requests.CreatedBy,
                CreatedDate = requests.CreatedDate,
                IsDelete= requests.IsDelete,
                Message = requests.Message,
                RequestNo = requests.RequestNo,
                ReturlUrl = requests.ReturlUrl,
                Title = requests.Title,
                UserId = requests.UserId,
                LongTermTeamId = Convert.ToInt32(requests.LongTermTeamId),
                RequestMessage = requests.RequestMessage
            };
        }

        /// <summary>
        /// User Requests to UserRequestsLiteDto
        /// </summary>
        /// <param name="news">List Of UserRequests</param>
        /// <returns>List Of UserRequestsLiteDto</returns>
        public static IEnumerable<UserRequestsLiteDto> MapLite(IEnumerable<UserRequests> requests)
        {
            return requests.Select(p => MapLiteDto(p));
        }

        /// <summary>
        /// Map Lite Dto
        /// </summary>
        /// <param name="newsDto">The UserRequests</param>
        /// <returns>The UserRequestsLiteDto</returns>
        public static UserRequestsLiteDto MapLiteDto(UserRequests requests)
        {
            return new UserRequestsLiteDto
            {
                Id = requests.Id,
                IsApproved = requests.IsApproved,
                Message = requests.Message,
                RequestNo = requests.RequestNo,
                ReturlUrl = requests.ReturlUrl,
                Title = requests.Title,
                UserId = requests.UserId,
                LongTermTeamId = Convert.ToInt32(requests.LongTermTeamId),
                TeamBrand = requests.LongTermTeam?.TeamBrand,
                TeamIcon = requests.LongTermTeam?.TeamBrand,
                UserName = requests.User?.UserName,
                RequestMessage = requests.RequestMessage
            };
        }
    }
}
