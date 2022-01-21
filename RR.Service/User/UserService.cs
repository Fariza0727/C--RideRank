using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RR.Data;
using RR.Dto;
using RR.Mapper;
using RR.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RR.Service.User
{
    public class UserService : IUserService
    {
        #region Constructor

        private readonly IRepository<UserDetail, RankRideContext> _repoUserDetail;
        private readonly IRepository<AspNetRoles, RankRideContext> _repoRoles;
        private readonly IRepository<AspNetUsers, RankRideContext> _repoUsers;
        private readonly IRepository<ForgetPasswordRequest, RankRideContext> _repoForgetPasswordRequest;


        public UserService(IRepository<UserDetail, RankRideContext> repoUserDetail,
             IRepository<AspNetRoles, RankRideContext> repoRoles,
             IRepository<ForgetPasswordRequest, RankRideContext> repoForgetPasswordRequest,
             IRepository<AspNetUsers, RankRideContext> repoUsers)
        {
            _repoUserDetail = repoUserDetail;
            _repoRoles = repoRoles;
            _repoForgetPasswordRequest = repoForgetPasswordRequest;
            _repoUsers = repoUsers;
        }

        #endregion

        /// <summary>
        /// Add Edit UserDetail
        /// </summary>
        /// <param name="registerDto">The RegisterDto</param>
        /// <returns>Empty</returns>
        public async Task AddEditUserDetail(UserDetailDto detailDto, bool isShopify = false)
        {
            var user = _repoUserDetail.Query()
                .Filter(o => o.UserId == detailDto.UserId)
                .Includes(x => x.Include(u => u.User))
                .Get()
                .SingleOrDefault();

            var roles = _repoRoles.Query();

            if (user == null)
            {
                user = new UserDetail
                {
                    Address1 = !string.IsNullOrEmpty(detailDto.Address1) ? detailDto.Address1 : "",
                    Address2 = !string.IsNullOrEmpty(detailDto.Address2) ? detailDto.Address2 : "",
                    Address3 = !string.IsNullOrEmpty(detailDto.Address3) ? detailDto.Address3 : "",
                    Avtar = !string.IsNullOrEmpty(detailDto.Avtar) ? (detailDto.Avtar == "/images/RR/user-n.png" ? "" : detailDto.Avtar) : "",
                    Banking = !string.IsNullOrEmpty(detailDto.Banking) ? detailDto.Banking : "",
                    City = detailDto.City,
                    Country = detailDto.Country,
                    Email = !string.IsNullOrEmpty(detailDto.Email) ? detailDto.Email : "",
                    FirstName = !string.IsNullOrEmpty(detailDto.FirstName) ? detailDto.FirstName : "",
                    IsActive = detailDto.IsActive,
                    IsBlock = detailDto.IsBlock,
                    IsDelete = detailDto.IsDelete,
                    LastName = !string.IsNullOrEmpty(detailDto.LastName) ? detailDto.LastName : "",
                    LeagueNotification = detailDto.LeagueNotification,
                    IsNotifyEmail = detailDto.IsNotifyEmail,
                    IsNotifySms = detailDto.IsNotifySms,
                    PhoneNumber = !string.IsNullOrEmpty(detailDto.PhoneNumber) ? detailDto.PhoneNumber : "",
                    OptPhoneNumber = !string.IsNullOrEmpty(detailDto.OptPhoneno) ? detailDto.OptPhoneno : "",
                    State = detailDto.State,
                    UpdatedDate = DateTime.Now,
                    UserId = detailDto.UserId,
                    UserName = detailDto.UserName,
                    CreatedDate = DateTime.Now,
                    Dob = detailDto.DOB,
                    SubscriptionExpiryDate = detailDto.SubscriptionExpiryDate,
                    WalletToken = detailDto.WalletToken.HasValue ? detailDto.WalletToken.Value : 0,
                    PlayerType = detailDto.PlayerType,
                    ZipCode = detailDto.ZipCode,
                    PaymentMode = detailDto.PaymentMode,
                    TeamName = detailDto.TeamName,
                    ReferralCode = detailDto.ReferralCode
                };
                if (isShopify)
                {
                    user.ShopifyMembership = detailDto.ShopifyMembership;
                    user.ShopifyCustomerId = detailDto.ShopifyCustomerId;
                    user.IsPaidMember = detailDto.IsPaidMember != null ? detailDto.IsPaidMember : user.IsPaidMember;
                }
                await _repoUserDetail.InsertGraphAsync(user);
            }
            else
            {
                user.Address1 = !string.IsNullOrEmpty(detailDto.Address1) ? detailDto.Address1 : "";
                user.Address2 = !string.IsNullOrEmpty(detailDto.Address2) ? detailDto.Address2 : "";
                user.Address3 = !string.IsNullOrEmpty(detailDto.Address3) ? detailDto.Address3 : "";
                user.Avtar = !string.IsNullOrEmpty(detailDto.Avtar) ? detailDto.Avtar : "";
                user.Banking = !string.IsNullOrEmpty(detailDto.Banking) ? detailDto.Banking : "";
                user.City = detailDto.City;
                user.Country = detailDto.Country;
                //user.Email = !string.IsNullOrEmpty(detailDto.Email) ? detailDto.Email : "";
                user.FirstName = !string.IsNullOrEmpty(detailDto.FirstName) ? detailDto.FirstName : "";
                user.IsActive = detailDto.IsActive;
                user.IsBlock = detailDto.IsBlock;
                user.IsDelete = detailDto.IsDelete;
                user.LastName = !string.IsNullOrEmpty(detailDto.LastName) ? detailDto.LastName : "";
                user.LeagueNotification = detailDto.LeagueNotification;
                user.PhoneNumber = !string.IsNullOrEmpty(detailDto.PhoneNumber) ? detailDto.PhoneNumber : "";
                user.OptPhoneNumber = !string.IsNullOrEmpty(detailDto.OptPhoneno) ? detailDto.OptPhoneno : "";
                user.State = detailDto.State;
                user.UpdatedDate = DateTime.Now;
                user.UserName = detailDto.UserName;
                user.Dob = detailDto.DOB;
                user.WalletToken = detailDto.WalletToken.HasValue ? detailDto.WalletToken.Value : 0;
                user.SubscriptionExpiryDate = (detailDto.SubscriptionExpiryDate != null ?
                     detailDto.SubscriptionExpiryDate : user.SubscriptionExpiryDate);
                user.PlayerType = detailDto.PlayerType;
                user.TeamName = detailDto.TeamName;
                user.ZipCode = detailDto.ZipCode;
                user.PaymentMode = !string.IsNullOrEmpty(detailDto.PaymentMode) ? detailDto.PaymentMode : "";
                user.IsNotifyEmail = detailDto.IsNotifyEmail;
                user.IsNotifySms = detailDto.IsNotifySms;
                user.ReferralCode = detailDto.ReferralCode;

                if (isShopify)
                {
                    user.ShopifyMembership = detailDto.ShopifyMembership;
                    user.ShopifyCustomerId = detailDto.ShopifyCustomerId;
                    user.IsPaidMember = detailDto.IsPaidMember != null ? detailDto.IsPaidMember : user.IsPaidMember;
                }
                await _repoUserDetail.UpdateAsync(user);
            }
        }

        /// <summary>
        /// Get UserDetail
        /// </summary>
        /// <param name="id">An Id</param>
        /// <returns>The UserDetailDto</returns>
        public async Task<UserDetailDto> GetUserDetail(string id)
        {
            var user = _repoUserDetail.Query()
                .Filter(o => o.UserId == id)
                .Includes(x => x.Include(u => u.User).ThenInclude(ur => ur.AspNetUserRoles).ThenInclude(r => r.Role))
                .Get()
                .FirstOrDefault();
            return await Task.FromResult(user != null ? UserDetailMapper.MapDto(user) : null);
        }

        public async Task<IEnumerable<DropDownDto>> GetRoles()
        {
            var roles = _repoRoles.Query().Get();
            return await Task.FromResult(UserRoleMapper.Map(roles));
        }

        public async Task<string> GetRolesById(string id)
        {
            var roleById = _repoRoles.Query()
                 .Filter(x => x.Id == id)
                 .Get()
                 .SingleOrDefault();

            return await Task.FromResult(roleById.NormalizedName);
        }

        public async Task DeleteForgetPasswordRequest(int forgetPasswordRequestId)
        {
            await _repoForgetPasswordRequest.DeleteAsync(forgetPasswordRequestId);
        }

        public async Task<ForgetPasswordRequestDto> GetForgetPasswordRequest(string code)
        {
            var forgetUserData = _repoForgetPasswordRequest
                 .Query()
                 .Filter(x => x.UrlId == code)
                 .Get()
                 .SingleOrDefault();

            var result = new ForgetPasswordRequestDto
            {
                Id = forgetUserData.Id,
                UrlId = forgetUserData.UrlId,
                UserId = forgetUserData.UserId
            };
            return await Task.FromResult(result);
        }

        public async Task<IdentityUser> GetUserId(string id)
        {
            var user = _repoUsers.Query()
                 .Filter(x => x.Id == id)
                 .Get()
                 .SingleOrDefault();

            var userData = new IdentityUser
            {
                UserName = user.Email,
                Email = user.Email,
                SecurityStamp = user.SecurityStamp,
                AccessFailedCount = user.AccessFailedCount,
                ConcurrencyStamp = user.ConcurrencyStamp,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd,
                NormalizedEmail = user.NormalizedEmail,
                NormalizedUserName = user.NormalizedUserName,
                PasswordHash = user.PasswordHash,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled
            };
            return await Task.FromResult(userData);
        }

        public async Task<UserSubscriptionDto> GetCurrentUserSubscription(string userId)
        {
            var userdetail = _repoUserDetail.Query()
                 .Filter(x => x.UserId == userId)
                 .Get()
                 .SingleOrDefault();
            return await Task.FromResult(UserDetailMapper.MapSubscriptionDto(userdetail));
        }

        public async Task<UserLiteDto> UserExistence(string user)
        {
            var userExists = _repoUserDetail.Query()
                 .Filter(x => x.Email == user || x.UserName == user)
                 .Get()
                 .FirstOrDefault();

            if (userExists != null)
            {
                return await Task.FromResult(new UserLiteDto { Email = userExists.Email, UserName = userExists.UserName });
            }
            return await Task.FromResult(new UserLiteDto() { });
        }



        /// <summary>
        /// Dispose UserDetail Service
        /// </summary>
        public void Dispose()
        {
            if (_repoUserDetail != null)
            {
                _repoUserDetail.Dispose();
            }
            if (_repoRoles != null)
            {
                _repoRoles.Dispose();
            }
            if (_repoForgetPasswordRequest != null)
            {
                _repoForgetPasswordRequest.Dispose();
            }
            if (_repoUsers != null)
            {
                _repoUsers.Dispose();
            }

        }

        public async Task UpdateLogedInStatus(string userId, bool isLogedin)
        {
            var user_ = _repoUserDetail.Query().Filter(d => d.UserId == userId).Get().SingleOrDefault();
            if (user_ != null)
            {
                user_.IsUserOnline = isLogedin;
                await _repoUserDetail.UpdateAsync(user_);
            }
        }

        public async Task<string> GetRealReferralCode(string customCode)
        {
            var user_ = _repoUserDetail.Query().Filter(d => d.CustomReferralCode == customCode).Get().SingleOrDefault();
            if (user_ != null)
            {
                return await Task.FromResult(user_.ReferralCode);
            }
            else
            {
                return await Task.FromResult("");
            }
        }

        public async Task UpdateReferredCustomers(string referralCode)
        {
            var user_ = _repoUserDetail.Query().Filter(d => d.ReferralCode == referralCode).Get().SingleOrDefault();
            if (user_ != null)
            {
                if(user_.ReferredCustomers == null)
                {
                    user_.ReferredCustomers = 1;
                }
                else
                {
                    user_.ReferredCustomers = user_.ReferredCustomers + 1;
                }
                
                await _repoUserDetail.UpdateAsync(user_);
            }
        }

        public Task<bool> IsPaidMember(string AspUserId)
        {
            var user_ = _repoUserDetail.Query().Filter(r => r.UserId == AspUserId).Get().SingleOrDefault();
            if (user_ != null)
                return Task.FromResult(Convert.ToBoolean(user_.IsPaidMember));

            return Task.FromResult(false);

        }

        public Task<bool> IsPaidMember(ClaimsPrincipal User)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user_ = _repoUserDetail.Query().Filter(r => r.UserId == userId).Get().SingleOrDefault();
            if (user_ != null)
                return Task.FromResult(Convert.ToBoolean(user_.IsPaidMember));

            return Task.FromResult(false);

        }

        public async Task DeleteCustomerUserDetail(long? customerId)
        {
            var customer_ = _repoUserDetail.Query().Filter(r => r.ShopifyCustomerId == customerId).Get().SingleOrDefault();
            if (customer_ != null)
            {
                await _repoUserDetail.DeleteAsync(customer_);
                await _repoUsers.DeleteAsync(customer_.UserId);
                
            }
        }

        public Task<bool> IsUserExist(string username, string userid = "")
        {
            if (!string.IsNullOrEmpty(userid))
            {
                var user = _repoUserDetail.Query().Filter(r => r.UserId.Trim().ToUpper() != userid.Trim().ToUpper() && r.UserName.ToUpper().Trim() == username.ToUpper().Trim()).Get().Count();
                return Task.FromResult(user > 0);
            }
            else
            {
                var user = _repoUserDetail.Query().Filter(r => r.UserName.ToUpper().Trim() == username.ToUpper().Trim()).Get().Count();
                return Task.FromResult(user > 0);
            }
            
        }

        public async Task<List<IdentityUser>> GetNoActivatedEmailUsers()
        {
            var userList = _repoUsers.Query()
                 .Filter(x => x.EmailConfirmed == false)
                 .Get()
                 .Select(user => new IdentityUser {
                     UserName = user.Email,
                     Email = user.Email,
                     SecurityStamp = user.SecurityStamp,
                     AccessFailedCount = user.AccessFailedCount,
                     ConcurrencyStamp = user.ConcurrencyStamp,
                     EmailConfirmed = user.EmailConfirmed,
                     LockoutEnabled = user.LockoutEnabled,
                     LockoutEnd = user.LockoutEnd,
                     NormalizedEmail = user.NormalizedEmail,
                     NormalizedUserName = user.NormalizedUserName,
                     PasswordHash = user.PasswordHash,
                     PhoneNumber = user.PhoneNumber,
                     PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                     TwoFactorEnabled = user.TwoFactorEnabled
                 }).ToList();

            return await Task.FromResult(userList);
        }
    }
}
