using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RR.AdminData;
using RR.Core;
using RR.Data;
using RR.Dto;
using RR.Dto.Event;
using RR.Repo;
using RR.Service.User;
using RR.StaticData;
using RR.StaticMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RR.Service
{
    public class RiderService : IRiderService
    {
        #region Constructor

        private readonly IRepository<Rider, RankRideStaticContext> _repoRider;
        private readonly IRepository<RiderManager, RankRideAdminContext> _repoRiderManager;
        private readonly IRepository<EventRider, RankRideStaticContext> _repoEventRider;
        private readonly IRepository<Event, RankRideStaticContext> _repoEvents;
        private readonly IRepository<LongTermTeam, RankRideContext> _repoLongTermTeam;
        private readonly IRepository<LongTermTeamRider, RankRideContext> _repoLongTermTeamRider;
        private readonly IRepository<PicuresManager, RankRideAdminContext> _repoPictures;
        private readonly IBullRiderPicturesService _bullRiderPicturesService;
        private readonly AppSettings _appsettings;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IRepository<FavoriteBullRiders, RankRideContext> _repoFavoriteBullRiders;

        public RiderService(IRepository<Rider, RankRideStaticContext> repoRider, IHttpContextAccessor httpContext,
            IRepository<FavoriteBullRiders, RankRideContext> repoFavoriteBullRiders,
            IRepository<LongTermTeam, RankRideContext> repoLongTermTeam,
            IRepository<LongTermTeamRider, RankRideContext> repoLongTermTeamRider,
            IRepository<PicuresManager, RankRideAdminContext> repoPictures,
            IBullRiderPicturesService bullRiderPicturesService,
            IRepository<EventRider, RankRideStaticContext> repoEventRider,
            IRepository<Event, RankRideStaticContext> repoEvents,
            IRepository<RiderManager, RankRideAdminContext> repoRiderManager,
            IOptions<AppSettings> appsettings
            )
        {
            _repoRider = repoRider;
            _httpContext = httpContext;
            _repoFavoriteBullRiders = repoFavoriteBullRiders;
            _repoLongTermTeam = repoLongTermTeam;
            _repoLongTermTeamRider = repoLongTermTeamRider;
            _appsettings = appsettings.Value;
            _repoPictures = repoPictures;
            _bullRiderPicturesService = bullRiderPicturesService;
            _repoEventRider = repoEventRider;
            _repoEvents = repoEvents;
            _repoRiderManager = repoRiderManager;
        }

        #endregion
        
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
           .And(x => x.IsDelete == false
                && (searchStr == ""
                || x.Name.ToLower().Contains(searchStr.ToLower())
                || x.Hand.Contains(searchStr.ToLower())
                || x.RiderId.ToString().Contains(searchStr.ToLower())
                || x.Id.ToString().Contains(searchStr.ToLower())));

            //predicate = predicate.And(x => x.Cwrp  > 0);

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
            if (3 == column)
            {
                //riders = (sort == "desc" ? riders.OrderBy(x => x.OrderByDescending(xx => xx.Cwrp)) : riders.OrderBy(x => x.OrderBy(xx => xx.Cwrp)));
                riders = sort == "asc" ? riders.OrderBy(x => x.OrderByDescending(xx => xx.Cwrp.HasValue)) : riders.OrderBy(x => x.OrderBy(xx => xx.Cwrp));
                
            }
            
            //if (FilterSortingVariable.RIDER_HEADSHOT == column)
            //{
            //     riders = (sort == "desc" ? riders.OrderBy(x => x.OrderByDescending(xx => xx.HeadShot)) : riders.OrderBy(x => x.OrderBy(xx => xx.HeadShot)));
            //}
            //if (FilterSortingVariable.RIDER_WORLDRANKING == column)
            //{
            //     riders = (sort == "desc" ? riders.OrderBy(x => x.OrderByDescending(xx => xx.WorldRanking)) : riders.OrderBy(x => x.OrderBy(xx => xx.WorldRanking)));
            //}    


            var userId = _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<int> addedRiderLLT = new List<int>();

            var userLongTermTeam = _repoLongTermTeam.Query().Filter(r => r.UserId == userId).Get().SingleOrDefault();
            if (userLongTermTeam != null)
            {
                addedRiderLLT = _repoLongTermTeamRider.Query().Filter(r => r.TeamId == userLongTermTeam.Id).Get().Select(r => r.RiderId).ToList();
            }

            return await Task.FromResult(new Tuple<IEnumerable<RiderDto>, int>(riders
                 .GetPage(start, length, out count).Select(y => new RiderDto
                 {
                     Id = y.Id,
                     HeadShot = 0,
                     WorldRanking = y.Cwrp ?? 9999,
                     Name = y.Name,                     
                     CountryName = "N/AA",
                     Outs = y.Mounted,
                     RiderId = y.RiderId,
                     RiderPower = y.RiderPower,
                     IsAddedFavorite = (_repoFavoriteBullRiders.Query().Filter(x => x.UserId == userId && x.RiderId == y.RiderId).Get().Count() > 0),
                     isAddedInLongTermTeam = addedRiderLLT.Contains(y.Id),
                     LTTeamIcon = userLongTermTeam?.TeamIcon,
                     LTTeamName = userLongTermTeam?.TeamBrand,
                     Avatar = GetRiderPic(y.RiderId).Result,
                     WorldRankPoint = 0,
                     RRTotalpoint = GetRiderTotalPoints(y.RiderId).Result
                 }), count));
        }
        public async Task<string> UpdateRedisCache()
        {
            MyRedisConnectorHelper redisHelper = new MyRedisConnectorHelper(_appsettings);
            if (!redisHelper.RedisConnected())
            {
                return await Task.FromResult("Not Connected Redis Cache");
            }
            int count = 0;
            int start = 0;
            int length = 10;
            int i = 0;
            string searchStr = "";
            string[] sorts = { "", "desc" };
            Tuple<IEnumerable<RiderDto>, int> resultTemp;
            List<Tuple<string, string>> resultRiders = new List<Tuple<string, string>>();
            while (start <= count)
            {
                for (int column = 0; column <= 5; column++)
                {
                    foreach (string sort in sorts)
                    {
                        var predicate = PredicateBuilder.True<Rider>()
                       .And(x => x.IsDelete == false && (searchStr == ""
                            || x.Name.ToLower().Contains(searchStr.ToLower())
                            || x.Hand.Contains(searchStr.ToLower())
                            || x.RiderId.ToString().Contains(searchStr.ToLower())
                            || x.Id.ToString().Contains(searchStr.ToLower())));

                        predicate = predicate.And(x => x.Cwrp > 0);

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
                        if (3 == column)
                        {
                            riders = (sort == "desc" ? riders.OrderBy(x => x.OrderByDescending(xx => xx.Cwrp)) : riders.OrderBy(x => x.OrderBy(xx => xx.Cwrp)));
                        }
                        if (4 == column)
                        {

                        }
                        var evenResults = _repoEvents.Query().Filter(d => d.StartDate.Year == DateTime.Now.Year && !string.IsNullOrEmpty(d.EventResult)).Get().SelectMany(m => JsonConvert.DeserializeObject<IEnumerable<EventResultDto>>(m.EventResult)).GroupBy(r => r.rider_id).Select(r => new { key = r.Key, point = r.Sum(g => g.rr_rider_score) });

                        if (5 == column)
                        {
                            if (sort == "desc")
                            {
                                var rrScorids = evenResults.OrderByDescending(r => r.point).Select(d => d.key).ToList();
                                riders = riders.OrderBy(x => x.OrderBy(xx => rrScorids.Concat(x.Select(g => g.RiderId)).ToList().IndexOf(xx.RiderId)));
                            }
                            else
                            {
                                var rrScorids = evenResults.OrderBy(r => r.point).Select(d => d.key).ToList();
                                riders = riders.OrderBy(x => x.OrderBy(xx => rrScorids.Concat(x.Select(g => g.RiderId)).ToList().IndexOf(xx.RiderId)));
                            }

                        }

                        var userId = _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                        IEnumerable<int> addedRiderLLT = new List<int>();
                        var userLongTermTeam = _repoLongTermTeam.Query().Filter(r => r.UserId == userId).Get().SingleOrDefault();
                        if (userLongTermTeam != null)
                        {
                            addedRiderLLT = _repoLongTermTeamRider.Query().Filter(r => r.TeamId == userLongTermTeam.Id).Get().Select(r => r.RiderId);
                        }

                        var favRider = _repoFavoriteBullRiders.Query().Filter(x => x.UserId == userId).Get().Select(r => r.RiderId);

                        resultTemp = new Tuple<IEnumerable<RiderDto>, int>(riders
                             .GetPage(i, length, out count).Select(y => new RiderDto
                             {
                                 Id = y.Id,
                                 HeadShot = 0,
                                 WorldRanking = y.Cwrp ?? 9999,
                                 Name = y.Name,
                                 CountryName = "UN",
                                 Outs = y.Mounted,
                                 RiderId = y.RiderId,
                                 RiderPower = y.RiderPower,
                                 IsAddedFavorite = (favRider != null && favRider.Contains(y.RiderId)),
                                 isAddedInLongTermTeam = addedRiderLLT.Contains(y.Id),
                                 LTTeamIcon = userLongTermTeam?.TeamIcon,
                                 LTTeamName = userLongTermTeam?.TeamBrand,
                                 // Avatar = GetRiderPic(y.RiderId).GetAwaiter().GetResult(),
                                 WorldRankPoint = 0,
                                 RRTotalpoint = evenResults.FirstOrDefault(r => r.key == y.RiderId)?.point ?? 0
                             }), count);

                        resultRiders.Add(new Tuple<string, string>($"-riders-page-{i.ToString()}-{column.ToString()}-{sort}", JsonConvert.SerializeObject(resultTemp)));
                        //redisHelper.SaveRedisRidersPageList(resultTemp, i, column, sort);
                        
                    }
                }
                i++;
                start = i * length + 1;
            }
            redisHelper.SaveRedisRidersAll(resultRiders);
            return await Task.FromResult("Updated Redis Cache");
        }
        public async Task<Tuple<IEnumerable<RiderDto>, int>> GetAllRidersShort(int start, int length, int column, string searchStr = "", string sort = "")
        {
            MyRedisConnectorHelper redisHelper = new MyRedisConnectorHelper(_appsettings);
            Tuple<IEnumerable<RiderDto>, int> resultTemp;
            if ((searchStr == "" && !redisHelper.ExistRedisRidersPageList(start, column, sort)) || searchStr != "")
            {
                int count = 0;

                var predicate = PredicateBuilder.True<Rider>()
               .And(x => x.IsDelete == false && (searchStr == ""
                    || x.Name.ToLower().Contains(searchStr.ToLower())
                    || x.Hand.Contains(searchStr.ToLower())
                    || x.RiderId.ToString().Contains(searchStr.ToLower())
                    || x.Id.ToString().Contains(searchStr.ToLower())));

                predicate = predicate.And(x => x.Cwrp > 0);

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
                if (3 == column)
                {
                    riders = (sort == "desc" ? riders.OrderBy(x => x.OrderByDescending(xx => xx.Cwrp)) : riders.OrderBy(x => x.OrderBy(xx => xx.Cwrp)));
                }
                if (4 == column)
                {
               
                }
                var evenResults = _repoEvents.Query().Filter(d => d.StartDate.Year == DateTime.Now.Year && !string.IsNullOrEmpty(d.EventResult)).Get().SelectMany(m => JsonConvert.DeserializeObject<IEnumerable<EventResultDto>>(m.EventResult)).GroupBy(r => r.rider_id).Select(r => new { key = r.Key, point = r.Sum(g => g.rr_rider_score) });

                if (5 == column)
                {
                    if (sort == "desc")
                    {
                        var rrScorids = evenResults.OrderByDescending(r => r.point).Select(d => d.key).ToList();
                        riders = riders.OrderBy(x => x.OrderBy(xx => rrScorids.Concat(x.Select(g => g.RiderId)).ToList().IndexOf(xx.RiderId)));
                    }
                    else
                    {
                        var rrScorids = evenResults.OrderBy(r => r.point).Select(d => d.key).ToList();
                        riders = riders.OrderBy(x => x.OrderBy(xx => rrScorids.Concat(x.Select(g => g.RiderId)).ToList().IndexOf(xx.RiderId)));
                    }

                }

                var userId = _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                IEnumerable<int> addedRiderLLT = new List<int>();
                var userLongTermTeam = _repoLongTermTeam.Query().Filter(r => r.UserId == userId).Get().SingleOrDefault();
                if (userLongTermTeam != null)
                {
                    addedRiderLLT = _repoLongTermTeamRider.Query().Filter(r => r.TeamId == userLongTermTeam.Id).Get().Select(r => r.RiderId);
                }

                var favRider = _repoFavoriteBullRiders.Query().Filter(x => x.UserId == userId).Get().Select(r => r.RiderId);

                resultTemp = new Tuple<IEnumerable<RiderDto>, int>(riders
                     .GetPage(start, length, out count).Select(y => new RiderDto
                 {
                     Id = y.Id,
                    HeadShot = 0,
                    WorldRanking = y.Cwrp ?? 9999,
                    Name = y.Name,
                    CountryName = "UN",
                    Outs = y.Mounted,
                    RiderId = y.RiderId,
                    RiderPower = y.RiderPower,
                    IsAddedFavorite = (favRider != null && favRider.Contains(y.RiderId)),
                    isAddedInLongTermTeam = addedRiderLLT.Contains(y.Id),
                    LTTeamIcon = userLongTermTeam?.TeamIcon,
                    LTTeamName = userLongTermTeam?.TeamBrand,
                    // Avatar = GetRiderPic(y.RiderId).GetAwaiter().GetResult(),
                    WorldRankPoint = 0,
                    RRTotalpoint = evenResults.FirstOrDefault(r => r.key == y.RiderId)?.point ?? 0
                 }), count);
                if (redisHelper.RedisConnected() && searchStr == "")
                {
                    redisHelper.SaveRedisRidersPageList(resultTemp, start, column, sort);
                }
                return await Task.FromResult(resultTemp);
            }
            else
            {
                return await Task.FromResult(redisHelper.ReadRedisRidersPageList(start, column, sort));
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
        public async Task<Tuple<IEnumerable<RiderEventDto>, int,decimal?>> GetAllRiderEvents(int riderid, int start, int length, int column, string searchStr = "", string sort = "")
        {
            int count = 0;

            var predicate = PredicateBuilder.True<EventRider>()
           .And(x => x.IsDelete == false && x.RiderId == riderid && (searchStr == ""
                || x.Event.Title.ToLower().Contains(searchStr.ToLower())
                || x.Event.Location.Contains(searchStr.ToLower())
                || x.Event.Season.ToString().Contains(searchStr.ToLower())
                || x.RiderId.ToString().Contains(searchStr.ToLower())
                || x.Id.ToString().Contains(searchStr.ToLower())));

            var riders = _repoEventRider
                .Query().Includes(d => d.Include(m => m.Event).Include(r => r.Rider))
                .Filter(predicate);

            if (0 == column)
            {
                riders = (sort == "desc" ? riders.OrderBy(x => x.OrderByDescending(xx => xx.Event.Season)) : riders.OrderBy(x => x.OrderBy(xx => xx.Event.Season)));
            }
            if (2 == column)
            {
                riders = (sort == "desc" ? riders.OrderBy(x => x.OrderByDescending(xx => xx.Event.StartDate)) : riders.OrderBy(x => x.OrderBy(xx => xx.Event.StartDate)));
            }
            if (3 == column)
            {
                riders = (sort == "desc" ? riders.OrderBy(x => x.OrderByDescending(xx => xx.Event.Location)) : riders.OrderBy(x => x.OrderBy(xx => xx.Event.Location)));
            }
            if (4 == column)
            {
                riders = (sort == "desc" ? riders.OrderBy(x => x.OrderByDescending(xx => xx.Event.Type)) : riders.OrderBy(x => x.OrderBy(xx => xx.Event.Type)));
            }

            var totalPoints = riders.Get().Select(s =>  !string.IsNullOrEmpty(s.Event.EventResult) ? JsonConvert.DeserializeObject<List<EventResultDto>>(s.Event.EventResult).FirstOrDefault(d => d.rider_id == s.Rider.RiderId)?.rr_rider_score.ToRound() : 0);
            return await Task.FromResult(new Tuple<IEnumerable<RiderEventDto>, int,decimal?>(riders
                 .GetPage(start, length, out count).Select(s => new RiderEventDto
                 {
                     EventName = s.Event.Title,
                     Date = s.Event.StartDate.ToString("MMMM dd"),
                     Series = "",
                     Season = s.Event.Season,
                     Location = s.Event.Location,
                     EventTpe = s.Event.Type,
                     Place = "",
                     Point = !string.IsNullOrEmpty(s.Event.EventResult) ? JsonConvert.DeserializeObject<List<EventResultDto>>(s.Event.EventResult).FirstOrDefault(d => d.rider_id == s.Rider.RiderId)?.rr_rider_score.ToRound().ToString() : "",
                 }), count, totalPoints.Sum()));
        }


        public async Task<IEnumerable<RiderDto>> GetCompleteRiders(string userId = "")
        {

            List<int> addedRiderLLT = new List<int>();

            var userLongTermTeam = _repoLongTermTeam.Query().Filter(r => r.UserId == userId).Get().SingleOrDefault();
            if (userLongTermTeam != null)
            {
                addedRiderLLT = _repoLongTermTeamRider.Query().Filter(r => r.TeamId == userLongTermTeam.Id).Get().Select(r => r.RiderId).ToList();
            }


            var ridersdata = _repoRider
                  .Query().Get().Select(riders => new RiderDto
                  {
                      Id = riders.Id,
                      Name = riders.Name,
                      Hand = riders.Hand,
                      RidePerc = Math.Round(riders.RidePerc, 2),
                      RidePrecCurent = Math.Round(riders.RidePrecCurent, 2),
                      Streak = Math.Round(riders.Streak, 2),
                      RiderPower = Math.Round(riders.RiderPower, 2),
                      RiderPowerCurrent = Math.Round(riders.RiderPowerCurrent, 2),
                      Rode = riders.Rode,
                      Mounted = riders.Mounted,
                      RiderId = riders.RiderId,
                      CWRP = riders.Cwrp ?? 9999,
                      IsAddedFavorite = (_repoFavoriteBullRiders.Query().Filter(x => x.UserId == userId && x.RiderId == riders.RiderId).Get().Count() > 0),
                      isAddedInLongTermTeam = addedRiderLLT.Contains(riders.Id),
                      LTTeamIcon = addedRiderLLT.Contains(riders.Id) ? string.Concat(_appsettings.MainSiteURL, userLongTermTeam?.TeamIcon) : "",
                      LTTeamName = addedRiderLLT.Contains(riders.Id) ? userLongTermTeam?.TeamBrand : "",
                      Avatar = GetRiderPic(riders.RiderId, _appsettings.MainSiteURL).Result

                  });
            return await Task.FromResult(ridersdata);
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

            var rider_ = RiderMapper.Map(rider);
            rider_.RiderManager = new RidermanagerDto();
            rider_.RiderManager.SocialLinks = _repoRiderManager.Query().Filter(d => d.RiderId == rider.RiderId).Get().Select(r => new RiderSocialLinksDto
            {
                Icon = r.Icon,
                Type = (SocialType)Enum.Parse(typeof(SocialType), r.Type),
                Sociallink = r.Sociallink,
                Id = r.Id
            }).ToList();
            rider_.Avatar = GetRiderPic(rider_.RiderId).Result;
            return await Task.FromResult(rider_);
        }

        public async Task<string> GetRiderPic(int riderId, string basePath = "")
        {
            return await _bullRiderPicturesService.GetRiderPic(riderId, basePath);
        }

        private async Task<decimal> GetRiderTotalPoints(int riderId)
        {
            var events_ = _repoEvents.Query().Filter(d => d.StartDate.Year == DateTime.Now.Year && !string.IsNullOrEmpty(d.EventResult)).Get().SelectMany(m => JsonConvert.DeserializeObject<IEnumerable<EventResultDto>>(m.EventResult));
            var totalRRPoint = (events_.Where(d => d.rider_id == riderId).Select(m => m.rr_rider_score).Sum()).ToRound();

            //var totalRRPoint = _repoEvents.Query().Filter(d => d.StartDate.Year == DateTime.Now.Year && !string.IsNullOrEmpty(d.EventResult)).Get().SelectMany(m => JArray.Parse(m.EventResult).Where(d => d.SelectToken("rider_id").ToString() == riderId.ToString()).Select(d => (decimal)d.SelectToken("rr_rider_score"))).Sum().ToRound();

            return await Task.FromResult(totalRRPoint);
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
