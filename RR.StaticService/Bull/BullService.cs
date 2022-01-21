using RR.Core;
using RR.Dto;
using RR.Repo;
using RR.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.StaticService
{
    public class BullService : IBullService
    {
        #region Constructor

        private readonly IRepository<Bull, RankRideStaticContext> _repoBull;

        public BullService(IRepository<Bull, RankRideStaticContext> repoBull)
        {
            _repoBull = repoBull;
        }

        #endregion

        /// <summary>
        /// Add Edit Bulls
        /// </summary>
        /// <param name="bullDto">Lis Of BullDto</param>
        /// <returns></returns>
        public void AddEditBulls(BullDto bullDto)
        {
            var bullRecord = _repoBull.Query()
                 .Filter(x => x.BullId == bullDto.Id)
                 .Get()
                 .SingleOrDefault();

            if (bullRecord == null)
            {
                var bull = new Bull
                {
                    BullId = bullDto.Id,
                    ActiveRank = (bullDto.ActiveRank == "unranked" ? 0 : Convert.ToInt32(bullDto.ActiveRank)),
                    AverageMark = Convert.ToDecimal(bullDto.AverageMark),
                    Brand = !string.IsNullOrEmpty(bullDto.Brand) ? bullDto.Brand : "",
                    BuckOffPerc = Convert.ToDecimal(bullDto.BuckOffPerc),
                    BuckOffPercVsLeftHandRider = Convert.ToDecimal(bullDto.BuckOffPercVsLeftHandRider),
                    BuckOffPercVsRightHandRider = Convert.ToDecimal(bullDto.BuckOffPercVsRightHandRider),
                    BuckOffPercVsTopRider = Convert.ToDecimal(bullDto.BuckOffPercVsTopRider),
                    HistoricalRank = (bullDto.HistoricalRank == "unranked" ? 0 : Convert.ToInt32(bullDto.HistoricalRank)),
                    IsRegistered = (bullDto.IsRegistered == "yes" ? true : false),
                    Mounted = Convert.ToInt32(bullDto.Mounted),
                    Name = !string.IsNullOrEmpty(bullDto.Name) ? bullDto.Name : "",
                    OutsVsTopRiders = Convert.ToInt32(bullDto.OutsVsTopRiders),
                    OutVsLeftHandRider = Convert.ToInt32(bullDto.OutVsLeftHandRider),
                    OutVsRightHandRider = Convert.ToInt32(bullDto.OutVsRightHandRider),
                    Owner = !string.IsNullOrEmpty(bullDto.Owner) ? bullDto.Owner : "",
                    PowerRating = Convert.ToDecimal(bullDto.PowerRating),
                    Rode = Convert.ToInt32(bullDto.Rode),
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    IsActive = true
                };
                _repoBull.Insert(bull);
            }
            else
            {
                bullRecord.BullId = bullDto.Id;
                bullRecord.ActiveRank = (bullDto.ActiveRank == "unranked" ? 0 : Convert.ToInt32(bullDto.ActiveRank));
                bullRecord.AverageMark = Convert.ToDecimal(bullDto.AverageMark);
                bullRecord.Brand = !string.IsNullOrEmpty(bullDto.Brand) ? bullDto.Brand : "";
                bullRecord.BuckOffPerc = Convert.ToDecimal(bullDto.BuckOffPerc);
                bullRecord.BuckOffPercVsLeftHandRider = Convert.ToDecimal(bullDto.BuckOffPercVsLeftHandRider);
                bullRecord.BuckOffPercVsRightHandRider = Convert.ToDecimal(bullDto.BuckOffPercVsRightHandRider);
                bullRecord.BuckOffPercVsTopRider = Convert.ToDecimal(bullDto.BuckOffPercVsTopRider);
                bullRecord.HistoricalRank = (bullDto.HistoricalRank == "unranked" ? 0 : Convert.ToInt32(bullDto.HistoricalRank));
                bullRecord.IsRegistered = (bullDto.IsRegistered == "yes" ? true : false);
                bullRecord.Mounted = Convert.ToInt32(bullDto.Mounted);
                bullRecord.Name = !string.IsNullOrEmpty(bullDto.Name) ? bullDto.Name : "";
                bullRecord.OutsVsTopRiders = Convert.ToInt32(bullDto.OutsVsTopRiders);
                bullRecord.OutVsLeftHandRider = Convert.ToInt32(bullDto.OutVsLeftHandRider);
                bullRecord.OutVsRightHandRider = Convert.ToInt32(bullDto.OutVsRightHandRider);
                bullRecord.Owner = !string.IsNullOrEmpty(bullDto.Owner) ? bullDto.Owner : "";
                bullRecord.PowerRating = Convert.ToDecimal(bullDto.PowerRating);
                bullRecord.Rode = Convert.ToInt32(bullDto.Rode);
                bullRecord.UpdatedDate = DateTime.Now;
                _repoBull.Update(bullRecord);
            }
        }

        /// <summary>
        /// Get All Bulls
        /// </summary>
        /// <param name="start">The Start Page</param>
        /// <param name="length">The Page Size</param>
        /// <param name="searchStr">The Search Keyword</param>
        /// <param name="sort">The Order of page</param>
        /// <returns>List Of 10 Bulls Along</returns>
        public async Task<Tuple<IEnumerable<BullDto>, int>> GetAllBulls(int start, int length, int column, string searchStr = "", string sort = "")
        {
            int count = 0;

            var predicate = PredicateBuilder.True<Bull>()
           .And(x => x.IsDelete == false && (searchStr == ""
              || x.Name.ToLower().Contains(searchStr.ToLower())
              || x.Owner.Contains(searchStr.ToLower())
              || x.BullId.ToString().Contains(searchStr.ToLower())));

            var bulls = _repoBull
                .Query()
              .Filter(predicate);

            if (FilterSortingVariable.BULL_ID == column)
            {
                bulls = (sort == "desc" ? bulls.OrderBy(x => x.OrderByDescending(xx => xx.Id)) : bulls.OrderBy(x => x.OrderBy(xx => xx.Id)));
            }
            if (FilterSortingVariable.BULL_NAME == column)
            {
                bulls = (sort == "desc" ? bulls.OrderBy(x => x.OrderByDescending(xx => xx.Name)) : bulls.OrderBy(x => x.OrderBy(xx => xx.Name)));
            }
            if (FilterSortingVariable.BULL_OWNER == column)
            {
                bulls = (sort == "desc" ? bulls.OrderBy(x => x.OrderByDescending(xx => xx.Owner)) : bulls.OrderBy(x => x.OrderBy(xx => xx.Owner)));
            }
            if (FilterSortingVariable.BULL_POWERATING == column)
            {
                bulls = (sort == "desc" ? bulls.OrderBy(x => x.OrderByDescending(xx => xx.PowerRating)) : bulls.OrderBy(x => x.OrderBy(xx => xx.PowerRating)));
            }
            if (FilterSortingVariable.BULL_AVERAGEMARK == column)
            {
                bulls = (sort == "desc" ? bulls.OrderBy(x => x.OrderByDescending(xx => xx.AverageMark)) : bulls.OrderBy(x => x.OrderBy(xx => xx.AverageMark)));
            }

            return await Task.FromResult(new Tuple<IEnumerable<BullDto>, int>(bulls
                 .GetPage(start, length, out count).Select(y => new BullDto
                 {
                     Id = y.Id,
                     AverageMark = y.AverageMark,
                     Age = 0,
                     Breeding = "",
                     BuckingStatistics = "",
                     BuckOffPerc = y.BuckOffPerc,
                     Name = y.Name,
                     Owner = y.Owner,
                     PowerRating = y.PowerRating,
                     BuckOffStreak = 0,
                     Rode = y.Rode,
                     BullId = y.BullId
                 }), count));
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
    }
}
