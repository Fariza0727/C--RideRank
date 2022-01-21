using Microsoft.AspNetCore.Identity;
using RR.Dto;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RR.Service.User
{
    public interface IUserService : IDisposable
    {
        /// <summary>
        /// Add Edit UserDetail
        /// </summary>
        /// <param name="registerDto">The RegisterDto</param>
        /// <param name="isShopify">If Shopify</param>
        /// <returns>Empty</returns>
        Task AddEditUserDetail(UserDetailDto registerDto, bool isShopify = false);

        /// <summary>
        /// Get UserDetail
        /// </summary>
        /// <param name="id">An Id</param>
        /// <returns>The UserDetailDto</returns>
        Task<UserDetailDto> GetUserDetail(string id);

        Task<IdentityUser> GetUserId(string userId);

        Task<List<IdentityUser>> GetNoActivatedEmailUsers();

        Task<IEnumerable<DropDownDto>> GetRoles();

        Task<string> GetRolesById(string id);

        Task<UserLiteDto> UserExistence(string user);
        /// <summary>
        /// Update User logedin Status
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isLogedin"></param>
        /// <returns></returns>
        Task UpdateLogedInStatus(string userId, bool isLogedin);

        Task UpdateReferredCustomers(string referralCode);

        Task<string> GetRealReferralCode(string customCode);
        /// <summary>
        /// Delete Forget Password Request
        /// </summary>
        /// <param name="forgetPasswordRequest">Forget Password Request Id</param>
        /// <returns></returns>
        Task DeleteForgetPasswordRequest(int forgetPasswordRequestId);

        Task<ForgetPasswordRequestDto> GetForgetPasswordRequest(string code);

        Task<UserSubscriptionDto> GetCurrentUserSubscription(string userId);
        Task<bool> IsPaidMember(string AspUserId);
        Task<bool> IsPaidMember(ClaimsPrincipal User);
        Task DeleteCustomerUserDetail(long? customerId);
        Task<bool> IsUserExist(string username, string userid = "");
    }
}
