using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RR.AdminData;
using RR.Core;
using RR.Data;
using RR.Dto;
using RR.Dto.Event;
using RR.Repo;
using RR.StaticData;
using RR.StaticMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RR.Service
{
    public class BullService : IBullService
    {
        #region Constructor

        private readonly IRepository<Bull, RankRideStaticContext> _repoBull;
        private readonly IRepository<EventBull, RankRideStaticContext> _repoEventBull;
        private readonly IRepository<Event, RankRideStaticContext> _repoEvent;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IRepository<FavoriteBullRiders, RankRideContext> _repoFavoriteBullRiders;
        private readonly IRepository<LongTermTeam, RankRideContext> _repoLongTermTeam;
        private readonly IRepository<LongTermTeamBull, RankRideContext> _repoLongTermTeamBull;
        private readonly IRepository<PicuresManager, RankRideAdminContext> _repoPictures;
        private readonly IBullRiderPicturesService _bullRiderPicturesService;
        private readonly AppSettings _appsettings;

        public BullService(IRepository<Bull, RankRideStaticContext> repoBull, IHttpContextAccessor httpContext,
            IRepository<FavoriteBullRiders, RankRideContext> repoFavoriteBullRiders,
            IRepository<LongTermTeam, RankRideContext> repoLongTermTeam,
            IRepository<LongTermTeamBull, RankRideContext> repoLongTermTeamBull,
            IRepository<PicuresManager, RankRideAdminContext> repoPictures,
            IBullRiderPicturesService bullRiderPicturesService,
            IRepository<EventBull, RankRideStaticContext> repoEventBull,
            IRepository<Event, RankRideStaticContext> repoEvent,
        IOptions<AppSettings> appsettings
            )
        {
            _repoBull = repoBull;
            _httpContext = httpContext;
            _repoFavoriteBullRiders = repoFavoriteBullRiders;
            _repoLongTermTeam = repoLongTermTeam;
            _repoLongTermTeamBull = repoLongTermTeamBull;
            _repoPictures = repoPictures;
            _bullRiderPicturesService = bullRiderPicturesService;
            _appsettings = appsettings.Value;
            _repoEventBull = repoEventBull;
            _repoEvent = repoEvent;
        }

        #endregion

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
            MyRedisConnectorHelper redisHelper = new MyRedisConnectorHelper(_appsettings);
            Tuple<IEnumerable<BullDto>, int> resultTemp;
            if ((searchStr == "" && !redisHelper.ExistRedisBullPageList(start, column, sort)) || searchStr != "")
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
                    bulls = (sort == "desc" ? bulls.OrderBy(x => x.OrderByDescending(xx => xx.BullId)) : bulls.OrderBy(x => x.OrderBy(xx => xx.BullId)));
                }
                if (FilterSortingVariable.BULL_NAME == column)
                {
                    bulls = (sort == "desc" ? bulls.OrderBy(x => x.OrderByDescending(xx => xx.Name)) : bulls.OrderBy(x => x.OrderBy(xx => xx.Name)));
                }
                if (FilterSortingVariable.BULL_OWNER == column)
                {
                    bulls = (sort == "desc" ? bulls.OrderBy(x => x.OrderByDescending(xx => xx.Owner.ToLower())) : bulls.OrderBy(x => x.OrderBy(xx => xx.Owner.ToLower())));
                }
                if (FilterSortingVariable.BULL_POWERATING == column)
                {
                    bulls = (sort == "desc" ? bulls.OrderBy(x => x.OrderByDescending(xx => xx.PowerRating)) : bulls.OrderBy(x => x.OrderBy(xx => xx.PowerRating)));
                }
                if (FilterSortingVariable.BULL_AVERAGEMARK == column)
                {
                    bulls = (sort == "desc" ? bulls.OrderBy(x => x.OrderByDescending(xx => xx.AverageMark)) : bulls.OrderBy(x => x.OrderBy(xx => xx.AverageMark)));
                }

                var userId = _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);


                List<int> addedBullLLT = new List<int>();

                var userLongTermTeam = _repoLongTermTeam.Query().Filter(r => r.UserId == userId).Get().SingleOrDefault();
                if (userLongTermTeam != null)
                {
                    addedBullLLT = _repoLongTermTeamBull.Query().Filter(r => r.TeamId == userLongTermTeam.Id).Get().Select(r => r.BullId).ToList();
                }

                var rrScorids = new List<int>();
                var evenResults = _repoEvent.Query().Filter(d => d.StartDate.Year == DateTime.Now.Year && !string.IsNullOrEmpty(d.EventResult)).Get().SelectMany(m => JsonConvert.DeserializeObject<List<EventResultDto>>(m.EventResult)).GroupBy(r => r.pbid).Select(r => new { key = r.Key, point = r.Sum(g => g.rr_bull_score) });
                if (6 == column)
                {
                    if (sort == "desc")
                    {
                        rrScorids = evenResults.OrderByDescending(r => r.point).Select(d => d.key).ToList();
                        bulls = bulls.OrderBy(x => x.OrderBy(xx => rrScorids.Concat(x.Select(g => g.BullId)).ToList().IndexOf(xx.BullId)));
                    }
                    else
                    {
                        rrScorids = evenResults.OrderBy(r => r.point).Select(d => d.key).ToList();
                        bulls = bulls.OrderBy(x => x.OrderBy(xx => rrScorids.Concat(x.Select(g => g.BullId)).ToList().IndexOf(xx.BullId)));
                    }

                }

                resultTemp = new Tuple<IEnumerable<BullDto>, int>(bulls
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
                         BullId = y.BullId,
                         IsAddedFavorite = (_repoFavoriteBullRiders.Query().Filter(x => x.UserId == userId && x.BullId == y.BullId).Get().Count() > 0),
                         isAddedInLongTermTeam = addedBullLLT.Contains(y.Id),
                         LTTeamIcon = userLongTermTeam?.TeamIcon,
                         LTTeamName = userLongTermTeam?.TeamBrand,
                         Avatar = GetBullPic(y.BullId).Result,
                         WorldStanding = "",
                         RankRideScore = evenResults.FirstOrDefault(r => r.key == y.BullId)?.point ?? 0
                     }), count);
                if (redisHelper.RedisConnected() && searchStr == "")
                {
                    redisHelper.SaveRedisBullPageList(resultTemp, start, column, sort);
                }
                return await Task.FromResult(resultTemp);
            }
            else
            {
                return await Task.FromResult(redisHelper.ReadRedisBullPageList(start, column, sort));
            }
        }

        public async Task<string> UpdateRedisCache()
        {
            MyRedisConnectorHelper redisHelper = new MyRedisConnectorHelper(_appsettings);
            if (!redisHelper.RedisConnected())
            {
                return await Task.FromResult("Not Connected Redis Cache");
            }
            Tuple<IEnumerable<BullDto>, int> resultTemp;
            List<Tuple<string, string>> resultBulls = new List<Tuple<string, string>>();
            int count = 0;
            int start = 0;
            int length = 10;
            int i = 0;
            string searchStr = "";
            string[] sorts = { "", "desc" };
            while (start <= count) {
                for (int column = 0; column <= 6; column++)
                {
                    foreach (string sort in sorts)
                    {
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
                            bulls = (sort == "desc" ? bulls.OrderBy(x => x.OrderByDescending(xx => xx.BullId)) : bulls.OrderBy(x => x.OrderBy(xx => xx.BullId)));
                        }
                        if (FilterSortingVariable.BULL_NAME == column)
                        {
                            bulls = (sort == "desc" ? bulls.OrderBy(x => x.OrderByDescending(xx => xx.Name)) : bulls.OrderBy(x => x.OrderBy(xx => xx.Name)));
                        }
                        if (FilterSortingVariable.BULL_OWNER == column)
                        {
                            bulls = (sort == "desc" ? bulls.OrderBy(x => x.OrderByDescending(xx => xx.Owner.ToLower())) : bulls.OrderBy(x => x.OrderBy(xx => xx.Owner.ToLower())));
                        }
                        if (FilterSortingVariable.BULL_POWERATING == column)
                        {
                            bulls = (sort == "desc" ? bulls.OrderBy(x => x.OrderByDescending(xx => xx.PowerRating)) : bulls.OrderBy(x => x.OrderBy(xx => xx.PowerRating)));
                        }
                        if (FilterSortingVariable.BULL_AVERAGEMARK == column)
                        {
                            bulls = (sort == "desc" ? bulls.OrderBy(x => x.OrderByDescending(xx => xx.AverageMark)) : bulls.OrderBy(x => x.OrderBy(xx => xx.AverageMark)));
                        }

                        //var userId = _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                        string userId = null;

                        List<int> addedBullLLT = new List<int>();

                        var userLongTermTeam = _repoLongTermTeam.Query().Filter(r => r.UserId == userId).Get().SingleOrDefault();
                        if (userLongTermTeam != null)
                        {
                            addedBullLLT = _repoLongTermTeamBull.Query().Filter(r => r.TeamId == userLongTermTeam.Id).Get().Select(r => r.BullId).ToList();
                        }

                        var rrScorids = new List<int>();
                        var evenResults = _repoEvent.Query().Filter(d => d.StartDate.Year == DateTime.Now.Year && !string.IsNullOrEmpty(d.EventResult)).Get().SelectMany(m => JsonConvert.DeserializeObject<List<EventResultDto>>(m.EventResult)).GroupBy(r => r.pbid).Select(r => new { key = r.Key, point = r.Sum(g => g.rr_bull_score) });
                        if (6 == column)
                        {
                            if (sort == "desc")
                            {
                                rrScorids = evenResults.OrderByDescending(r => r.point).Select(d => d.key).ToList();
                                bulls = bulls.OrderBy(x => x.OrderBy(xx => rrScorids.Concat(x.Select(g => g.BullId)).ToList().IndexOf(xx.BullId)));
                            }
                            else
                            {
                                rrScorids = evenResults.OrderBy(r => r.point).Select(d => d.key).ToList();
                                bulls = bulls.OrderBy(x => x.OrderBy(xx => rrScorids.Concat(x.Select(g => g.BullId)).ToList().IndexOf(xx.BullId)));
                            }

                        }

                        resultTemp = new Tuple<IEnumerable<BullDto>, int>(bulls
                             .GetPage(i, length, out count).Select(y => new BullDto
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
                                 BullId = y.BullId,
                                 IsAddedFavorite = (_repoFavoriteBullRiders.Query().Filter(x => x.UserId == userId && x.BullId == y.BullId).Get().Count() > 0),
                                 isAddedInLongTermTeam = addedBullLLT.Contains(y.Id),
                                 LTTeamIcon = userLongTermTeam?.TeamIcon,
                                 LTTeamName = userLongTermTeam?.TeamBrand,
                                 Avatar = GetBullPic(y.BullId).Result,
                                 WorldStanding = "",
                                 RankRideScore = evenResults.FirstOrDefault(r => r.key == y.BullId)?.point ?? 0
                             }), count);

                        resultBulls.Add(new Tuple<string, string>($"-bull-page-{i.ToString()}-{column.ToString()}-{sort}", JsonConvert.SerializeObject(resultTemp)));
                        //redisHelper.SaveRedisBullPageList(resultTemp, i, column, sort);
                    }
                }
                i++;
                start = i * length + 1;
            }
            redisHelper.SaveRedisBullAll(resultBulls);
            return await Task.FromResult("Updated Redis Cache");
        }
        public async Task<Tuple<IEnumerable<BullEventDto>, int>> GetBullEvents(int bullid, int start, int length, int column, string searchStr = "", string sort = "")
        {
            int count = 0;

            var predicate = PredicateBuilder.True<EventBull>()
           .And(x => x.IsDelete == false && x.BullId == bullid && (searchStr == ""
              || x.Bull.Name.ToLower().Contains(searchStr.ToLower())
              || x.Event.Season.ToString().Contains(searchStr.ToLower())
              || x.Event.Title.Contains(searchStr.ToLower())
              || x.BullId.ToString().Contains(searchStr.ToLower())));

            var bulls = _repoEventBull
                .Query().Includes(r=>r.Include(m=>m.Event).Include(s=>s.Bull))
              .Filter(predicate);

            if (0 == column)
            {
                bulls = (sort == "desc" ? bulls.OrderBy(x => x.OrderByDescending(xx => xx.Event.Season)) : bulls.OrderBy(x => x.OrderBy(xx => xx.Event.Season)));
            }

            if (2 == column)
            {
                bulls = (sort == "desc" ? bulls.OrderBy(x => x.OrderByDescending(xx => xx.Event.Title)) : bulls.OrderBy(x => x.OrderBy(xx => xx.Event.Title)));
            }
            if (3 == column)
            {
                bulls = (sort == "desc" ? bulls.OrderBy(x => x.OrderByDescending(xx => xx.Bull.Owner)) : bulls.OrderBy(x => x.OrderBy(xx => xx.Bull.Owner)));
            }

            return await Task.FromResult(new Tuple<IEnumerable<BullEventDto>, int>(bulls
                 .GetPage(start, length, out count).Select(y => new BullEventDto
                 { Buckoftime ="",
                 BullScore ="",
                 Date = y.Event.StartDate.ToString("MMMM dd"),
                 EventName = y.Event.Title,
                 EventTpe = y.Event.Type,
                 Location = y.Event.Location,
                 Rider =y.Bull.Owner,
                 RiderScore ="",
                 Season = y.Event.Season,
                 Series = ""
                 }), count));
        }

        public async Task<IEnumerable<BullDto>> GetCompleteBulls(string userId = "")
        {
            List<int> addedBullLLT = new List<int>();
            var userLongTermTeam = _repoLongTermTeam.Query().Filter(r => r.UserId == userId).Get().SingleOrDefault();
            if (userLongTermTeam != null)
            {
                addedBullLLT = _repoLongTermTeamBull.Query().Filter(r => r.TeamId == userLongTermTeam.Id)
                    .Get().Select(r => r.BullId).ToList();
            }

            var bulls = _repoBull.Query().Get().Select(r=> new BullDto
            {
                Id = r.BullId,
                //Age=bulls.Age,  
                AverageMark = Math.Round(r.AverageMark, 2),
                BuckOffPerc = Math.Round(r.BuckOffPerc, 2),
                //Breeding = bulls.Breeding,
                //BuckingStatistics = bulls.BuckingStatistics,
                //BuckOffStreak =bulls.BuckOffStreak,
                Owner = r.Owner,
                PowerRating = Math.Round(r.PowerRating),
                Name = r.Name,
                CreatedDate = r.CreatedDate,
                IsAddedFavorite = (_repoFavoriteBullRiders.Query().Filter(x => x.UserId == userId && x.BullId == r.BullId).Get().SingleOrDefault()!=null),
                isAddedInLongTermTeam = addedBullLLT.Contains(r.Id),
                LTTeamIcon = addedBullLLT.Contains(r.Id)? string.Concat(_appsettings.MainSiteURL, userLongTermTeam?.TeamIcon) :"",
                LTTeamName = addedBullLLT.Contains(r.Id)?userLongTermTeam?.TeamBrand:"",
                Avatar = GetBullPic(r.BullId, _appsettings.MainSiteURL).Result
            });
            return await Task.FromResult(bulls);
        }

        /// <summary>
        /// Get Bull Record By Id
        /// </summary>
        /// <param name="id">A Bull Id</param>
        /// <returns>The BullDto</returns>
        public async Task<BullDto> GetBullById(int id)
        {
            var bull = _repoBull.Query()
                 .Filter(x => x.Id == id)
                 .Get()
                 .SingleOrDefault();

            var bull_ = BullMapper.Map(bull);
            bull_.Avatar = GetBullPic(bull_.Id).Result;

            return await Task.FromResult(bull_);
        }

        /// <summary>
        /// Get Bull Record By Id
        /// </summary>
        /// <param name="id">A DB Id</param>
        /// <returns>The BullDto</returns>
        public async Task<BullDto> GetBullRecordById(int id)
        {
            var bull = _repoBull.Query()
                    .Filter(x => x.Id == id)
                    .Get()
                    .SingleOrDefault();

            var bull_ = BullMapper.Map(bull);
            bull_.Avatar = await GetBullPic(bull_.Id);

            return await Task.FromResult(bull_);
        }

        public async Task<string> GetBullPic(int bullId, string baseUrl = "")
        {
            return await _bullRiderPicturesService.GetBullPic(bullId, baseUrl);
        }

        private async Task<decimal> GetBullTotalPoints(int bullId)
        {
            var events_ = _repoEvent.Query().Filter(d => d.StartDate.Year == DateTime.Now.Year && !string.IsNullOrEmpty(d.EventResult)).Get().SelectMany(m => JsonConvert.DeserializeObject<List<EventResultDto>>(m.EventResult));

            
            var bullPointss_ = (events_.Where(d => d.pbid == bullId).Select(m => m.rr_bull_score).Sum()).ToRound();
            return await Task.FromResult(bullPointss_);
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
