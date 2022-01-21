using RR.Core;
using RR.Dto;
using RR.Repo;
using RR.StaticData;
using RR.StaticMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.StaticService
{
    public class RiderService : IRiderService
    {
        #region Constructor

        private readonly IRepository<Rider, RankRideStaticContext> _repoRider;

        public RiderService(IRepository<Rider, RankRideStaticContext> repoRider)
        {
            _repoRider = repoRider;

        }

        #endregion

        /// <summary>
        /// Add Edit Riders
        /// </summary>
        /// <param name="riderDto">RiderDto</param>
        /// <returns></returns>
        public void AddEditRiders(RiderDto riderDto)
        {
            var riderRecord = _repoRider.Query()
                 .Filter(x => x.RiderId == riderDto.Id)
                 .Get()
                 .SingleOrDefault();

            if (riderRecord == null)
            {
                var rider = new Rider
                {
                    RiderId = riderDto.Id,
                    Name = !string.IsNullOrEmpty(riderDto.Name) ? riderDto.Name : "",
                    Mounted = Convert.ToInt32(riderDto.Mounted),
                    Rode = Convert.ToInt32(riderDto.Rode),
                    Streak = Convert.ToDecimal(riderDto.Streak),
                    Hand = !string.IsNullOrEmpty(riderDto.Hand) ? riderDto.Hand : "",
                    RidePerc = Convert.ToDecimal(riderDto.RidePerc),
                    RidePrecCurent = Convert.ToDecimal(riderDto.RidePrecCurent),
                    MountedCurrent = Convert.ToInt32(riderDto.MountedCurrent),
                    RiderPower = Convert.ToDecimal(riderDto.RiderPower),
                    RiderPowerCurrent = Convert.ToDecimal(riderDto.RiderPowerCurrent),
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    IsActive = true,
                    Cwrp = riderDto.CWRP
                };
                _repoRider.InsertGraph(rider);
            }
            else
            {
                riderRecord.Name = !string.IsNullOrEmpty(riderDto.Name) ? riderDto.Name : "";
                riderRecord.Mounted = Convert.ToInt32(riderDto.Mounted);
                riderRecord.Rode = Convert.ToInt32(riderDto.Rode);
                riderRecord.Streak = Convert.ToDecimal(riderDto.Streak);
                riderRecord.Hand = !string.IsNullOrEmpty(riderDto.Hand) ? riderDto.Hand : "";
                riderRecord.RidePerc = Convert.ToDecimal(riderDto.RidePerc);
                riderRecord.RidePrecCurent = Convert.ToDecimal(riderDto.RidePrecCurent);
                riderRecord.MountedCurrent = Convert.ToInt32(riderDto.MountedCurrent);
                riderRecord.RiderPower = Convert.ToDecimal(riderDto.RiderPower);
                riderRecord.RiderPowerCurrent = Convert.ToDecimal(riderDto.RiderPowerCurrent);
                riderRecord.UpdatedDate = DateTime.Now;
                riderRecord.Cwrp = riderDto.CWRP;
                _repoRider.Update(riderRecord);
            }
        }

        /// <summary>
        /// Get All Riders
        /// </summary>
        /// <param name="start">The Start Page</param>
        /// <param name="length">The Page Size</param>
        /// <param name="searchStr">The Search Keyword</param>
        /// <param name="sort">The Order of page</param>
        /// <returns>List Of 10 Riders Along</returns>
        public async Task<Tuple<IEnumerable<RiderDto>, int>> GetAllRiders(int start, int length, int column, string searchStr = "", string sort = "")
        {
            int count = 0;

            var predicate = PredicateBuilder.True<Rider>()
           .And(x => x.IsDelete == false && (searchStr == ""
                || x.Name.ToLower().Contains(searchStr.ToLower())
                || x.Hand.Contains(searchStr.ToLower())
                || x.Id.ToString().Contains(searchStr.ToLower())));

            var riders = _repoRider
                .Query()
                .Filter(predicate);

            if (FilterSortingVariable.RIDER_ID == column)
            {
                riders = (sort == "desc" ? riders.OrderBy(x => x.OrderByDescending(xx => xx.RiderId)) : riders.OrderBy(x => x.OrderBy(xx => xx.RiderId)));
            }
            if (FilterSortingVariable.RIDER_NAME == column)
            {
                riders = (sort == "desc" ? riders.OrderBy(x => x.OrderByDescending(xx => xx.Name)) : riders.OrderBy(x => x.OrderBy(xx => xx.Name)));
            }
            //if (FilterSortingVariable.RIDER_HEADSHOT == column)
            //{
            //     riders = (sort == "desc" ? riders.OrderBy(x => x.OrderByDescending(xx => xx.HeadShot)) : riders.OrderBy(x => x.OrderBy(xx => xx.HeadShot)));
            //}
            //if (FilterSortingVariable.RIDER_WORLDRANKING == column)
            //{
            //     riders = (sort == "desc" ? riders.OrderBy(x => x.OrderByDescending(xx => xx.WorldRanking)) : riders.OrderBy(x => x.OrderBy(xx => xx.WorldRanking)));
            //}

            return await Task.FromResult(new Tuple<IEnumerable<RiderDto>, int>(riders
                 .GetPage(start, length, out count).Select(y => new RiderDto
                 {
                     Id = y.Id,
                     HeadShot = 0,
                     WorldRanking = 0,
                     Name = y.Name,
                     CountryName = "",
                     Outs = y.Mounted,
                     RiderId = y.RiderId
                 }), count));
        }

        /// <summary>
        /// Get Rider By Id
        /// </summary>
        /// <param name="id">A Rider Id</param>
        /// <returns>The RiderDto</returns>
        public async Task<RiderDto> GetRiderById(int id)
        {
            var rider = _repoRider.Query()
                 .Filter(x => x.Id == id)
                 .Get()
                 .SingleOrDefault();
            return await Task.FromResult(RiderMapper.Map(rider));
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
    }
}
