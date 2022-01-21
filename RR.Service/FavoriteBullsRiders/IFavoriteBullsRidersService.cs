using RR.Dto;
using System;
using System.Threading.Tasks;

namespace RR.Service
{
    public interface IFavoriteBullsRidersService : IDisposable
    {
        /// <summary>
        /// Add Rider as Favorite 
        /// </summary>
        /// <param name="favoriteBullRidersDto">FavoriteBullRiders Dto</param>
        /// <returns></returns>
        Task AddRiderAsFavorite(FavoriteBullRidersDto favoriteBullRidersDto);

        /// <summary>
        /// Add Bull as Favorite 
        /// </summary>
        /// <param name="favoriteBullRidersDto">FavoriteBullRiders Dto</param>
        /// <returns></returns>
        Task AddBullAsFavorite(FavoriteBullRidersDto favoriteBullRidersDto);

        /// <summary>
        /// Check rider added as Favorite
        /// </summary>
        /// <param name="AspNetUserId">AspNet UserId</param>
        /// <param name="riderId">rider Id</param>
        /// <returns>True/False</returns>
        Task<bool> IsRiderAdded(string AspNetUserId, int riderId);

        /// <summary>
        /// Check bull added as Favorite
        /// </summary>
        /// <param name="AspNetUserId">AspNet UserId</param>
        /// <param name="bullId">bull Id</param>
        /// <returns>True/False</returns>
        Task<bool> IsBullAdded(string AspNetUserId, int bullId);

        /// <summary>
        /// Get User Favorite Bulls and Riders
        /// </summary>
        /// <param name="AspNetUserId">AspNet User Id</param>
        /// <returns>UserFavoriteBullsRidersDto</returns>
        Task<UserFavoriteBullsRidersDto> GetUserFavoriteBullsRiders(string AspNetUserId);

        /// <summary>
        /// Remove User Favorite Bulls
        /// </summary>
        /// <param name="AspNetUserId">AspNet User Id</param>
        /// <param name="bullId">Bull Id</param>
        /// <returns></returns>
        Task RemoveUserFavoriteBulls(string AspNetUserId, int bullId);

        /// <summary>
        /// Remove User Favorite Riders
        /// </summary>
        /// <param name="AspNetUserId">AspNet User Id</param>
        /// <param name="riderId">Rider Id</param>
        /// <returns></returns>
        Task RemoveUserFavoriteRider(string AspNetUserId, int riderId);
    }
}
