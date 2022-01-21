using RR.Data;
using RR.Dto;
using System.Collections.Generic;
using System.Linq;

namespace RR.AdminMapper
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
                Address1 = !string.IsNullOrEmpty(user.Address1) ? user.Address1 : "",
                Address2 = !string.IsNullOrEmpty(user.Address2) ? user.Address2 : "",
                Address3 = !string.IsNullOrEmpty(user.Address3) ? user.Address3 : "",
                Avtar = !string.IsNullOrEmpty(user.Avtar) ? ("/images/profilePicture/" + user.Avtar) : "/images/RR/user-n.png",
                Banking = !string.IsNullOrEmpty(user.Banking) ? user.Banking : "",
                City = user.City,
                Country = user.Country,
                CreatedDate = user.CreatedDate,
                Email = !string.IsNullOrEmpty(user.Email) ? user.Email : "",
                FirstName = !string.IsNullOrEmpty(user.FirstName) ? user.FirstName : "",
                IsActive = user.IsActive,
                IsBlock = user.IsBlock.HasValue ? user.IsBlock.Value : false,
                IsDelete = user.IsDelete,
                LastName = !string.IsNullOrEmpty(user.LastName) ? user.LastName : "",
                LeagueNotification = user.LeagueNotification.HasValue ? user.LeagueNotification.Value : false,
                PhoneNumber = !string.IsNullOrEmpty(user.PhoneNumber) ? user.PhoneNumber : "",
                State = user.State,
                UpdatedDate = user.UpdatedDate,
                UserId = user.UserId,
                UserName = !string.IsNullOrEmpty(user.UserName) ? user.UserName : "",
                DOB = user.Dob,
                WalletToken = user.WalletToken.HasValue ? user.WalletToken.Value : 0,
                RegisteredOn = user.CreatedDate.ToString("MM/dd/yyyy"),
                PlayerType = user.PlayerType,
                ZipCode = user.ZipCode,
                ShopifyMembership = !string.IsNullOrEmpty(user.ShopifyMembership) ? user.ShopifyMembership : user.IsPaidMember == true ? "Premium" : "Basic",
                ShopifyCustomerId = user.ShopifyCustomerId
            };
        }
    }
}
