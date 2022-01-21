using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RR.AdminData;
using RR.Core;
using RR.Data;
using RR.Dto;
using RR.Dto.Contest;
using RR.Dto.Team;
using RR.Mapper;
using RR.Repo;
using RR.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RR.Service
{
    public class ContestService : IContestService
    {
        #region Constructor

        private readonly IRepository<Contest, RankRideAdminContext> _repoContest;
        private readonly IRepository<ContestCategory, RankRideAdminContext> _repoContestCategory;
        private readonly IRepository<Award, RankRideAdminContext> _repoAward;
        private readonly IRepository<AwardType, RankRideAdminContext> _repoAwardType;
        private readonly IRepository<ContestWinner, RankRideAdminContext> _repoContestWinner;
        private readonly IRepository<JoinedContest, RankRideContext> _repoJoinedContest;
        private readonly IRepository<UserDetail, RankRideContext> _repoUsers;
        private readonly IRepository<Team, RankRideContext> _repoTeam;
        private readonly IRepository<Event, RankRideStaticContext> _repoEvent;
        private readonly IRepository<TeamRider, RankRideContext> _repoTeamRider;
        private readonly IRepository<TeamBull, RankRideContext> _repoTeamBull;
        private readonly IRepository<Rider, RankRideStaticContext> _repoRider;
        private readonly IRepository<Bull, RankRideStaticContext> _repoBull;
        private readonly IRepository<EventBull, RankRideStaticContext> _repoEventBull;
        private readonly IRepository<EventRider, RankRideStaticContext> _repoEventRider;
        private readonly IRepository<ContestUserWinner, RankRideContext> _repoContestUserWinner;
        private readonly IRepository<UserDetail, RankRideContext> _repoUserDetail;
        public static AppSettings _appSettings;

        private readonly IHttpContextAccessor _httpContext;
        private readonly IRepository<FavoriteBullRiders, RankRideContext> _repoFavoriteBullRiders;
        
        public ContestService(IRepository<Contest, RankRideAdminContext> repoContest,
             IRepository<ContestCategory, RankRideAdminContext> repoContestCategory,
             IRepository<Award, RankRideAdminContext> repoAward,
             IRepository<AwardType, RankRideAdminContext> repoAwardType,
             IRepository<ContestWinner, RankRideAdminContext> repoContestWinner,
             IRepository<JoinedContest, RankRideContext> repoJoinedContest,
             IRepository<UserDetail, RankRideContext> repoUsers,
             IRepository<Team, RankRideContext> repoTeam,
             IRepository<Event, RankRideStaticContext> repoEvent,
             IRepository<TeamRider, RankRideContext> repoTeamRider,
             IRepository<TeamBull, RankRideContext> repoTeamBull,
             IRepository<Rider, RankRideStaticContext> repoRider,
             IRepository<Bull, RankRideStaticContext> repoBull,
             IRepository<EventBull, RankRideStaticContext> repoEventBull,
             IRepository<EventRider, RankRideStaticContext> repoEventRider,
             IRepository<ContestUserWinner, RankRideContext> repoContestUserWinner,
             IRepository<UserDetail, RankRideContext> repoUserDetail,
             IOptions<AppSettings> appSettings,
             IRepository<FavoriteBullRiders, RankRideContext> repoFavoriteBullRiders,
             IHttpContextAccessor httpContext
            )
        {
            _repoContest = repoContest;
            _repoContestCategory = repoContestCategory;
            _repoAward = repoAward;
            _repoAwardType = repoAwardType; 
            _repoContestWinner = repoContestWinner;
            _repoJoinedContest = repoJoinedContest;
            _repoUsers = repoUsers;
            _repoTeam = repoTeam;
            _repoEvent = repoEvent;
            _repoTeamRider = repoTeamRider;
            _repoTeamBull = repoTeamBull;
            _repoRider = repoRider;
            _repoBull = repoBull;
            _repoEventBull = repoEventBull;
            _repoEventRider = repoEventRider;
            _repoContestUserWinner = repoContestUserWinner;
            _repoUserDetail = repoUserDetail;
            _appSettings = appSettings.Value;
            _httpContext = httpContext;
            _repoFavoriteBullRiders = repoFavoriteBullRiders;
        }
        #endregion

        public async Task<Tuple<IEnumerable<ContestLiteDto>, decimal, string>> GetContestOfEvent(int eventId, string eventName)
        {
            var contests = _repoContest.Query()
                 .Filter(x => x.EventId == eventId
                 && x.UniqueCode.Substring(0, x.UniqueCode.IndexOf("-")) != "PRC"
                 && x.IsActive == true && x.ContestWinner.Count >= 0
                 )
                 .Includes(u => u.Include(uu => uu.ContestCategory).Include(at => at.ContestWinner))
              .Get();
            decimal largestjoiningFee = 0;
            if (contests != null && contests.Count() > 0)
            {
                largestjoiningFee = contests.Max(x => x.JoiningFee);
            }
            return await Task.FromResult(new Tuple<IEnumerable<ContestLiteDto>, decimal, string>(
                 ContestMapper.Map<ContestLiteDto>(contests), largestjoiningFee, eventName));
        }

        public async Task<Tuple<IEnumerable<UserContestLiteDto>, int>> GetContestOfCurrentUser(string userId, int start = 0, int length = 10)
        {
            var contests = (from jc in _repoJoinedContest.Query().Get()
                            join c in _repoContest.Query().Get() on jc.ContestId equals c.Id
                            join u in _repoUsers.Query().Get() on jc.UserId equals u.UserId
                            join t in _repoTeam.Query().Get() on jc.TeamId equals t.Id
                            join e in _repoEvent.Query().Get() on c.EventId equals e.Id
                            where u.UserId == userId
                            select new
                            {
                                EventStatus = ((e.PerfTimeUTC != null && e.PerfTimeUTC < DateTime.UtcNow) && (e.PerfTimeUTC != null && e.PerfTime < DateTime.UtcNow)) ? "Upcoming" : "Completed",
                                ContestName = c.Title,
                                u.UserName,
                                JoiningDate = jc.CreatedDate,
                                t.TeamPoint,
                                u.UserId,
                                jc.ContestId,
                                jc.TeamId,
                                c.JoiningFee,
                                e.Id,
                                u.Avtar,
                                u.TeamName
                            }).ToList();
            var data = contests.OrderByDescending(x => x.JoiningDate).Skip(start).Take(length);
            int recordsTotal = contests.Count();
            List<UserContestLiteDto> userContests = new List<UserContestLiteDto>();
            foreach (var x in data)
            {
                var tmpRank = GetContestRankAwardByUserId(x.ContestId, x.TeamId, x.UserId, x.TeamPoint).Result;
                var tmp = new UserContestLiteDto
                {
                    UserId = x.UserId,
                    ContestId = x.ContestId,
                    TeamId = x.TeamId,
                    EventStatus = x.EventStatus,
                    JoiningFee = x.JoiningFee,
                    Username = x.UserName,
                    JoiningDate = x.JoiningDate,
                    TeamPoint = x.TeamPoint,
                    ContestName = x.ContestName,
                    EventId = x.Id,
                    TeamName = x.TeamName,
                    Rank = tmpRank.Item1,
                    IsAward = tmpRank.Item2,
                    JoiningDateUTCString = x.JoiningDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                    Avtar = !string.IsNullOrEmpty(x.Avtar) ? (x.Avtar.Contains("https://") ? x.Avtar : (x.Avtar != "/images/RR/user-n.png" ? (_appSettings.MainSiteURL + "/images/profilePicture/" + x.Avtar) : _appSettings.MainSiteURL + "/images/home/team-icon.png")) : _appSettings.MainSiteURL + "/images/home/team-icon.png"
                };
                userContests.Add(tmp);
            }
            return await Task.FromResult(new Tuple<IEnumerable<UserContestLiteDto>, int> (userContests, recordsTotal));
        }
        public async Task<Tuple<int, bool>> GetContestRankAwardByUserId(int contestId, int teamId, string userId, decimal teamPoint)
        {
            
            bool bAward = false;
            var entries = (from jc in _repoJoinedContest.Query()
                              .Includes(xx => xx.Include(x => x.Team))
                              .Get().Where(r => r.IsDelete != true)
                          where jc.ContestId == contestId
                          select new
                          {
                              jc.Team.TeamPoint,
                              jc.ContestId,
                              jc.UserId,
                              jc.TeamId,
                          }).Distinct().ToList();
            
            var position = entries.Count(x => x.TeamPoint > teamPoint) + 1;
            var userWinnerRank = _repoContestUserWinner.Query()
                        .Filter(x => x.UserId == userId && x.TeamId == teamId && x.ContestId == contestId)
                        .Get().Select(x => x.TeamRank).SingleOrDefault();
            if (userWinnerRank > 0)
            {
                position = userWinnerRank;
                bAward = true;
            }
           
            
            return await Task.FromResult(new Tuple<int, bool>(position, bAward));
        }
        public async Task<IEnumerable<UserContestLiteDto>> GetContestStandingsApi(string userId)
        {
            var contests = (from jc in _repoJoinedContest.Query().Get()
                            join c in _repoContest.Query().Get() on jc.ContestId equals c.Id
                            join u in _repoUsers.Query().Get() on jc.UserId equals u.UserId
                            join ud in _repoUserDetail.Query().Get() on jc.UserId equals ud.UserId
                            join t in _repoTeam.Query().Get() on jc.TeamId equals t.Id
                            join e in _repoEvent.Query().Get() on c.EventId equals e.Id
                            where u.UserId == userId
                            select new
                            {
                                EventStatus = e.StartDate < DateTime.Now ? "Completed" : "Upcoming",
                                ContestName = c.Title,
                                u.UserName,
                                JoiningDate = jc.CreatedDate,
                                t.TeamPoint,
                                u.UserId,
                                jc.ContestId,
                                jc.TeamId,
                                c.JoiningFee,
                                e.Id,
                                ud.Avtar
                            }).ToList();

            //Calculate rank for joined team for contest.
            var rankResult = (from a in contests.OrderByDescending(x => x.TeamPoint)
                              select new
                              {
                                  Rank = contests.Count(x => x.TeamPoint > a.TeamPoint) + 1,
                                  a.EventStatus,
                                  a.ContestName,
                                  a.UserName,
                                  a.TeamPoint,
                                  a.UserId,
                                  a.ContestId,
                                  a.TeamId,
                                  a.JoiningFee,
                                  a.Id,
                                  a.JoiningDate,
                                  a.Avtar
                              }).ToList();

            return await Task.FromResult(rankResult.Select(x => new UserContestLiteDto
            {
                Rank = x.Rank,
                UserId = x.UserId,
                ContestId = x.ContestId,
                TeamId = x.TeamId,
                EventStatus = x.EventStatus,
                JoiningFee = x.JoiningFee,
                Username = x.UserName,
                JoiningDate = x.JoiningDate,
                TeamPoint = x.TeamPoint,
                ContestName = x.ContestName,
                EventId = x.Id,
                Avtar = !string.IsNullOrEmpty(x.Avtar) ? (x.Avtar.Contains("https://") ? x.Avtar : (x.Avtar != "/images/RR/user-n.png" ? (_appSettings.MainSiteURL + "/images/profilePicture/" + x.Avtar) : _appSettings.MainSiteURL + "/images/RR/user-n.png")) : _appSettings.MainSiteURL + "/images/RR/user-n.png"
            }));
        }

        public async Task<Tuple<IEnumerable<RiderContestLiteDto>, IEnumerable<BullContestLiteDto>, int, int>> GetContestDetailOfCurrentUser
               (int eventId, int contestId, int teamId)
        {
            var userId = _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var favRiderBull = _repoFavoriteBullRiders.Query().Filter(x => x.UserId == userId).Get();
            var favRider = favRiderBull.Select(r => r.RiderId);
            var favBull = favRiderBull.Select(b => b.BullId);
            var contestJoinEntries = (from jc in _repoJoinedContest.Query()
                                      .Includes(xx => xx.Include(x => x.Team))
                                      .Get().Where(r => r.IsDelete != true)
                                    where jc.ContestId == contestId
                                    select new
                                    {
                                        jc.Team.TeamPoint,
                                        jc.ContestId,
                                        jc.UserId,
                                        jc.TeamId,
                                          
                                    }).Distinct().ToList();
            var numberOfEntries = contestJoinEntries.Count();
            var teamPoint = contestJoinEntries.Where(x => x.TeamId == teamId).Select(x => x.TeamPoint).SingleOrDefault();
            var position = contestJoinEntries.Count(x => x.TeamPoint > teamPoint) + 1;
            
            var contestJoinRiders = (from jc in _repoJoinedContest.Query().Get()
                                     join t in _repoTeam.Query().Get() on jc.TeamId equals t.Id
                                     join tr in _repoTeamRider.Query().Get() on jc.TeamId equals tr.TeamId
                                     join r in _repoRider.Query().Get() on tr.RiderId equals r.Id
                                     join er in _repoEventRider.Query().Get() on tr.RiderId equals er.RiderId
                                     where jc.ContestId == contestId && jc.TeamId == teamId && er.EventId == eventId
                                     select new
                                     {
                                         t.TeamPoint,
                                         jc.ContestId,
                                         jc.TeamId,
                                         tr.RiderId,
                                         RiderName = r.Name,
                                         RiderTier = er.EventTier,
                                         t.ContestType,
                                         SubstituteRider = tr.IsSubstitute ? "Yes" : "No",
                                         joiningDate = jc.CreatedDate.ToString("dd MMM yyyy"),
                                         RiderPoint = tr.RiderPoint,
                                         IsDropout =er.IsDropout
                                     }).Distinct().ToList();

            var contestJoinBulls = (from jc in _repoJoinedContest.Query().Get()
                                    join t in _repoTeam.Query().Get() on jc.TeamId equals t.Id
                                    join tb in _repoTeamBull.Query().Get() on jc.TeamId equals tb.TeamId
                                    join b in _repoBull.Query().Get() on tb.BullId equals b.Id
                                    join er in _repoEventBull.Query().Get() on tb.BullId equals er.BullId
                                    where jc.ContestId == contestId && jc.TeamId == teamId && er.EventId == eventId
                                    select new
                                    {
                                        tb.BullId,
                                        BullName = b.Name,
                                        BullTier = er.EventTier,
                                        SubstituteBull = tb.IsSubstitute ? "Yes" : "No",
                                        BullPoint = tb.BullPoint,
                                        IsDropout = er.IsDropout
                                    }).Distinct().ToList();

            return await Task.FromResult(new Tuple<IEnumerable<RiderContestLiteDto>, IEnumerable<BullContestLiteDto>, int, int>(contestJoinRiders
                                         .Select(x => new RiderContestLiteDto
                                         {
                                             TeamId = x.TeamId,
                                             TeamPoint = x.TeamPoint,
                                             RiderId = x.RiderId,
                                             RiderName = x.RiderName,
                                             RiderTier = x.RiderTier,
                                             SubstituteRider = x.SubstituteRider,
                                             ContestType = Enum.GetName(typeof(Enums.ContestType), x.ContestType),
                                             jDate = x.joiningDate,
                                             RiderPoint = x.RiderPoint,
                                             IsDropout =x.IsDropout??false,
                                             IsAddedFavorite = (favRider.Where(y => y == x.RiderId).Count() > 0),
                                         }).Distinct(), contestJoinBulls.Select(x => new BullContestLiteDto
                                         {
                                             BullId = x.BullId,
                                             BullName = x.BullName,
                                             BullTier = x.BullTier,
                                             SubstituteBull = x.SubstituteBull,
                                             BullPoint = x.BullPoint,
                                             IsDropout = x.IsDropout??false,
                                             IsAddedFavorite = (favBull.Where(y => y == x.BullId).Count() > 0),
                                         }).Distinct(), numberOfEntries, position));
        }
        public async Task<IEnumerable<ContestLiteDto>> FilterContest(int eventId, int priceFrom, int priceTo, int priceFilter)
        {
            var predicate = PredicateBuilder.True<Contest>()
                 .And(x => x.IsActive == true
                 && x.UniqueCode.Substring(0, x.UniqueCode.IndexOf("-")) != "PRC"
                 && x.ContestWinner.Count > 0
                 && x.EventId == eventId
                 && x.JoiningFee >= priceFrom
                 && x.JoiningFee <= priceTo);

            var contests = _repoContest.Query()
                 .Filter(predicate)
                 .Includes(u => u.Include(uu => uu.ContestCategory)
                 .Include(at => at.ContestWinner));
            //.ThenInclude(x => x.Award)
            //.ThenInclude(x => x.AwardType)

            //if (priceFrom >= 0 && priceTo > 0)
            //{
            //     contests = contests.Filter(x => x.JoiningFee >= priceFrom && x.JoiningFee <= priceTo);
            //}


            //Sorting
            contests = (FilterSortingVariable.CONTEST_PRICE_LOWTOHIGH == priceFilter ?
              contests.OrderBy(x => x.OrderBy(y => y.JoiningFee)) :
                         contests.OrderBy(x => x.OrderByDescending(y => y.JoiningFee)));


            return await Task.FromResult(ContestMapper.Map<ContestLiteDto>(contests.Get()));
        }

        public async Task<Tuple<List<AwardLiteDto>, AwardLiteDescriptionDto>> GetContestAwards(int contestId, int eventId)
        {
            AwardLiteDescriptionDto descriptionDto = new AwardLiteDescriptionDto();
            var contestAward = _repoContest.Query()
                 .Filter(x => x.Id == contestId)
                 .Includes(at => at.Include(att => att.ContestWinner))//.ThenInclude(a => a.Award).ThenInclude(cw => cw.AwardType)
                 .Get()
                 .SingleOrDefault();

            var predicate = PredicateBuilder.True<JoinedContest>()
              .And(x => x.IsActive == true)
            .And(x => x.ContestId == contestId)
            .And(x => x.Team.EventId == eventId);

            var joinedContest = _repoJoinedContest.Query()
                  .Filter(predicate)
                  .Includes(tc => tc.Include(tcc => tcc.Team));

            //var numberOfTeamsOfContest = joinedContest
            //.Filter(x => x.ContestId == contestId && x.Team.EventId == eventId && x.UserId == userId)
            //.Get()
            //.Count();

            var numberOfPlayersOfContest = joinedContest
                 .Get()
                 //.GroupBy(x => x.UserId)
                 .Select(x => x).Count();
            descriptionDto.PlayerOfContest = numberOfPlayersOfContest;
            descriptionDto.Title = contestAward.WinningTitle;
            descriptionDto.Price = contestAward.WinningPrice;
            descriptionDto.Token = contestAward.WinningToken;

            var contestAwardList = new List<AwardLiteDto>();

            for (int i = 0; i < contestAward.ContestWinner.ToList().Count(); i++)
            {
                var contestAwardObject = new AwardLiteDto();
                //contestAwardObject.AwardName = contestAward.ContestWinner
                //.Where(x => x.ContestId == contestAward.Id)
                //.Select(y => y.Award.AwardType.Name)
                //.ToList()[i];
                contestAwardObject.ContestId = contestAward.Id;
                contestAwardObject.RankFrom = contestAward.ContestWinner
                     .Where(x => x.ContestId == contestAward.Id).Select(y => y.RankFrom).ToList()[i];
                contestAwardObject.RankTo = contestAward.ContestWinner
                     .Where(x => x.ContestId == contestAward.Id).Select(y => y.RankTo).ToList()[i];
                contestAwardObject.Price = GetWinningPrice(contestAward.ContestWinner
                     .Where(x => x.ContestId == contestAward.Id)
                     .Select(y => y.PricePercentage)
                     .ToList()[i], contestAward.WinningPrice);
                contestAwardObject.Token = GetWinningToken(contestAward.ContestWinner
                     .Where(x => x.ContestId == contestAward.Id)
                     .Select(y => y.TokenPercentage)
                     .ToList()[i], contestAward.WinningToken);
                contestAwardObject.Merchendise = GetMercendiseAward(contestAward.ContestWinner
                     .Where(x => x.ContestId == contestAward.Id)
                     .Select(y => y.Marchendise)
                     .ToList()[i]);
                contestAwardObject.AwardName = GetMercendiseAward(contestAward.ContestWinner
                     .Where(x => x.ContestId == contestAward.Id)
                     .Select(y => y.OtherReward)
                     .ToList()[i]);
                contestAwardList.Add(contestAwardObject);
            }
            return await Task.FromResult(new Tuple<List<AwardLiteDto>, AwardLiteDescriptionDto>(contestAwardList, descriptionDto));
        }

        public async Task<ContestLiteDto> GetContestById(int contestId)
        {
            var contestFees = _repoContest.Query()
            .Filter(x => x.Id == contestId)
            .Includes(c => c.Include(cc => cc.ContestCategory))
            .Get()
            .SingleOrDefault();

            return await Task.FromResult(ContestMapper.Map<ContestLiteDto>(contestFees));
        }

        public async Task<List<long>> GetJoinedContestListByEventId(int eventId, string UserId)
        {
            var data = (from jc in _repoJoinedContest.Query().Get()
                        join c in _repoContest.Query().Get() on jc.ContestId equals c.Id
                        where jc.UserId == UserId && c.EventId == eventId
                        select new
                        {
                            c
                        }).ToList();

            return await Task.FromResult(data.Select(x => x.c.Id).ToList());
        }
        public async Task<Tuple<List<JoinUserContestLiteDto>, int>> JoinedUserContestAjax(long contestId, int eventId, int start = 0, int length = 10)
        {
            List<JoinUserContestLiteDto> joinUserContests = new List<JoinUserContestLiteDto>();
            int totalRecords = 0;
            
            try
            {
                var evnt = _repoEvent.Query().Filter(d => d.Id == eventId).Get().FirstOrDefault();
                bool CanEditTeam = (DateTime.Now.AddHours(1) <= evnt.PerfTime);
                var result = (from jc in _repoJoinedContest.Query()
                              .Includes(xx => xx.Include(x => x.Team))
                              .Get().Where(r => r.IsDelete != true)
                              join u in _repoUsers.Query().Get() on jc.UserId equals u.UserId
                              join uw in _repoContestUserWinner.Query().Get() on jc.TeamId equals uw.TeamId into ps
                              from uw in ps.DefaultIfEmpty()
                              where jc.ContestId == contestId
                              select new
                              {
                                  jc.Team.TeamPoint,
                                  jc.ContestId,
                                  jc.UserId,
                                  jc.TeamId,
                                  u.UserName,
                                  u.TeamName,
                                  u.Email,
                                  uw,
                                  u.Avtar
                              }).Distinct().ToList();
                
                //Calculate rank for joined team for contest.
                var rankResult = (from a in result.OrderByDescending(x => x.TeamPoint)
                                  select new
                                  {
                                      Rank = result.Count(x => x.TeamPoint > a.TeamPoint) + 1,
                                      a.ContestId,
                                      a.UserId,
                                      a.TeamId,
                                      a.TeamPoint,
                                      a.UserName,
                                      a.TeamName,
                                      a.Email,
                                      ContestWinnerId = a.uw != null ? a.uw.ContestWinnerId : 0,
                                      a.Avtar
                                  }).ToList();

                totalRecords = rankResult.Count;
                var tmpResult = rankResult.Skip(start).Take(length); 
                //get Number of contest 
                var allJonedContests = _repoJoinedContest.Query().Get().ToList();

                foreach (var item in tmpResult)
                {
                    var modelData = GetWinnings(item.ContestId, item.ContestWinnerId);
                    var model = modelData.Item1;
                    JoinUserContestLiteDto dto = new JoinUserContestLiteDto();
                    dto.Email = item.Email;
                    dto.Merchendise = model != null ? !string.IsNullOrEmpty(model.Merchandise) ? model.Merchandise : "" : "";
                    dto.OtherAward = model != null ? !string.IsNullOrEmpty(model.OtherReward) ? model.OtherReward : "" : "";
                    dto.Price = Convert.ToDecimal(model != null ? !string.IsNullOrEmpty(model.Price) ? model.Price.Replace("$", "") : "0" : "0");
                    dto.Token = Convert.ToDecimal(model != null ? !string.IsNullOrEmpty(model.Token) ? model.Token : "0" : "0");
                    dto.UserName = item.UserName;
                    dto.TeamName = item.TeamName;
                    dto.TeamPoint = item.TeamPoint;
                    dto.TeamRank = item.Rank;
                    dto.UserId = item.UserId;
                    dto.ContestId = item.ContestId;
                    dto.TeamId = item.TeamId;
                    dto.Avatar = !string.IsNullOrEmpty(item.Avtar) ? (item.Avtar.Contains("https://") ? item.Avtar : (item.Avtar != "/images/RR/user-n.png" ? (_appSettings.MainSiteURL + "/images/profilePicture/" + item.Avtar) : _appSettings.MainSiteURL + "/images/home/team-icon.png")) : _appSettings.MainSiteURL + "/images/home/team-icon.png";
                    dto.CanUpdateTeam = false; //CanEditTeam;
                    dto.NumberOfContest = allJonedContests.Where(x => x.UserId == item.UserId).Count();
                    joinUserContests.Add(dto);
                }

                
            }
            catch (Exception ex)
            {
            }
            return await Task.FromResult(new Tuple<List<JoinUserContestLiteDto>, int>(joinUserContests.ToList(), totalRecords ));
        }
        public async Task<Tuple<List<JoinUserContestLiteDto>, int, string>> JoinedUserContest(long contestId, int eventId, int start = 0, int length = 10, int column = 0, string searchStr = "", string sort = "")
        {
            List<JoinUserContestLiteDto> joinUserContests = new List<JoinUserContestLiteDto>();
            int Members = 0;
            string ContestName = "";
            try
            {
                var evnt = _repoEvent.Query().Filter(d => d.Id == eventId).Get().FirstOrDefault();
                bool CanEditTeam = (DateTime.Now.AddHours(1) <= evnt.PerfTime);
                var result = (from jc in _repoJoinedContest.Query()
                              .Includes(xx => xx.Include(x => x.Team))
                              .Get().Where(r=>r.IsDelete != true)
                              join u in _repoUsers.Query().Get() on jc.UserId equals u.UserId
                              join uw in _repoContestUserWinner.Query().Get() on jc.TeamId equals uw.TeamId into ps
                              from uw in ps.DefaultIfEmpty()
                              where jc.ContestId == contestId
                              select new
                              {
                                  jc.Team.TeamPoint,
                                  jc.ContestId,
                                  jc.UserId,
                                  jc.TeamId,
                                  u.UserName,
                                  u.TeamName,
                                  u.Email,
                                  uw,
                                  u.Avtar
                              }).Distinct().ToList();

                //Calculate rank for joined team for contest.
                var rankResult = (from a in result.OrderByDescending(x => x.TeamPoint)
                                  select new
                                  {
                                      Rank = result.Count(x => x.TeamPoint > a.TeamPoint) + 1,
                                      a.ContestId,
                                      a.UserId,
                                      a.TeamId,
                                      a.TeamPoint,
                                      a.UserName,
                                      a.TeamName,
                                      a.Email,
                                      ContestWinnerId = a.uw != null ? a.uw.ContestWinnerId : 0,
                                      a.Avtar
                                  }).Distinct().ToList();

                //get Number of contest 
                var allJonedContests = _repoJoinedContest.Query().Get().ToList();

                foreach (var item in rankResult)
                {
                    var modelData = GetWinnings(item.ContestId, item.ContestWinnerId);
                    Members = modelData.Item2;
                    ContestName = modelData.Item3;
                    var model = modelData.Item1;
                    JoinUserContestLiteDto dto = new JoinUserContestLiteDto();
                    dto.Email = item.Email;
                    dto.Merchendise = model != null ? !string.IsNullOrEmpty(model.Merchandise) ? model.Merchandise : "" : "";
                    dto.OtherAward = model != null ? !string.IsNullOrEmpty(model.OtherReward) ? model.OtherReward : "" : "";
                    dto.Price = Convert.ToDecimal(model != null ? !string.IsNullOrEmpty(model.Price) ? model.Price.Replace("$","") : "0" : "0");
                    dto.Token = Convert.ToDecimal(model != null ? !string.IsNullOrEmpty(model.Token) ? model.Token : "0" : "0");
                    dto.UserName = item.UserName;
                    dto.TeamName = item.TeamName;
                    dto.TeamPoint = item.TeamPoint;
                    dto.TeamRank = item.Rank;
                    dto.UserId = item.UserId;
                    dto.ContestId = item.ContestId;
                    dto.TeamId = item.TeamId;
                    dto.Avatar = !string.IsNullOrEmpty(item.Avtar) ? (item.Avtar.Contains("https://") ? item.Avtar : (item.Avtar != "/images/RR/user-n.png" ? (_appSettings.MainSiteURL + "/images/profilePicture/" + item.Avtar) : _appSettings.MainSiteURL + "/images/home/team-icon.png")) : _appSettings.MainSiteURL + "/images/home/team-icon.png";
                    dto.CanUpdateTeam = false; //CanEditTeam;
                    dto.NumberOfContest = allJonedContests.Where(x => x.UserId == item.UserId).Count();
                    joinUserContests.Add(dto);
                }

                switch (column)
                {
                    case 0:
                        joinUserContests = (sort == "desc" ? joinUserContests.OrderByDescending(xx => xx.UserName) : joinUserContests.OrderBy(xx => xx.UserName)).Distinct().ToList();
                        break;
                    case 1:
                        joinUserContests = (sort == "desc" ? joinUserContests.OrderByDescending(xx => xx.TeamPoint) : joinUserContests.OrderBy(xx => xx.TeamPoint)).Distinct().ToList();
                        break;
                    case 2:
                        joinUserContests = (sort == "desc" ? joinUserContests.OrderByDescending(xx => xx.TeamRank) : joinUserContests.OrderBy(xx => xx.TeamRank)).Distinct().ToList();
                        break;
                    case 3:
                        joinUserContests = (sort == "desc" ? joinUserContests.OrderByDescending(xx => xx.Token)
                                                             .OrderByDescending(xx => xx.Merchendise)
                                                             .OrderByDescending(xx => xx.OtherAward)
                                                             .OrderByDescending(xx => xx.Price) : joinUserContests.OrderBy(xx => xx.Token)
                                                             .OrderBy(xx => xx.Price).OrderBy(xx => xx.Merchendise)
                                                             .OrderBy(xx => xx.OtherAward))
                                                             .Distinct()
                                                             .ToList();
                        break;
                    default:
                        joinUserContests = (sort == "desc" ? joinUserContests.OrderByDescending(xx => xx.UserName) : joinUserContests.OrderBy(xx => xx.UserName)).Distinct().ToList();
                        break;
                }
            }
            catch (Exception ex)
            {
            }
            return await Task.FromResult(new Tuple<List<JoinUserContestLiteDto>, int, string>(joinUserContests.Where(x => x.UserName.ToLower().Contains(searchStr.ToLower())).ToList(), Members, ContestName));
        }
        
        public async Task<ContestLiteDto> GetContestByUniqueCode(string uniqueCode)
        {
            var contest = _repoContest.Query()
                   .Filter(x => x.UniqueCode == uniqueCode)
                   .Includes(x => x.Include(cc => cc.ContestCategory))
                   .Get()
                   .FirstOrDefault();
            if (contest != null)
            {
                var joinedMembers = _repoJoinedContest.Query().Filter(x => x.ContestId == contest.Id).Get().ToList().Count;

                var response = ContestMapper.MapContestLiteDto(contest);

                if (joinedMembers == contest.Members)
                {
                    response.Message = "full";
                }

                return await Task.FromResult(response);
            }
            else
            {
                return null;
            }
        }

        public string GetWinningPrice(int percentage, decimal TotalPrice)
        {
            string price = "";
            try
            {
                if (percentage > 0)
                {
                    var Amount = (TotalPrice * percentage) / 100;
                    price = "$" + Amount;
                }
            }
            catch (Exception ex)
            {
            }
            return price;
        }

        public string GetWinningToken(int percentage, decimal TotalToken)
        {
            string price = "";
            try
            {
                if (percentage > 0)
                {
                    var Token = (TotalToken * percentage) / 100;
                    price = "" + Token;
                }
            }
            catch (Exception ex)
            {
            }
            return price;
        }

        public string GetMercendiseAward(long? Id)
        {
            string award = "";

            try
            {
                if (Id.HasValue)
                {
                    var awardData = _repoAward.Query().Filter(x => x.Id == Id.Value && x.IsDelete == false).Get().SingleOrDefault();
                    if (awardData != null)
                    {
                        award = awardData.Message;
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return award;
        }

        public Tuple<ContestWinnerLiteDto, int, string> GetWinnings(long ContestId, long ContestWinningId)
        {
            ContestWinnerLiteDto model = new ContestWinnerLiteDto();
            int members = 0;
            string ContestName = "";
            var contest = _repoContest
                .Query()
                .Filter(x => x.Id == ContestId)
                .Includes(xx => xx.Include(x => x.ContestWinner))
                .Get().FirstOrDefault();
            members = contest != null ? contest.Winners : 0;
            ContestName = contest != null ? contest.WinningTitle : "";

            var contestWinner = contest.ContestWinner.Where(x => x.Id == ContestWinningId).FirstOrDefault();

            if (contestWinner != null)
            {
                model.Merchandise = GetMercendiseAward(contestWinner.Marchendise);
                model.OtherReward = GetMercendiseAward(contestWinner.OtherReward);
                model.Price = GetWinningPrice(contestWinner.PricePercentage, contest.WinningPrice);
                model.Token = GetWinningToken(contestWinner.TokenPercentage, contest.WinningToken);
            }

            return new Tuple<ContestWinnerLiteDto, int, string>(model, members, ContestName);
        }

        /// <summary>
        /// Dispose All Services of contest
        /// </summary>
        public void Dispose()
        {
            if (_repoContest != null)
            {
                _repoContest.Dispose();
            }
            if (_repoAward != null)
            {
                _repoAward.Dispose();
            }
            if (_repoAwardType != null)
            {
                _repoAwardType.Dispose();
            }
            if (_repoContestCategory != null)
            {
                _repoContestCategory.Dispose();
            }
            if (_repoContestWinner != null)
            {
                _repoContestWinner.Dispose();
            }
            if (_repoJoinedContest != null)
            {
                _repoJoinedContest.Dispose();
            }
            if (_repoUsers != null)
            {
                _repoUsers.Dispose();
            }
            if (_repoTeam != null)
            {
                _repoTeam.Dispose();
            }
            if (_repoEvent != null)
            {
                _repoEvent.Dispose();
            }
            if (_repoTeamRider != null)
            {
                _repoTeamRider.Dispose();
            }
            if (_repoTeamBull != null)
            {
                _repoTeamBull.Dispose();
            }
            if (_repoRider != null)
            {
                _repoRider.Dispose();
            }
            if (_repoBull != null)
            {
                _repoBull.Dispose();
            }
            if (_repoEventBull != null)
            {
                _repoEventBull.Dispose();
            }
            if (_repoEventRider != null)
            {
                _repoEventRider.Dispose();
                _repoEventRider.Dispose();
            }
        }
    }
}
