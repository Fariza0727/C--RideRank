using RR.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.AdminService
{
    public interface IBannerService : IDisposable
    {
        /// <summary>
        /// Add Update Banner Detail
        /// </summary>
        /// <param name="bannerDto"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task AddEditBanner(BannerDto bannerDto, string userId);
        /// <summary>
        /// Get All Banners
        /// </summary>
        /// <param name="start">Page Number</param>
        /// <param name="length">Number Of Record </param>
        /// <param name="searchStr"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        Task<Tuple<IEnumerable<BannerDto>, int>> GetAllBannersRecords(int start, int length, int column, string searchStr = "", string sort = "");
        /// <summary>
        /// Get Banner By Id
        /// </summary>
        /// <param name="Id">Banner Id</param>
        /// <returns>The BannerDto</returns>
        Task<BannerDto> GetBannerById(int Id);
        /// <summary>
        /// Delete Banner By Id
        /// </summary>
        /// <param name="Id">Banner Id</param>
        /// <returns></returns>
        void DeleteBanner(int Id);
    }
}
