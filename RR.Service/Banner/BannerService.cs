using RR.AdminData;
using RR.Core;
using RR.Dto;
using RR.Repo;
using RR.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.Service
{
    public class BannerService : IBannerService
    {
        #region Constructor
        public readonly IRepository<Banner, RankRideAdminContext> _repoBanner;
        public readonly IRepository<Sponsor, RankRideAdminContext> _repoSponser;
        #endregion
        public BannerService(IRepository<Banner, RankRideAdminContext> repoBanner, IRepository<Sponsor, RankRideAdminContext> repoSponser)
        {
            _repoBanner = repoBanner;
            _repoSponser = repoSponser;
        }

        /// <summary>
        /// return all partner banners
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<BannerDto>> GetAllBannersRecords()
        {
            var banner = _repoBanner
                .Query()
                .Filter(x => !string.IsNullOrEmpty(x.PicPath))
                .Get();


            return await Task.FromResult(banner
                    .Select(y => new BannerDto
                    {
                        PicPath = y.PicPath,
                        Title = y.Title,
                        Url = y.Url
                    }));
        }

        /// <summary>
        /// Dispose banner Service
        /// </summary>
        public void Dispose()
        {
            if (_repoBanner != null)
            {
                _repoBanner.Dispose();
            }
        }

        public async Task<IEnumerable<SponsorDto>> GetAllSponserRecords()
        {
            var banner = _repoSponser
               .Query()
               .Filter(x => x.IsActive && !string.IsNullOrEmpty(x.SponsorLogo))
               .Get();

            return await Task.FromResult(banner
                    .Select(y => new SponsorDto
                    {
                        SponsorLogo = y.SponsorLogo,
                        SponsorName = y.SponsorName,
                        IsActive = y.IsActive,
                        WebUrl = y.WebUrl
                    }));
        }
    }
}
