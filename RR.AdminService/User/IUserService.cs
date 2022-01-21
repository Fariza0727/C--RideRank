using RR.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.AdminService
{
    public interface IUserService : IDisposable
    {
        /// <summary>
        /// Add Edit UserDetail
        /// </summary>
        /// <param name="detailDto">The UserDetailDto</param>
        /// <returns></returns>
        Task AddEditUserDetail(UserDetailDto registerDto);

        /// <summary>
        /// Get User Detail
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>User DetailDto</returns>
        Task<UserDetailDto> GetUserDetail(int id);

        /// <summary>
        /// Get All Exist Users
        /// </summary>
        /// <param name="start">PageNumber</param>
        /// <param name="length">Number Of Record On Single Page</param>
        /// <param name="searchStr">Search string</param>
        /// <param name="column">Column Index</param>
        /// <param name="sort">Order</param>
        /// <returns>List Of UserDetailDto</returns>
        Task<Tuple<IEnumerable<UserLiteDetailDto>, int>> GetAllExistUsers(int start, int length, int column, string searchStr = "", string sort = "");

        /// <summary>
        /// Update User Status
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns></returns>
        Task UpdateStatus(int id);

        /// <summary>
        /// Add Forget Password Request
        /// </summary>
        /// <returns></returns>
        Task AddForgetPasswordRequest(Guid code, string userId);
        /// <summary>
        /// Unlock user
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<string> Updateuserlockstatus(string userName);

        /// <summary>
        /// Get User Detail By Email
        /// </summary>
        /// <param name="emailId"></param>
        /// <returns></returns>
        Task<UserDetailDto> GetUserDetailbyEmail(string emailId);

        /// <summary>
        /// Delete User
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeleteUserByID(long id);

        /// <summary>
        /// update user role
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="Role"></param>
        /// <returns></returns>
        Task<string> UpdateUserRole(string UserId, string Role);

        /// <summary>
        /// Get users as card
        /// </summary>
        /// <returns>Tuple<int, int> item1:total, item2:active</returns>
        Task<Tuple<int, int>> GetUsersAsCard();

        /// <summary>
        /// Get users which added in one week 
        /// </summary>
        /// <returns>UserChartDto</returns>
        Task<UserChartDto> GetWeeklyUsersAsCard();

    }
}
