using RR.Data;
using RR.Dto;
using System.Collections.Generic;
using System.Linq;

namespace RR.Mapper
{
    public static class UserDetailMapper
    {
        /// <summary>
        /// Map User Detail To UserDetailDto
        /// </summary>
        /// <param name="users">List Of UserDetail</param>
        /// <returns>List Of UserDetailDto</returns>
        public static IEnumerable<UserDetailDto> Map(IEnumerable<UserDetail> users)
        {
            return users.Select(p => MapDto(p));
        }

        /// <summary>
        /// Map Dto
        /// </summary>
        /// <param name="user">The User Detail</param>
        /// <returns>The UserDetailDto</returns>
        public static UserDetailDto MapDto(UserDetail user)
        {
            return new UserDetailDto
            {
                Id = user.Id,
                Address1 = !string.IsNullOrEmpty(user.Address1) ? user.Address1 : "N/A",
                Address2 = !string.IsNullOrEmpty(user.Address2) ? user.Address2 : "N/A",
                Address3 = !string.IsNullOrEmpty(user.Address3) ? user.Address3 : "",
                Avtar = !string.IsNullOrEmpty(user.Avtar) ? (user.Avtar.Contains("https://") ? user.Avtar : (user.Avtar != "/images/RR/user-n.png" ? ("/images/profilePicture/" + user.Avtar) : "/images/RR/user-n.png")) : "/images/RR/user-n.png",
                Banking = !string.IsNullOrEmpty(user.Banking) ? user.Banking : "",
                City = user.City,
                Country = user.Country,
                CreatedDate = user.CreatedDate,
                Email = !string.IsNullOrEmpty(user.Email) ? user.Email : "N/A",
                FirstName = !string.IsNullOrEmpty(user.FirstName) ? user.FirstName : "N/A",
                IsActive = user.IsActive,
                IsBlock = user.IsBlock.HasValue ? user.IsBlock.Value : false,
                IsDelete = user.IsDelete,
                LastName = !string.IsNullOrEmpty(user.LastName) ? user.LastName : "",
                LeagueNotification = user.LeagueNotification.HasValue ? user.LeagueNotification.Value : false,
                IsNotifyEmail = user.IsNotifyEmail.HasValue ? user.IsNotifyEmail.Value : false,
                IsNotifySms = user.IsNotifySms.HasValue ? user.IsNotifySms.Value : false,
                PhoneNumber = !string.IsNullOrEmpty(user.PhoneNumber) ? user.PhoneNumber : "N/A",
                OptPhoneno = !string.IsNullOrEmpty(user.OptPhoneNumber) ? user.OptPhoneNumber : "N/A",
                State = user.State,
                UpdatedDate = user.UpdatedDate,
                UserId = user.UserId,
                UserName = !string.IsNullOrEmpty(user.UserName) ? user.UserName : "N/A",
                DOB = user.Dob,
                WalletToken = user.WalletToken ?? 0,
                PlayerType = (user.User.AspNetUserRoles.Count > 0 ?
                 user.User.AspNetUserRoles.FirstOrDefault().Role.NormalizedName : ""),
                PlayerTypeId = (user.User.AspNetUserRoles.Count > 0 ?
                 user.User.AspNetUserRoles.FirstOrDefault().Role.Id : ""),
                ZipCode = user.ZipCode,
                IsPaidMember = user.IsPaidMember,
                IsOnline = user.IsUserOnline,
                ShopifyCustomerId = user.ShopifyCustomerId,
                ShopifyMembership = user.ShopifyMembership,
                TeamName = user.TeamName,
                ReferralCode = user.ReferralCode
            };

        }
        public static UserLiteDto MapLiteDto(UserDetail user)
        {
            return new UserLiteDto
            {
                Email = user.Email,
                UserName = user.UserName
            };
        }
        public static UserSubscriptionDto MapSubscriptionDto(UserDetail user)
        {
            return new UserSubscriptionDto
            {
                PaymentMode = user.PaymentMode,
                PlayerType = user.PlayerType
            };

        }
    }
}
