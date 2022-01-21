using Microsoft.AspNetCore.Mvc.Rendering;
using RR.AdminData;
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
    public class BullService : IBullService
    {
        #region Constructor

        private readonly IRepository<Bull, RankRideStaticContext> _repoBull;
        private readonly IPictureManagerService _pictureManager;
        private readonly IRepository<EventDraw, RankRideStaticContext> _repoEventDraw;

        public BullService(IRepository<Bull, RankRideStaticContext> repoBull,
            IRepository<EventDraw, RankRideStaticContext> repoEventDraw,
            IPictureManagerService pictureManager)
        {
            _repoBull = repoBull;
            _pictureManager = pictureManager;
            _repoEventDraw = repoEventDraw;
        }

        #endregion

        /// <summary>
        /// Get All Bulls
        /// </summary>
        /// <param name="start">Page Number</param>
        /// <param name="length">Number Of Record </param>
        /// <param name="searchStr"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public async Task<Tuple<IEnumerable<BullLiteDto>, int>> GetAllBulls(int start, int length, int column, string searchStr = "", string sort = "", int rank = 1, bool getCleanbull = false)
        {
            int count = 0;

            var predicate = PredicateBuilder.True<Bull>()
          .And(x => x.IsDelete == false && (searchStr == "" || x.Name.Contains(searchStr.ToLower())
          || x.Owner.Contains(searchStr.ToLower())
          || x.ActiveRank.ToString().Contains(searchStr.ToLower())
          || x.PowerRating.ToString().Contains(searchStr.ToLower())
          || x.Mounted.ToString().Contains(searchStr.ToLower())
          || x.BuckOffPerc.ToString().Contains(searchStr.ToLower())
          || x.BullId.ToString().Contains(searchStr.ToLower())) && rank == 1 ? x.HistoricalRank > 0 : x.HistoricalRank == 0);

            if (getCleanbull)
            {
                var year_ = DateTime.Now.AddYears(-1).Year;
                var bullIds = _repoEventDraw.Query().Filter(r => r.CreatedDate.Year == year_).Get().Select(r => r.BullId).ToList();
                predicate = predicate.And(d => bullIds.Count() > 0 && !bullIds.Contains(d.BullId));
            }

            var bulls = _repoBull
                .Query()
                .Filter(predicate);

            if (FilterSortingVariable.BULL_ID == column)
            {
                bulls = (sort == "desc" ? bulls.OrderBy(x => x.OrderByDescending(xx => xx.BullId)) : bulls.OrderBy(x => x.OrderBy(xx => xx.BullId)));
            }
            if (FilterSortingVariable.BULL_NAME == column)
            {
                bulls = (sort == "desc" ? bulls.OrderBy(x => x.OrderByDescending(xx => xx.Name)) : bulls.OrderBy(x => x.OrderBy(xx => xx.Name)));
            }
            if (FilterSortingVariable.BULL_OWNER == column)
            {
                bulls = (sort == "desc" ? bulls.OrderBy(x => x.OrderByDescending(xx => xx.Owner)) : bulls.OrderBy(x => x.OrderBy(xx => xx.Owner)));
            }
            if (FilterSortingVariable.BULL_HISTORICALRANK == column)
            {
                bulls = (sort == "desc" ? bulls.OrderBy(x => x.OrderByDescending(xx => xx.HistoricalRank)) : bulls.OrderBy(x => x.OrderBy(xx => xx.HistoricalRank)));
            }
            if (FilterSortingVariable.BULL_POWER == column)
            {
                bulls = (sort == "desc" ? bulls.OrderBy(x => x.OrderByDescending(xx => xx.PowerRating)) : bulls.OrderBy(x => x.OrderBy(xx => xx.PowerRating)));
            }
            if (FilterSortingVariable.BULL_MOUNTED == column)
            {
                bulls = (sort == "desc" ? bulls.OrderBy(x => x.OrderByDescending(xx => xx.Mounted)) : bulls.OrderBy(x => x.OrderBy(xx => xx.Mounted)));
            }
            if (FilterSortingVariable.BULL_BUCKOFF == column)
            {
                bulls = (sort == "desc" ? bulls.OrderBy(x => x.OrderByDescending(xx => xx.BuckOffPerc)) : bulls.OrderBy(x => x.OrderBy(xx => xx.BuckOffPerc)));
            }


            return await Task.FromResult(new Tuple<IEnumerable<BullLiteDto>, int>(bulls
                    .GetPage(start, length, out count).Select(y => new BullLiteDto
                    {
                        BullId = y.BullId,
                        Id = y.Id,
                        BuckOffPerc = y.BuckOffPerc,
                        HistoricalRank = y.HistoricalRank,
                        Mounted = y.Mounted,
                        Name = y.Name,
                        Owner = y.Owner,
                        PowerRating = y.PowerRating,
                        isActive = y.IsActive,
                        Aatar = (_pictureManager.GetBullPic(y.BullId)).Result
                    }), count));
        }

        /// <summary>
        /// Get Bull By Id
        /// </summary>
        /// <param name="bullId">Bull Id</param>
        /// <returns>The BullDto</returns>
        public async Task<BullDto> GetBullById(int bullId)
        {
            var bullDetail = _repoBull.Query()
              .Filter(e => e.Id == bullId)
              .Get()
              .SingleOrDefault();
            return await Task.FromResult(BullMapper.MapDto(bullDetail));
        }

        /// <summary>
        /// Update Bull Detail
        /// </summary>
        /// <param name="bullDto">The BullDto</param>
        /// <returns></returns>
        public async Task UpdateBullDetail(BullDto bullDto)
        {
            var bullData = _repoBull.Query().Filter(x => x.BullId == bullDto.Id).Get().FirstOrDefault();

            bullData.Brand = !string.IsNullOrEmpty(bullDto.Brand) ? bullDto.Brand : "";
            bullData.ActiveRank = Convert.ToInt32(bullDto.ActiveRank);
            bullData.AverageMark = bullDto.AverageMark;
            bullData.BuckOffPerc = bullDto.BuckOffPerc;
            bullData.BuckOffPercVsLeftHandRider = bullDto.BuckOffPercVsLeftHandRider;
            bullData.BuckOffPercVsRightHandRider = bullDto.BuckOffPercVsRightHandRider;
            bullData.BuckOffPercVsTopRider = bullDto.BuckOffPercVsTopRider;
            bullData.HistoricalRank = Convert.ToInt32(bullDto.HistoricalRank);
            bullData.IsRegistered = Convert.ToBoolean(bullDto.IsRegistered);
            bullData.Mounted = bullDto.Mounted;
            bullData.Name = !string.IsNullOrEmpty(bullDto.Name) ? bullDto.Name : "";
            bullData.OutsVsTopRiders = bullDto.OutsVsTopRiders;
            bullData.OutVsLeftHandRider = bullDto.OutVsLeftHandRider;
            bullData.OutVsRightHandRider = bullDto.OutVsRightHandRider;
            bullData.Owner = !string.IsNullOrEmpty(bullDto.Owner) ? bullDto.Owner : "";
            bullData.PowerRating = bullDto.PowerRating;
            bullData.Rode = bullDto.Rode;
            await _repoBull.UpdateAsync(bullData);
        }

        /// <summary>
        /// Update Bull Status
        /// </summary>
        /// <param name="bullId">Bull Id</param>
        /// <returns></returns>
        public async Task UpdateStatus(int bullId)
        {
            var bullDetail = _repoBull.Query()
                .Filter(x => x.BullId == bullId)
               .Get()
               .SingleOrDefault();
            bullDetail.IsActive = (bullDetail.IsActive == true) ? false : true;
            await _repoBull.UpdateAsync(bullDetail);
        }

        /// <summary>
        /// Delete Bull By Id
        /// </summary>
        /// <param name="bullId">Bull Id</param>
        /// <returns></returns>
        public async Task DeleteBullById(int bullId)
        {
            await _repoBull.DeleteAsync(bullId);
        }

        /// <summary>
        /// Dispose Bull Service
        /// </summary>
        public void Dispose()
        {
            if (_repoBull != null)
            {
                _repoBull.Dispose();
            }
        }

        public Task<IEnumerable<SelectListItem>> GetBulls(int selectedid = 0)
        {
            return Task.FromResult(_repoBull.Query().Get().Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.BullId.ToString(),
                Selected = (r.BullId == selectedid),
            }));
        }

        public async Task<Tuple<int, int>> GetBullsAsCard()
        {
            var total_ = _repoBull.Query().Get().Count();
            var active_ = _repoBull.Query().Filter(r => r.IsActive == true).Get().Count();
            return await Task.FromResult(new Tuple<int, int>(total_, active_));
        }

        public async Task DeleteNotSeenBulls(bool isParmanentdelete = false)
        {
            if (isParmanentdelete)
            {
                var year_ = DateTime.Now.AddYears(-1).Year;
                var bullIds = _repoEventDraw.Query().Filter(r => r.CreatedDate.Year == year_).Get().Select(r => r.BullId).ToList();

                var bulls = _repoBull.Query().Filter(d => bullIds.Count() > 0 && !bullIds.Contains(d.BullId)).Get().ToList();

                await _repoBull.DeleteCollection(bulls);
            }
            else
            {
                var year_ = DateTime.Now.AddYears(-1).Year;
                var bullIds = _repoEventDraw.Query().Filter(r => r.CreatedDate.Year == year_).Get().Select(r => r.BullId).ToList();

                var bulls = _repoBull.Query().Filter(d => bullIds.Count() > 0 && !bullIds.Contains(d.BullId)).Get().ToList();

                foreach (var item in bulls)
                {
                    item.IsActive = false;
                    item.IsDelete = true;
                }
                await _repoBull.UpdateCollection(bulls);
            }
        }
    }
}
