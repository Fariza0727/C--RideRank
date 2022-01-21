using Microsoft.AspNetCore.Mvc.Rendering;
using RR.AdminMapper;
using RR.Core;
using RR.Dto;
using RR.Repo;
using RR.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.AdminService
{
    public class RiderService : IRiderService
    {
        #region Constructor 

        private readonly IRepository<Rider, RankRideStaticContext> _repoRider;
        private readonly IRepository<EventDraw, RankRideStaticContext> _repoEventDraw;
        private readonly IPictureManagerService _pictureManager;

        public RiderService(IRepository<Rider, RankRideStaticContext> repoRider,
            IRepository<EventDraw, RankRideStaticContext> repoEventDraw, IPictureManagerService pictureManager)
        {
            _repoRider = repoRider;
            _pictureManager = pictureManager;
            _repoEventDraw = repoEventDraw;
        }

        #endregion

        /// <summary>
        /// Get All Riders
        /// </summary>
        /// <param name="start">Page Number</param>
        /// <param name="length">Number Of Record </param>
        /// <param name="searchStr"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public async Task<Tuple<IEnumerable<RiderDto>, int>> GetAllRiders(int start, int length, int column, string searchStr = "", string sort = "", int rank = 1, bool getCleanRider = false)
        {
            int count = 0;

            var predicate = PredicateBuilder.True<Rider>()
        .And(x => x.IsDelete == false && (searchStr == "" || x.Name.Contains(searchStr.ToLower())
        || x.RiderId.ToString().Contains(searchStr.ToLower())
        || x.Mounted.ToString().Contains(searchStr.ToLower())
        || x.Rode.ToString().Contains(searchStr.ToLower())
        || x.Streak.ToString().Contains(searchStr.ToLower())
        || x.Hand.Contains(searchStr.ToLower())
        || x.RiderPower.ToString().Contains(searchStr.ToLower())) && (rank == 1 ? x.Cwrp.HasValue : !x.Cwrp.HasValue));

            if (getCleanRider)
            {
                var year_ = DateTime.Now.AddYears(-1).Year;
                var riderIds = _repoEventDraw.Query().Filter(r => r.CreatedDate.Year == year_).Get().Select(r => r.RiderId).ToList();
                predicate = predicate.And(d => riderIds.Count() >0&& !riderIds.Contains(d.RiderId));
            }

            var riders = _repoRider
                .Query()
                .Filter(predicate);

            //IList<RiderDto> records = riders.Get().Select(y => new RiderDto
            //{
            //    RiderId = y.RiderId,
            //    Id = y.Id,
            //    Hand = y.Hand,
            //    Mounted = y.Mounted,
            //    Name = y.Name,
            //    RidePerc = y.RidePerc,
            //    RiderPower = y.RiderPower,
            //    RiderPowerCurrent = y.RiderPowerCurrent,
            //    Rode = y.Rode,
            //    Streak = y.Streak,
            //    IsActive = y.IsActive,
            //    //WorldRanking = riders.Get().Count(x => x.RiderPowerCurrent > x.RiderPowerCurrent) + 1
            //}).ToList();

            //records = records.Select(y => new RiderDto
            //{
            //    RiderId = y.RiderId,
            //    Id = y.Id,
            //    Hand = y.Hand,
            //    Mounted = y.Mounted,
            //    Name = y.Name,
            //    RidePerc = y.RidePerc,
            //    RiderPower = y.RiderPower,
            //    RiderPowerCurrent = y.RiderPowerCurrent,
            //    Rode = y.Rode,
            //    Streak = y.Streak,
            //    IsActive = y.IsActive,
            //    WorldRanking = records.Count(x => x.RiderPowerCurrent > y.RiderPowerCurrent)
            //}).ToList();
            //count = records.Count;
            if (FilterSortingVariable.RIDER_WORLDRANKING == column)
            {
                riders = (sort == "desc" ? riders.OrderBy(x => x.OrderByDescending(xx => xx.Cwrp)) : riders.OrderBy(x => x.OrderBy(xx => xx.Cwrp)));
                //records = sort == "desc" ? records.OrderByDescending(x => x.WorldRanking).ToList() : records.OrderBy(x => x.WorldRanking).ToList();
            }

            if (FilterSortingVariable.RIDER_ID == column)
            {
                riders = (sort == "desc" ? riders.OrderBy(x => x.OrderByDescending(xx => xx.RiderId)) : riders.OrderBy(x => x.OrderBy(xx => xx.RiderId)));
                //records = sort == "desc" ? records.OrderByDescending(x => x.RiderId).ToList() : records.OrderBy(x => x.RiderId).ToList();
            }
            if (FilterSortingVariable.RIDER_NAME == column)
            {
                riders = (sort == "desc" ? riders.OrderBy(x => x.OrderByDescending(xx => xx.Name)) : riders.OrderBy(x => x.OrderBy(xx => xx.Name)));
                //records = sort == "desc" ? records.OrderByDescending(x => x.Name).ToList() : records.OrderBy(x => x.Name).ToList();
            }
            if (FilterSortingVariable.RIDER_MOUNTED == column)
            {
                riders = (sort == "desc" ? riders.OrderBy(x => x.OrderByDescending(xx => xx.Mounted)) : riders.OrderBy(x => x.OrderBy(xx => xx.Mounted)));
                //records = sort == "desc" ? records.OrderByDescending(x => x.Mounted).ToList() : records.OrderBy(x => x.Mounted).ToList();
            }
            if (FilterSortingVariable.RIDER_RODE == column)
            {
                riders = (sort == "desc" ? riders.OrderBy(x => x.OrderByDescending(xx => xx.Rode)) : riders.OrderBy(x => x.OrderBy(xx => xx.Rode)));
                //records = sort == "desc" ? records.OrderByDescending(x => x.Rode).ToList() : records.OrderBy(x => x.Rode).ToList();
            }
            if (FilterSortingVariable.RIDER_STREAK == column)
            {
                riders = (sort == "desc" ? riders.OrderBy(x => x.OrderByDescending(xx => xx.Streak)) : riders.OrderBy(x => x.OrderBy(xx => xx.Streak)));
                //records = sort == "desc" ? records.OrderByDescending(x => x.Streak).ToList() : records.OrderBy(x => x.Streak).ToList();
            }
            if (FilterSortingVariable.RIDER_HAND == column)
            {
                riders = (sort == "desc" ? riders.OrderBy(x => x.OrderByDescending(xx => xx.Hand)) : riders.OrderBy(x => x.OrderBy(xx => xx.Hand)));
                //records = sort == "desc" ? records.OrderByDescending(x => x.Hand).ToList() : records.OrderBy(x => x.Hand).ToList();
            }
            if (FilterSortingVariable.RIDER_POWER == column)
            {
                riders = (sort == "desc" ? riders.OrderBy(x => x.OrderByDescending(xx => xx.RiderPower)) : riders.OrderBy(x => x.OrderBy(xx => xx.RiderPower)));
                //records = sort == "desc" ? records.OrderByDescending(x => x.RiderPowerCurrent).ToList() : records.OrderBy(x => x.RiderPowerCurrent).ToList();
            }
            //int skip = start * 10;
            //int take = 10;
            //return await Task.FromResult(new Tuple<IEnumerable<RiderDto>, int>(records.Skip(skip).Take(take), count));

            return await Task.FromResult(new Tuple<IEnumerable<RiderDto>, int>(riders
                     .GetPage(start, length, out count).Select(y => new RiderDto
                     {
                         RiderId = y.RiderId,
                         Id = y.Id,
                         Hand = y.Hand,
                         Mounted = y.Mounted,
                         Name = y.Name,
                         RidePerc = y.RidePerc,
                         RiderPower = y.RiderPowerCurrent,
                         Rode = y.Rode,
                         Streak = y.Streak,
                         IsActive = y.IsActive,
                         CWRP = y.Cwrp ?? 9999,
                         Avatar = (_pictureManager.GetRiderPic(y.RiderId)).Result
                     }), count));
        }

        /// <summary>
        /// Get Rider By Id
        /// </summary>
        /// <param name="riderId">Rider Id</param>
        /// <returns>The RiderDto</returns>
        public async Task<RiderDto> GetRiderById(int riderId)
        {
            var riderDetail = _repoRider.Query()
             .Filter(e => e.Id == riderId)
             .Get()
             .SingleOrDefault();

            return await Task.FromResult(RiderMapper.MapDto(riderDetail));
        }

        /// <summary>
        /// Update Rider Detail
        /// </summary>
        /// <param name="riderDto">The RiderDto</param>
        /// <returns></returns>
        public async Task UpdateRiderDetail(RiderDto riderDto)
        {
            var riderData = _repoRider.Query()
                 .Filter(x => x.RiderId == riderDto.Id)
                 .Get()
                 .SingleOrDefault();

            riderData.Hand = !string.IsNullOrEmpty(riderDto.Hand) ? riderDto.Hand : "";
            riderData.Mounted = riderDto.Mounted;
            riderData.MountedCurrent = Convert.ToInt32(riderDto.MountedCurrent);
            riderData.Name = !string.IsNullOrEmpty(riderDto.Name) ? riderDto.Name : "";
            riderData.RidePerc = Convert.ToDecimal(riderDto.RidePerc);
            riderData.RidePrecCurent = Convert.ToDecimal(riderDto.RidePrecCurent);
            riderData.RiderPower = Convert.ToDecimal(riderDto.RiderPower);
            riderData.RiderPowerCurrent = Convert.ToDecimal(riderDto.RiderPowerCurrent);
            riderData.Rode = riderDto.Rode;
            riderData.Streak = riderDto.Streak;

            await _repoRider.UpdateAsync(riderData);
        }

        /// <summary>
        /// Update Rider Status
        /// </summary>
        /// <param name="riderId">Rider Id</param>
        /// <returns></returns>
        public async Task UpdateStatus(int riderId)
        {
            var riderDetail = _repoRider.Query()
              .Filter(x => x.RiderId == riderId)
             .Get()
             .SingleOrDefault();
            riderDetail.IsActive = (riderDetail.IsActive == true) ? false : true;
            await _repoRider.UpdateAsync(riderDetail);
        }

        /// <summary>
        /// Delete Rider By Id
        /// </summary>
        /// <param name="riderId">Rider Id</param>
        /// <returns></returns>
        public async Task DeleteRider(int riderId)
        {
            await _repoRider.DeleteAsync(riderId);
        }

        /// <summary>
        /// Dispose Rider Service
        /// </summary>
        public void Dispose()
        {
            if (_repoRider != null)
            {
                _repoRider.Dispose();
            }
        }


        public Task<IEnumerable<SelectListItem>> GetRiders(int selectedid = 0)
        {
            return Task.FromResult(_repoRider.Query().Get().Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.RiderId.ToString(),
                Selected = (r.RiderId == selectedid),
            }));
        }

        public async Task<Tuple<int, int>> GetRidersAsCard()
        {
            var total_ = _repoRider.Query().Get().Count();
            var active_ = _repoRider.Query().Filter(r => r.IsActive == true).Get().Count();
            return await Task.FromResult(new Tuple<int, int>(total_, active_));
        }

        public async Task DeleteNotSeenRider(bool isParmanentdelete = false)
        {
            if (isParmanentdelete)
            {
                var year_ = DateTime.Now.AddYears(-1).Year;
                var riderIds = _repoEventDraw.Query().Filter(r => r.CreatedDate.Year == year_).Get().Select(r => r.RiderId).ToList();

                var riders_ = _repoRider.Query().Filter(d => riderIds.Count() > 0 && !riderIds.Contains(d.RiderId)).Get().ToList();

                await _repoRider.DeleteCollection(riders_);
            }
            else
            {
                var year_ = DateTime.Now.AddYears(-1).Year;
                var riderIds = _repoEventDraw.Query().Filter(r => r.CreatedDate.Year == year_).Get().Select(r => r.RiderId).ToList();

                var riders_ = _repoRider.Query().Filter(d => riderIds.Count() > 0 && !riderIds.Contains(d.RiderId)).Get().ToList();

                foreach (var item in riders_)
                {
                    item.IsActive = false;
                    item.IsDelete = true;
                }
                await _repoRider.UpdateCollection(riders_);
            }
        }
    }
}
