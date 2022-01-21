using RR.AdminData;
using RR.AdminMapper;
using RR.AdminService;
using RR.Core;
using RR.Dto;
using RR.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.AdminService
{
    public class BannerService : IBannerService
    {
        #region Constructor
        public readonly IRepository<Banner, RankRideAdminContext> _repoBanner;
        #endregion
        public BannerService(IRepository<Banner, RankRideAdminContext> repoBanner)
        {
            _repoBanner = repoBanner;
        }
        /// <summary>
        /// Add Update Banner Detail
        /// </summary>
        /// <param name="bannerDto"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task AddEditBanner(BannerDto bannerDto, string userId)
        {
            try
            {
                Banner bannerEntity = new Banner();
                if (bannerDto.Id > 0)
                {
                    bannerEntity = _repoBanner.Query().Filter(x=>x.Id == bannerDto.Id).Get().FirstOrDefault();
                    bannerEntity.UpdatedBy = userId;
                    bannerEntity.UpdatedDate = DateTime.Now;
                  //  bannerEntity.Title = bannerDto.Title;
                  //  bannerEntity.Url = bannerDto.Url;
                    bannerEntity.PicPath = bannerDto.PicPath;
                    await _repoBanner.UpdateAsync(bannerEntity);
                }
                else
                {
                    bannerEntity.CreatedDate = DateTime.Now;
                    bannerEntity.CreatedBy = userId;
                    bannerEntity.UpdatedBy = userId;
                    bannerEntity.UpdatedDate = DateTime.Now;
                   // bannerEntity.Title = bannerDto.Title;
                   // bannerEntity.Url = bannerDto.Url;
                    bannerEntity.PicPath = bannerDto.PicPath;
                    await _repoBanner.InsertAsync(bannerEntity);

                }


            }
            catch(Exception )
            {

            }

        }
        /// <summary>
        /// Get All Banners
        /// </summary>
        /// <param name="start">Page Number</param>
        /// <param name="length">Number Of Record </param>
        /// <param name="searchStr"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public async Task<Tuple<IEnumerable<BannerDto>, int>> GetAllBannersRecords(int start, int length, int column, string searchStr = "", string sort = "")
        {
            int count = 0;
            var predicate = PredicateBuilder.True<Banner>()
           .And(x => (searchStr == "" || x.Title.ToLower().Contains(searchStr.ToLower())
           || x.Id.ToString().Contains(searchStr.ToLower()) || 
           x.Url.Contains(searchStr.ToLower())));

            var banner = _repoBanner
                .Query()
                .Filter(predicate);

            switch (column)
            {
                case 0:
                    banner = (sort == "desc" ? banner.OrderBy(x => x.OrderByDescending(xx => xx.Id)) : banner.OrderBy(x => x.OrderBy(xx => xx.Id)));
                    break;
                case 1:
                    banner = (sort == "desc" ? banner.OrderBy(x => x.OrderByDescending(xx => xx.Title)) : banner.OrderBy(x => x.OrderBy(xx => xx.Title)));
                    break;
                case 2:
                    banner = (sort == "desc" ? banner.OrderBy(x => x.OrderByDescending(xx => xx.Url)) : banner.OrderBy(x => x.OrderBy(xx => xx.Url)));
                    break;

                default:
                    banner = (sort == "desc" ? banner.OrderBy(x => x.OrderByDescending(xx => xx.CreatedDate)) : banner.OrderBy(x => x.OrderBy(xx => xx.CreatedDate)));
                    break;
            }

            return await Task.FromResult(new Tuple<IEnumerable<BannerDto>, int>(banner
                    .GetPage(start, length, out count).Select(y => new BannerDto
                    {
                        Id = y.Id,
                        PicPath = y.PicPath,
                        Title = y.Title,
                        Url = y.Url
                    }), count));
        }
        /// <summary>
        /// Get Banner By Id
        /// </summary>
        /// <param name="Id">Banner Id</param>
        /// <returns>The BannerDto</returns>
        public async Task<BannerDto> GetBannerById(int Id)
        {
            var BannerData = _repoBanner.Query().Filter(x => x.Id == Id).Get().FirstOrDefault();

            return await Task.FromResult(BannerMapper.MapDto(BannerData));
        }

        /// <summary>
        /// Delete Banner By Id
        /// </summary>
        /// <param name="Id">Banner Id</param>
        /// <returns></returns>
        public void DeleteBanner(int Id)
        {
            _repoBanner.Delete(Id);
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
    }
}
