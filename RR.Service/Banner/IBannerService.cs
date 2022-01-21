using RR.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.Service
{
    public interface IBannerService : IDisposable
    {
        /// <summary>
        /// return all partner banners
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<BannerDto>> GetAllBannersRecords();
        /// <summary>
        /// return all partner banners
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<SponsorDto>> GetAllSponserRecords();
    }
}
