using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RR.Core;
using RR.Data;
using RR.Dto;
using RR.Dto.Event;
using RR.Repo;
using RR.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Service
{
    public class FavoriteBullsRidersService : IFavoriteBullsRidersService
    {
        #region Constructor
        private readonly IRepository<FavoriteBullRiders, RankRideContext> _repoFavoriteBullRiders;
        private readonly IRepository<Bull, RankRideStaticContext> _repoBullStatic;
        private readonly IRepository<Rider, RankRideStaticContext> _repoRiderStatic;
        private readonly IRepository<Event, RankRideStaticContext> _repoEvent;
        private readonly AppSettings _appsettings;
        private readonly IBullRiderPicturesService _picturesService;

        public FavoriteBullsRidersService(IRepository<FavoriteBullRiders, 
            RankRideContext> repoFavoriteBullRiders, 
            IRepository<Bull, RankRideStaticContext> repoBullStatic,
            IRepository<Rider, RankRideStaticContext> repoRiderStatic,
            IRepository<Event, RankRideStaticContext> repoEvent,
            IOptions<AppSettings> appsettings,
            IBullRiderPicturesService picturesService)
        {
            _repoFavoriteBullRiders = repoFavoriteBullRiders;
            _repoBullStatic = repoBullStatic;
            _repoRiderStatic = repoRiderStatic;
            _appsettings = appsettings.Value;
            _picturesService = picturesService;
            _repoEvent = repoEvent;
        }
        #endregion

        /// <summary>
        /// Add Rider as Favorite 
        /// </summary>
        /// <param name="favoriteBullRidersDto">FavoriteBullRiders Dto</param>
        /// <returns></returns>
        public async Task AddRiderAsFavorite(FavoriteBullRidersDto favoriteBullRidersDto)
        {

            bool IsExist = _repoFavoriteBullRiders.Query()
                            .Filter(x => x.UserId == favoriteBullRidersDto.UserId && x.RiderId == favoriteBullRidersDto.RiderId)
                            .Get().Count() > 0;
            if (!IsExist)
            {
                await _repoFavoriteBullRiders.InsertAsync(new FavoriteBullRiders
                {
                    Id = favoriteBullRidersDto.Id,
                    RiderId = favoriteBullRidersDto.RiderId,
                    UserId = favoriteBullRidersDto.UserId
                });
            }
        }

        /// <summary>
        /// Add Bull as Favorite 
        /// </summary>
        /// <param name="favoriteBullRidersDto">FavoriteBullRiders Dto</param>
        /// <returns></returns>
        public async Task AddBullAsFavorite(FavoriteBullRidersDto favoriteBullRidersDto)
        {

            bool IsExist = _repoFavoriteBullRiders.Query()
                            .Filter(x => x.UserId == favoriteBullRidersDto.UserId && x.BullId == favoriteBullRidersDto.BullId)
                            .Get().Count() > 0;
            if (!IsExist)
            {
                await _repoFavoriteBullRiders.InsertAsync(new FavoriteBullRiders
                {
                    Id = favoriteBullRidersDto.Id,
                    BullId = favoriteBullRidersDto.BullId,
                    UserId = favoriteBullRidersDto.UserId
                });
            }
        }

        /// <summary>
        /// Check rider added as Favorite
        /// </summary>
        /// <param name="AspNetUserId">AspNet UserId</param>
        /// <param name="riderId">rider Id</param>
        /// <returns>True/False</returns>
        public async Task<bool> IsRiderAdded(string AspNetUserId, int riderId)
        {
            return await Task.FromResult((_repoFavoriteBullRiders.Query()
                           .Filter(x => x.UserId == AspNetUserId && x.RiderId == riderId)
                           .Get().Count() > 0));
        }

        /// <summary>
        /// Check bull added as Favorite
        /// </summary>
        /// <param name="AspNetUserId">AspNet UserId</param>
        /// <param name="bullId">bull Id</param>
        /// <returns>True/False</returns>
        public async Task<bool> IsBullAdded(string AspNetUserId, int bullId)
        {
            return await Task.FromResult((_repoFavoriteBullRiders.Query()
                           .Filter(x => x.UserId == AspNetUserId && x.BullId == bullId)
                           .Get().Count() > 0));
        }

        /// <summary>
        /// Get User Favorite Bulls and Riders
        /// </summary>
        /// <param name="AspNetUserId">AspNet User Id</param>
        /// <returns>UserFavoriteBullsRidersDto</returns>
        public async Task<UserFavoriteBullsRidersDto> GetUserFavoriteBullsRiders(string AspNetUserId)
        {
            if (!string.IsNullOrEmpty(AspNetUserId))
            {
                var temp1 = _repoEvent.Query().Filter(d => d.StartDate.Year == DateTime.Now.Year && !string.IsNullOrEmpty(d.EventResult)).Get().SelectMany(m => JsonConvert.DeserializeObject<IEnumerable<EventResultDto>>(m.EventResult));

                
                var bullIds = _repoFavoriteBullRiders.Query().Filter(x => x.UserId == AspNetUserId && x.BullId > 0).Get().Select(d => d.BullId);
                var riderIds = _repoFavoriteBullRiders.Query().Filter(x => x.UserId == AspNetUserId && x.RiderId > 0).Get().Select(d => d.RiderId);

                var evenResults = temp1.GroupBy(r => r.rider_id).Select(r => new { key = r.Key, point = r.Sum(g => g.rr_rider_score) });
                var evenResultsBull = temp1.GroupBy(r => r.pbid).Select(r => new { key = r.Key, point = r.Sum(g => g.rr_bull_score) });

                var riders = _repoRiderStatic.Query().Filter(r => riderIds.Contains(r.Id)).Get().Select(ri => new UserFavoriteRidersDto
                {
                    Id = ri.Id,
                    RiderId = ri.RiderId,
                    RiderName = ri.Name,
                    WorldRank = ri.Cwrp.HasValue ? ri.Cwrp.Value : 0,
                    Avatar = _picturesService.GetRiderPic(ri.RiderId, _appsettings.MainSiteURL).Result,
                    RRTotalPoint = evenResults.FirstOrDefault(k => k.key == ri.RiderId)?.point ?? 0,
                    PowerRanking = ri.RiderPower
                }).ToList();

                var bulls = _repoBullStatic.Query().Filter(r => bullIds.Contains(r.Id)).Get().Select(b => new UserFavoriteBullsDto
                {
                    Id = b.Id,
                    BullId = b.BullId,
                    BullName = b.Name,
                    AverageMark = b.AverageMark,
                    PowerRating = b.PowerRating,
                    OwnerName = b.Owner,
                    Avatar = _picturesService.GetBullPic(b.BullId, _appsettings.MainSiteURL).Result,
                    RankRideScore = evenResultsBull.FirstOrDefault(r => r.key == b.BullId)?.point ?? 0
                }).ToList();

                return await Task.FromResult(new UserFavoriteBullsRidersDto
                {
                    FavoriteBulls = bulls,
                    FavoriteRiders = riders
                });
            }
            return await Task.FromResult(new UserFavoriteBullsRidersDto());
        }

        /// <summary>
        /// Remove User Favorite Bulls
        /// </summary>
        /// <param name="AspNetUserId">AspNet User Id</param>
        /// <param name="bullId">Bull Id</param>
        /// <returns></returns>
        public async Task RemoveUserFavoriteBulls(string AspNetUserId, int bullId)
        {
            var favBull = _repoFavoriteBullRiders.Query()
                           .Filter(x => x.UserId == AspNetUserId && x.BullId == bullId)
                           .Get().SingleOrDefault();
            if(favBull != null)
            {
                await _repoFavoriteBullRiders.DeleteAsync(favBull);
            }
        }

        /// <summary>
        /// Remove User Favorite Riders
        /// </summary>
        /// <param name="AspNetUserId">AspNet User Id</param>
        /// <param name="riderId">Rider Id</param>
        /// <returns></returns>
        public async Task RemoveUserFavoriteRider(string AspNetUserId, int riderId)
        {
            var favRider = _repoFavoriteBullRiders.Query()
                           .Filter(x => x.UserId == AspNetUserId && x.RiderId == riderId)
                           .Get().SingleOrDefault();

            if (favRider != null)
            {
                await _repoFavoriteBullRiders.DeleteAsync(favRider);
            }
        }

        /// <summary>
        /// Dispose UserDetail Service
        /// </summary>
        public void Dispose()
        {
            if (_repoFavoriteBullRiders != null)
            {
                _repoFavoriteBullRiders.Dispose();
            }
        }
    }
}
