using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RR.AdminData;
using RR.Core;
using RR.Data;
using RR.Dto;
using RR.Dto.Event;
using RR.Dto.Team;
using RR.Mapper;
using RR.Repo;
using RR.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.Service
{
    public class TeamService : ITeamService
    {
        #region Constructor

        private readonly IRepository<EventDraw, RankRideStaticContext> _repoeventDraw;
        private readonly IRepository<Team, RankRideContext> _repoTeam;
        private readonly IRepository<JoinedContest, RankRideContext> _repoJoinContest;
        private readonly IRepository<Event, RankRideStaticContext> _repoEvent;
        private readonly IRepository<TeamBull, RankRideContext> _repoTeamBull;
        private readonly IRepository<TeamRider, RankRideContext> _repoTeamRider;
        private readonly IRepository<EventBull, RankRideStaticContext> _repoEventBull;
        private readonly IRepository<EventRider, RankRideStaticContext> _repoEventRider;
        private readonly IRepository<UserDetail, RankRideContext> _repoUserDetail;
        private readonly IRepository<Contest, RankRideAdminContext> _repoContest;
        private readonly IRepository<Bull, RankRideStaticContext> _repoBull;
        private readonly IRepository<Rider, RankRideStaticContext> _repoRider;
        private AppSettings _appSettings;
        private readonly IRiderService _riderService;
        private readonly IBullService _bullService;

        public TeamService(IRepository<EventDraw, RankRideStaticContext> repoeventDraw,
             IRepository<Team, RankRideContext> repoTeam,
             IRepository<Event, RankRideStaticContext> repoEvent,
             IRepository<JoinedContest, RankRideContext> repoJoinContest,
             IRepository<TeamBull, RankRideContext> repoTeamBull,
             IRepository<TeamRider, RankRideContext> repoTeamRider,
             IRepository<EventBull, RankRideStaticContext> repoEventBull,
             IRepository<EventRider, RankRideStaticContext> repoEventRider,
             IRepository<UserDetail, RankRideContext> repoUserDetail,
             IRepository<Contest, RankRideAdminContext> repoContest,
             IRepository<Bull, RankRideStaticContext> repoBull,
             IRepository<Rider, RankRideStaticContext> repoRider,
             IRiderService riderService,
             IBullService bullService,
             IOptions<AppSettings> appSettings)
        {
            _repoeventDraw = repoeventDraw;
            _repoTeam = repoTeam;
            _repoEvent = repoEvent;
            _repoJoinContest = repoJoinContest;
            _repoTeamBull = repoTeamBull;
            _repoTeamRider = repoTeamRider;
            _repoEventBull = repoEventBull;
            _repoEventRider = repoEventRider;
            _repoUserDetail = repoUserDetail;
            _repoContest = repoContest;
            _repoBull = repoBull;
            _repoRider = repoRider;
            _appSettings = appSettings.Value;
            _riderService = riderService;
            _bullService = bullService;
        }

        #endregion

        public async Task<TeamFormationDetailDto> EventPlayerDataById(int eventId, int contestId, int teamId)
        {
            TeamFormationDetailDto formDto = new TeamFormationDetailDto();
            formDto.EventId = eventId;
            formDto.ContestId = contestId;
            formDto.IsFinished = true;
            formDto.IsEditedTeam = true;
            var eventDetail = _repoEvent.Query().Filter(x => x.Id == eventId).Get().FirstOrDefault();
            if (eventDetail != null)
            {
                var tmpPerfTime = eventDetail.PerfTimeUTC ?? eventDetail.PerfTime;

                if (tmpPerfTime > DateTime.UtcNow)
                    formDto.IsFinished = false;
                if (tmpPerfTime >= DateTime.UtcNow.AddHours(-1))
                    formDto.IsEditedTeam = false;
            }
            formDto.DrawList = new List<TeamDrawDto>();
            formDto.BullList = new List<TeamBullDrawDto>();
            formDto.EventResultList = new List<EventResultDto>();

            var temp = _repoeventDraw.Query().Filter(x => x.EventId == eventId).Get();
            var riderIds = temp.Select(x => x.RiderId).ToList();
            var bullIds = _repoEventBull.Query().Filter(x => x.EventId == eventId).Get().Select(y => y.BullId).ToList();

            var temp1 = _repoEvent.Query().Filter(d => d.StartDate.Year == DateTime.Now.Year && !string.IsNullOrEmpty(d.EventResult)).Get().SelectMany(m => JsonConvert.DeserializeObject<IEnumerable<EventResultDto>>(m.EventResult));

            var evenResults = temp1.Where(x => riderIds.Any(id => id == x.rider_id)).GroupBy(r => r.rider_id).Select(r => new { key = r.Key, point = r.Sum(g => g.rr_rider_score) });
            var evenResultsBull = temp1.GroupBy(r => r.pbid).Select(r => new { key = r.Key, point = r.Sum(g => g.rr_bull_score) });

            MyRedisConnectorHelper redisHelper = new MyRedisConnectorHelper(_appSettings);

            if (formDto.IsFinished)
            {
                if (!redisHelper.ExistRedisEventResultList(eventId) || (redisHelper.ExistRedisEventResultList(eventId) && redisHelper.ReadRedisEventResultList(eventId).Count == 0))
                {
                    var bulls = _repoBull.Query().Get();
                    var tempResult = _repoEvent.Query().Filter(d => d.Id == eventId && !string.IsNullOrEmpty(d.EventResult)).Get().SelectMany(m => JsonConvert.DeserializeObject<IEnumerable<EventResultDto>>(m.EventResult)).OrderByDescending(x => x.ride_score).ToList();
                    foreach (var rrTemp in tempResult)
                    {
                        rrTemp.bull_name = bulls.Where(y => y.BullId == rrTemp.pbid).Select(x => x.Name).FirstOrDefault();
                        rrTemp.bull_avatar = _bullService.GetBullPic(rrTemp.pbid, _appSettings.MainSiteURL).Result;
                        rrTemp.rider_avatar = _riderService.GetRiderPic(rrTemp.rider_id, _appSettings.MainSiteURL).Result;
                    }
                    formDto.EventResultList = tempResult;
                    redisHelper.SaveRedisEventResultList(tempResult, eventId);
                }
                else
                {
                    formDto.EventResultList = redisHelper.ReadRedisEventResultList(eventId);
                }
            }
            else
            {
                List<TeamDrawDto> drawEvent = new List<TeamDrawDto>();
                if (!redisHelper.ExistRedisEventDrawList(eventId) || (redisHelper.ExistRedisEventDrawList(eventId) && redisHelper.ReadRedisEventDrawList(eventId).Count == 0))
                {
                    drawEvent = (from ed in _repoeventDraw.Query().Filter(x => x.EventId == eventId).Get()
                                join r in _repoRider.Query().Get() on ed.RiderId equals r.RiderId
                                join er in _repoEventRider.Query().Filter(x => x.EventId == eventId).Get() on r.Id equals er.RiderId
                                join b in _repoBull.Query().Get() on ed.BullId equals b.BullId
                                join eb in _repoEventBull.Query().Filter(x => x.EventId == eventId).Get() on b.Id equals eb.BullId
                                where ed.EventId == eventId
                                select new TeamDrawDto
                                {
                                    Round = ed.Round,
                                    QRProb = ed.QRProb ?? 0,
                                    EventId = ed.EventId,
                                    RiderId = r.Id,
                                    RiderName = r.Name,
                                    RiderTier = er.EventTier,
                                    RiderIsDropout = er.IsDropout ?? false,
                                    WorldRanking = r.Cwrp ?? 9999,
                                    RiderPower = r.RiderPower,
                                    RRTotalpoint = evenResults.FirstOrDefault(k => k.key == r.RiderId)?.point ?? 0,
                                    RiderAvatar = _riderService.GetRiderPic(r.RiderId, _appSettings.MainSiteURL).Result,
                                    RiderIsSelected = false,

                                    BullId = b.Id,
                                    BullName = ed.BullName,
                                    BullTier = eb.EventTier,
                                    BullIsDropout = eb.IsDropout ?? false,
                                    BullOwner = b.Owner,
                                    BullAverageMark = b.AverageMark,
                                    BullPowerRating = b.PowerRating,
                                    BullRankRideScore = evenResultsBull.FirstOrDefault(r => r.key == b.BullId)?.point ?? 0,
                                    BullAvatar = _bullService.GetBullPic(b.BullId, _appSettings.MainSiteURL).Result,
                                    BullIsSelected = false,
                                }).Distinct().ToList();
                    redisHelper.SaveRedisEventDrawList(drawEvent, eventId);
                }
                else
                {
                    drawEvent = redisHelper.ReadRedisEventDrawList(eventId);
                }
                List<TeamBullDrawDto> bulls = new List<TeamBullDrawDto>();
                if (!redisHelper.ExistRedisExtraBullsList(eventId) || (redisHelper.ExistRedisExtraBullsList(eventId) && redisHelper.ReadRedisExtraBullsList(eventId).Count == 0))
                {
                    var drawBullIds = drawEvent.Select(y => y.BullId).ToList();
                    //get remaining bulls
                    bulls = _repoEventBull.Query()
                      .Filter(x => x.EventId == eventId && x.BullId > 0 && !drawBullIds.Any(id => id == x.BullId))
                      .Includes(x => x.Include(z => z.Bull))
                       .Get().Select(y => new TeamBullDrawDto
                       {
                           BullId = y.BullId,
                           BullName = y.Bull.Name,
                           BullTier = y.EventTier,
                           BullIsDropout = y.IsDropout ?? false,
                           BullOwner = y.Bull.Owner,
                           BullAverageMark = y.Bull.AverageMark,
                           BullPowerRating = y.Bull.PowerRating,
                           BullIsSelected = false,
                           BullAvatar = _bullService.GetBullPic(y.BullId, _appSettings.MainSiteURL).Result,
                           BullRankRideScore = evenResultsBull.FirstOrDefault(r => r.key == y.BullId)?.point ?? 0,
                       }).ToList();
                    redisHelper.SaveRedisExtraBullsList(bulls, eventId);
                }
                else
                {
                    bulls = redisHelper.ReadRedisExtraBullsList(eventId);
                }
                if (!formDto.IsEditedTeam && teamId > 0)
                {
                    var teamBull = _repoTeamBull.Query().Filter(x => x.TeamId == teamId).Get();
                    var teamRider = _repoTeamRider.Query().Filter(x => x.TeamId == teamId).Get();

                    foreach (var tmp in teamRider)
                    {
                        var drawItem = drawEvent.Where(x => x.RiderId == tmp.RiderId).FirstOrDefault();
                        if (drawItem != null)
                        {
                            drawItem.RiderIsSelected = true;
                        }
                    }
                    
                    foreach (var tmp in teamBull)
                    {
                        var drawItem = drawEvent.Where(x => x.BullId == tmp.BullId).FirstOrDefault();
                        if (drawItem != null)
                        {
                            drawItem.BullIsSelected = true;
                        }
                        var item = bulls.Where(x => x.BullId == tmp.BullId).FirstOrDefault();
                        if (item != null)
                        {
                            item.BullIsSelected = true;
                        }
                    }
                }
                formDto.DrawList = drawEvent;
                formDto.BullList = bulls;
            }
            
            return await Task.FromResult(formDto);
        }
        public async Task<TeamFormationDto> EventPlayersById(int eventId, int contestId, int teamId)
        {
            TeamFormationDto formationDto = new TeamFormationDto();
            formationDto.BullArray = new DataResult<List<TeamBullFormationDto>, List<TeamBullFormationDto>, List<TeamBullFormationDto>>();
            formationDto.RiderArray = new DataResult<List<TeamRiderFormationDto>, List<TeamRiderFormationDto>, List<TeamRiderFormationDto>>();
            formationDto.EventId = eventId;
            formationDto.ContestId = contestId;
            formationDto.IsFinished = true;
            formationDto.IsEditedTeam = true;
            var eventDetail = _repoEvent.Query().Filter(x => x.Id == eventId).Get().FirstOrDefault();
            if (eventDetail != null)
            {
                if (eventDetail.PerfTime > DateTime.Now)
                    formationDto.IsFinished = false;
                if (eventDetail.PerfTime >= eventDetail.PerfTime.AddHours(-1))
                    formationDto.IsEditedTeam = false;
            }

            if (!formationDto.IsEditedTeam && teamId > 0)
            {
                formationDto.BullList = new List<TeamBullFormationDto>();
                formationDto.RiderList = new List<TeamRiderFormationDto>();

                var bulls = _repoEventBull.Query()
                  .Filter(x => x.EventId == eventId && x.BullId > 0)
                  .Includes(x => x.Include(y => y.Bull))
                   .Get();
                var riders = _repoEventRider.Query()
                     .Filter(x => x.EventId == eventId && x.RiderId > 0)
                     .Includes(x => x.Include(y => y.Rider))
                      .Get();

                var teamBull = _repoTeamBull.Query().Get();
                var teamRider = _repoTeamRider.Query().Get();
                foreach (var item in bulls)
                {
                    var bullList = new TeamBullFormationDto();
                    if (teamBull.Where(x => x.BullId == item.BullId && item.EventId == eventId && x.TeamId == teamId).FirstOrDefault() != null)
                    {
                        bullList.TeamId = teamBull.FirstOrDefault(x => x.BullId == item.BullId && item.EventId == eventId && x.TeamId == teamId)?.TeamId;
                        bullList.IsSelected = true;
                        bullList.BullId = item.BullId;
                        bullList.BullName = item.Bull.Name;
                        bullList.BullTier = item.EventTier;
                        bullList.IsDropout = item.IsDropout ?? false;
                        formationDto.BullList.Add(bullList);
                    }
                    else
                    {
                        bullList.TeamId = teamBull.FirstOrDefault(x => x.BullId == item.BullId && item.EventId == eventId && x.TeamId == teamId)?.TeamId;
                        bullList.IsSelected = false;
                        bullList.BullId = item.BullId;
                        bullList.BullName = item.Bull.Name;
                        bullList.BullTier = item.EventTier;
                        bullList.IsDropout = item.IsDropout ?? false;
                        formationDto.BullList.Add(bullList);
                    }
                }

                foreach (var item in riders)
                {
                    var riderList = new TeamRiderFormationDto();
                    if (teamRider.Where(x => x.RiderId == item.RiderId && item.EventId == eventId && x.TeamId == teamId).FirstOrDefault() != null)
                    {
                        riderList.TeamId = teamRider.FirstOrDefault(x => x.RiderId == item.RiderId && item.EventId == eventId && x.TeamId == teamId)?.TeamId;
                        riderList.IsSelected = true;
                        riderList.RiderId = item.RiderId;
                        riderList.RiderName = item.Rider.Name;
                        riderList.RiderTier = item.EventTier;
                        riderList.IsDropout = item.IsDropout ?? false;
                        formationDto.RiderList.Add(riderList);
                    }
                    else
                    {
                        riderList.TeamId = teamRider.FirstOrDefault(x => x.RiderId == item.RiderId && item.EventId == eventId && x.TeamId == teamId)?.TeamId;
                        riderList.IsSelected = false;
                        riderList.RiderId = item.RiderId;
                        riderList.RiderName = item.Rider.Name;
                        riderList.RiderTier = item.EventTier;
                        riderList.IsDropout = item.IsDropout ?? false;
                        formationDto.RiderList.Add(riderList);
                    }
                }
            }
            else if (!formationDto.IsFinished)
            {
                var bulls = _repoEventBull.Query()
                  .Filter(x => x.EventId == eventId && x.BullId > 0)
                  .Includes(x => x.Include(y => y.Bull))
                   .Get();
                var riders = _repoEventRider.Query()
                     .Filter(x => x.EventId == eventId && x.RiderId > 0)
                     .Includes(x => x.Include(y => y.Rider))
                      .Get();
                formationDto.BullList = TeamMapper.Map(bulls).ToList();
                formationDto.RiderList = TeamMapper.Map(riders).ToList();
            }
            else
            {
                formationDto.BullList = new List<TeamBullFormationDto>();
                formationDto.RiderList = new List<TeamRiderFormationDto>();
            }
            return await Task.FromResult(formationDto);
        }

        public async Task<TeamFormationDto> EventPlayersByIdApi(int eventId, int contestId, int teamId)
        {
            TeamFormationDto formationDto = new TeamFormationDto();
            formationDto.BullArray = new DataResult<List<TeamBullFormationDto>, List<TeamBullFormationDto>, List<TeamBullFormationDto>>();
            formationDto.RiderArray = new DataResult<List<TeamRiderFormationDto>, List<TeamRiderFormationDto>, List<TeamRiderFormationDto>>();
            formationDto.BullArray.Result1 = new List<TeamBullFormationDto>();
            formationDto.BullArray.Result2 = new List<TeamBullFormationDto>();
            formationDto.BullArray.Result3 = new List<TeamBullFormationDto>();
            formationDto.RiderArray.Result1 = new List<TeamRiderFormationDto>();
            formationDto.RiderArray.Result2 = new List<TeamRiderFormationDto>();
            formationDto.RiderArray.Result3 = new List<TeamRiderFormationDto>();
            formationDto.EventId = eventId;
            formationDto.ContestId = contestId;
            formationDto.IsFinished = true;
            formationDto.IsEditedTeam = true;
            var eventDetail = _repoEvent.Query().Filter(x => x.Id == eventId).Get().FirstOrDefault();
            if (eventDetail != null)
            {
                if (eventDetail.PerfTime > DateTime.Now)
                    formationDto.IsFinished = false;
                if (eventDetail.PerfTime >= eventDetail.PerfTime.AddHours(-1))
                    formationDto.IsEditedTeam = false;
            }

            if (!formationDto.IsEditedTeam && teamId > 0)
            {
                formationDto.BullList = new List<TeamBullFormationDto>();
                formationDto.RiderList = new List<TeamRiderFormationDto>();

                var bulls = _repoEventBull.Query()
                  .Filter(x => x.EventId == eventId && x.BullId > 0)
                  .Includes(x => x.Include(y => y.Bull))
                   .Get();
                var riders = _repoEventRider.Query()
                     .Filter(x => x.EventId == eventId && x.RiderId > 0)
                     .Includes(x => x.Include(y => y.Rider))
                      .Get();

                var teamBull = _repoTeamBull.Query().Get();
                var teamRider = _repoTeamRider.Query().Get();

                foreach (var item in bulls)
                {
                    var bullList = new TeamBullFormationDto();
                    if (teamBull.Where(x => x.BullId == item.BullId && item.EventId == eventId && x.TeamId == teamId).FirstOrDefault() != null)
                    {
                        bullList.IsSelected = true;
                        bullList.BullId = item.BullId;
                        bullList.BullName = item.Bull.Name;
                        bullList.BullTier = item.EventTier;
                        formationDto.BullList.Add(bullList);
                    }
                    else
                    {
                        bullList.IsSelected = false;
                        bullList.BullId = item.BullId;
                        bullList.BullName = item.Bull.Name;
                        bullList.BullTier = item.EventTier;
                        formationDto.BullList.Add(bullList);
                    }
                    if (bullList.BullTier == 1)
                    {
                        formationDto.BullArray.Result1.Add(bullList);
                    }
                    else if (item.EventTier == 2)
                    {
                        formationDto.BullArray.Result2.Add(bullList);
                    }
                    else
                    {
                        formationDto.BullArray.Result3.Add(bullList);
                    }
                }

                foreach (var item in riders)
                {
                    var riderList = new TeamRiderFormationDto();
                    if (teamRider.Where(x => x.RiderId == item.RiderId && item.EventId == eventId && x.TeamId == teamId).FirstOrDefault() != null)
                    {
                        riderList.IsSelected = true;
                        riderList.RiderId = item.RiderId;
                        riderList.RiderName = item.Rider.Name;
                        riderList.RiderTier = item.EventTier;
                        formationDto.RiderList.Add(riderList);
                    }
                    else
                    {
                        riderList.IsSelected = false;
                        riderList.RiderId = item.RiderId;
                        riderList.RiderName = item.Rider.Name;
                        riderList.RiderTier = item.EventTier;
                        formationDto.RiderList.Add(riderList);
                    }
                    if (riderList.RiderTier == 1)
                    {
                        formationDto.RiderArray.Result1.Add(riderList);
                    }
                    else if (item.EventTier == 2)
                    {
                        formationDto.RiderArray.Result2.Add(riderList);
                    }
                    else
                    {
                        formationDto.RiderArray.Result3.Add(riderList);
                    }
                }
            }
            else if (!formationDto.IsFinished)
            {
                var bulls = _repoEventBull.Query()
                  .Filter(x => x.EventId == eventId && x.BullId > 0)
                  .Includes(x => x.Include(y => y.Bull))
                   .Get();
                var riders = _repoEventRider.Query()
                     .Filter(x => x.EventId == eventId && x.RiderId > 0)
                     .Includes(x => x.Include(y => y.Rider))
                      .Get();
                formationDto.BullList = TeamMapper.Map(bulls).ToList();
                formationDto.RiderList = TeamMapper.Map(riders).ToList();

                foreach (var item in formationDto.BullList)
                {
                    if (item.BullTier == 1)
                    {
                        formationDto.BullArray.Result1.Add(item);
                    }
                    else if (item.BullTier == 2)
                    {
                        formationDto.BullArray.Result2.Add(item);
                    }
                    else
                    {
                        formationDto.BullArray.Result3.Add(item);
                    }
                }
                foreach (var item in formationDto.RiderList)
                {
                    if (item.RiderTier == 1)
                    {
                        formationDto.RiderArray.Result1.Add(item);
                    }
                    else if (item.RiderTier == 2)
                    {
                        formationDto.RiderArray.Result2.Add(item);
                    }
                    else
                    {
                        formationDto.RiderArray.Result3.Add(item);
                    }
                }
            }
            else
            {
                formationDto.BullList = new List<TeamBullFormationDto>();
                formationDto.RiderList = new List<TeamRiderFormationDto>();
            }
            return await Task.FromResult(formationDto);
        }

        public async Task<int> CreateTeam(IEnumerable<TeamDto> teamDto, int eventId, string userId)
        {

            var bigTeamContest = teamDto.Where(x => x.TeamNumber == 1 && x.EventId == eventId);
            var otherContest = teamDto.Where(x => x.TeamNumber == 2 && x.EventId == eventId);

            if (bigTeamContest != null && bigTeamContest.Count() > 0)
            {
                var teamModel = new Team();
                teamModel = new Team
                {
                    UserId = userId,
                    ContestType = (byte)Enums.ContestType.BigTeamContest,
                    CreatedDate = DateTime.Now,
                    EventId = eventId,
                    IsDelete = false,
                    TeamNumber = bigTeamContest.FirstOrDefault().TeamNumber,
                    CreatedBy = userId,
                    UpdatedBy = userId,
                    UpdatedDate = DateTime.Now
                };
                foreach (var team in bigTeamContest)
                {
                    if (team.BullId > 0)
                    {
                        teamModel.TeamBull.Add(new TeamBull
                        {
                            BullId = team.BullId,
                            IsSubstitute = team.IsSubstitute
                        });
                    }
                    else
                    {
                        teamModel.TeamRider.Add(new TeamRider
                        {
                            RiderId = team.RiderId,
                            IsSubstitute = team.IsSubstitute
                        });
                    }
                }
                await _repoTeam.InsertGraphAsync(teamModel);
                return teamModel.Id;
            }
            if (otherContest != null && otherContest.Count() > 0)
            {
                var teamModel = new Team();
                teamModel = new Team
                {
                    UserId = userId,
                    ContestType = (byte)Enums.ContestType.Contest,
                    CreatedDate = DateTime.Now,
                    EventId = eventId,
                    IsDelete = false,
                    TeamNumber = otherContest.FirstOrDefault().TeamNumber,
                    CreatedBy = userId,
                    UpdatedBy = userId,
                    UpdatedDate = DateTime.Now

                };
                foreach (var team in otherContest)
                {
                    if (team.BullId > 0)
                    {
                        teamModel.TeamBull.Add(new TeamBull
                        {
                            BullId = team.BullId,
                            IsSubstitute = team.IsSubstitute
                        });
                    }
                    else
                    {
                        teamModel.TeamRider.Add(new TeamRider
                        {
                            RiderId = team.RiderId,
                            IsSubstitute = team.IsSubstitute
                        });
                    }
                }
                await _repoTeam.InsertGraphAsync(teamModel);
                return teamModel.Id;
            }
            return 0;
        }

        public async Task<int> CreateTeamApi(TeamFormationDto teamFormationDto)
        {
            if (teamFormationDto.TeamNumber == 1)
            {
                var teamModel = new Team();
                teamModel = new Team
                {
                    UserId = teamFormationDto.UserId,
                    ContestType = (byte)Enums.ContestType.BigTeamContest,
                    CreatedDate = DateTime.Now,
                    EventId = teamFormationDto.EventId,
                    IsDelete = false,
                    TeamNumber = 1,
                    CreatedBy = teamFormationDto.UserId,
                    UpdatedBy = teamFormationDto.UserId,
                    UpdatedDate = DateTime.Now
                };
                teamModel.TeamBull.Add(new TeamBull
                {
                    BullId = teamFormationDto.BullTier1
                });
                teamModel.TeamBull.Add(new TeamBull
                {
                    BullId = teamFormationDto.BullTier2
                });
                teamModel.TeamBull.Add(new TeamBull
                {
                    BullId = teamFormationDto.BullTier3
                });
                foreach (var team in teamFormationDto.RiderTier1)
                {
                    teamModel.TeamRider.Add(new TeamRider
                    {
                        RiderId = team
                    });
                }
                foreach (var team in teamFormationDto.RiderTier2)
                {
                    teamModel.TeamRider.Add(new TeamRider
                    {
                        RiderId = team
                    });
                }
                foreach (var team in teamFormationDto.RiderTier3)
                {
                    teamModel.TeamRider.Add(new TeamRider
                    {
                        RiderId = team
                    });
                }
                await _repoTeam.InsertGraphAsync(teamModel);
                return teamModel.Id;
            }
            if (teamFormationDto.TeamNumber == 2)
            {
                var teamModel = new Team();
                teamModel = new Team
                {
                    UserId = teamFormationDto.UserId,
                    ContestType = (byte)Enums.ContestType.Contest,
                    CreatedDate = DateTime.Now,
                    EventId = teamFormationDto.EventId,
                    IsDelete = false,
                    TeamNumber = 2,
                    CreatedBy = teamFormationDto.UserId,
                    UpdatedBy = teamFormationDto.UserId,
                    UpdatedDate = DateTime.Now

                };
                foreach (var team in teamFormationDto.BullArray.Result1)
                {
                    teamModel.TeamBull.Add(new TeamBull
                    {
                        BullId = team.BullId
                    });
                }
                foreach (var team in teamFormationDto.BullArray.Result2)
                {
                    teamModel.TeamBull.Add(new TeamBull
                    {
                        BullId = team.BullId
                    });
                }
                foreach (var team in teamFormationDto.BullArray.Result3)
                {
                    teamModel.TeamBull.Add(new TeamBull
                    {
                        BullId = team.BullId
                    });
                }
                foreach (var team in teamFormationDto.RiderArray.Result1)
                {
                    teamModel.TeamRider.Add(new TeamRider
                    {
                        RiderId = team.RiderId
                    });
                }
                foreach (var team in teamFormationDto.RiderArray.Result2)
                {
                    teamModel.TeamRider.Add(new TeamRider
                    {
                        RiderId = team.RiderId
                    });
                }
                foreach (var team in teamFormationDto.RiderArray.Result3)
                {
                    teamModel.TeamRider.Add(new TeamRider
                    {
                        RiderId = team.RiderId
                    });
                }
                await _repoTeam.InsertGraphAsync(teamModel);
                return teamModel.Id;
            }
            return 0;
        }


        public async Task<Tuple<IEnumerable<TeamLiteDto>, int>> TeamList(int eventId,
                                                                         int contestId,
                                                                         string user,
                                                                         int start,
                                                                         int length,
                                                                         int column,
                                                                         string searchStr = "",
                                                                         string sort = "")
        {
            var eventDetail = _repoEvent.FindById(eventId);

            int count = 0;
            var predicate = PredicateBuilder.True<Team>()
           .And(x => x.IsDelete == false && x.EventId == eventId && (searchStr == ""
              || x.Id.ToString().Contains(searchStr.ToLower())
              || x.User.UserName.Contains(searchStr.ToLower()))
              && x.UserId == user);

            var teams = _repoTeam.Query()
                  .Filter(predicate)
                  .Includes(t => t.Include(tb => tb.TeamBull).Include(tr => tr.TeamRider));

            var joinContest = _repoJoinContest.Query();

            if (FilterSortingVariable.TEAM_ID == column)
            {
                teams = (sort == "desc" ? teams.OrderBy(x => x.OrderByDescending(xx => xx.Id)) :
                     teams.OrderBy(x => x.OrderBy(xx => xx.Id)));
            }
            if (FilterSortingVariable.TEAM_USERNAME == column)
            {
                teams = (sort == "desc" ? teams.OrderBy(x => x.OrderByDescending(xx => xx.User.UserName)) :
                     teams.OrderBy(x => x.OrderBy(xx => xx.User.UserName)));
            }

            return await Task.FromResult(new Tuple<IEnumerable<TeamLiteDto>, int>(teams
                 .GetPage(start, length, out count).Select(y => new TeamLiteDto
                 {
                     Id = y.Id,
                     TeamId = joinContest
                      .Filter(x => x.TeamId == y.Id && x.ContestId == contestId).Get().SingleOrDefault() != null ? 0 : y.Id,
                     EventName = eventDetail.Title,
                     UserEmail = user,
                     ContestType = Enum.GetName(typeof(Enums.ContestType), y.ContestType)
                 }), count));

        }

        public async Task AddJoinTeamContest(JoinedContestDto joinedContestDto)
        {
            var joinContest = _repoJoinContest.Query()
                 .Filter(x => x.ContestId == joinedContestDto.ContestId && x.TeamId == joinedContestDto.TeamId)
                 .Get()
                 .SingleOrDefault();

            if (joinContest == null)
            {
                var teamContest = new JoinedContest
                {
                    TeamId = joinedContestDto.TeamId,
                    ContestId = joinedContestDto.ContestId,
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    IsDelete = false
                };
                await _repoJoinContest.InsertGraphAsync(teamContest);
            }
        }

        public async Task<List<Team>> GetJoinedTeamsByEventId(int eventId)
        {
            var teamList = new List<Team>();
            teamList = (from team in _repoTeam.Query()
                        .Includes(t => t.Include(tb => tb.TeamBull).Include(tr => tr.TeamRider).Include(tr => tr.JoinedContest)).Get()
                        join jc in _repoJoinContest.Query().Get() on team.Id equals jc.TeamId
                        where team.EventId == eventId && team.IsDelete == false
                        select new Team()
                        {
                            EventId = team.EventId,
                            Id = team.Id,
                            TeamBull = team.TeamBull,
                            TeamRider = team.TeamRider,
                            UserId = team.UserId,
                            TeamPoint = team.TeamPoint,
                            JoinedContest = team.JoinedContest
                        }).ToList();

            return await Task.FromResult(teamList);
        }

        public async Task UpdateTeamBullPoint(int teamBullId, decimal point = 0)
        {
            var bullExists = _repoTeamBull.Query()
                .Filter(x => x.TeamBullId == teamBullId).Get().FirstOrDefault();
            if (bullExists != null)
            {
                bullExists.BullPoint = point;
                await _repoTeamBull.UpdateAsync(bullExists);
            }
        }

        public async Task UpdateTeamRiderPoint(int teamRiderId, decimal point = 0)
        {
            var riderExists = _repoTeamRider.Query()
                .Filter(x => x.TeamRiderId == teamRiderId).Get().FirstOrDefault();
            if (riderExists != null)
            {
                riderExists.RiderPoint = point;
                await _repoTeamRider.UpdateAsync(riderExists);
            }
        }

        public async Task UpdateTeamTotalPoint(int teamId, decimal point = 0)
        {
            var teamExists = _repoTeam.Query()
                .Filter(x => x.Id == teamId).Get().FirstOrDefault();
            if (teamExists != null)
            {
                teamExists.TeamPoint = point;
                await _repoTeam.UpdateAsync(teamExists);
            }
        }

        public async Task<List<JoinedContestDto>> GetJoinedTeamsByContestId(long contestId)
        {
            var predicate = PredicateBuilder.True<JoinedContest>()
           .And(x => (x.ContestId == contestId))
            .And(x => x.IsActive == true)
            .And(x => x.IsDelete == false);

            var joinedTeams = _repoJoinContest
                            .Query()
                            .Filter(predicate);
            return await Task.FromResult((joinedTeams
                    .Get()).Select(y => new JoinedContestDto
                    {
                        PaymentTxnId = y.PaymentTxnId,
                        UserId = y.UserId,
                        ContestId = y.ContestId,
                        TeamId = y.TeamId
                    }).ToList());
        }

        public async Task<IEnumerable<PlayStandingLiteDto>> GetPlayerPointsOfEvent(bool getAll = false)
        {
            #region Old COde

            // int eventId = 0;
            // var eventDetail = _repoEvent.Query()
            //.Filter(x => x.PerfTime < DateTime.Now && x.EventResult != null)
            //.OrderBy(x => x.OrderByDescending(xx => xx.PerfTime))
            //.Get()
            //.FirstOrDefault();

            // if (eventDetail != null)
            // {
            //     eventId = eventDetail.Id;
            // }

            // var teamList = _repoTeam.Query()
            //      .Includes(x => x.Include(jc => jc.JoinedContest)
            //      .Include(u => u.User)
            //      .ThenInclude(ud => ud.UserDetail))
            //      .Get();

            // var lst = teamList.OrderByDescending(x => x.TeamPoint).Select(s => new PlayStandingLiteDto()
            // {
            //     Rank = teamList.Count(x => x.TeamPoint > s.TeamPoint) + 1,
            //     TeamPoint = s.TeamPoint,
            //     UserName = s.User.UserDetail.FirstOrDefault(y => y.UserId == s.UserId)?.UserName
            // }).Take(5).ToList();

            // return await Task.FromResult(teamList.OrderByDescending(x => x.TeamPoint).Select(s => new PlayStandingLiteDto()
            // {
            //     Rank = teamList.Count(x => x.TeamPoint > s.TeamPoint) + 1,
            //     TeamPoint = s.TeamPoint,
            //     UserName = s.User.UserDetail.FirstOrDefault(y => y.UserId == s.UserId)?.UserName
            // }).Take(5));

            #endregion

            #region New Code for Year end standings
            IEnumerable<PlayStandingLiteDto> resultTemp;
            MyRedisConnectorHelper redisHelper = new MyRedisConnectorHelper(_appSettings);

            if (!redisHelper.ExistRedisPlayerPoints())
            {
                List<StandingDto> standingList = new List<StandingDto>();
                var standings = new List<JoinedContest>();
                var listStandings = new List<StandingDto>();
                bool isPlayer = false;
                var sdfsdf = _repoJoinContest.Query().Filter(r => r.IsDelete == false).Get().ToList();
                standings = _repoJoinContest.Query()
                        .Includes(y => y.Include(u => u.User)
                        .ThenInclude(ud => ud.UserDetail)
                        .Include(t => t.Team)).Filter(d => d.User.UserDetail.Count(r => r.IsActive == true) > 0)
                        .Filter(x => x.CreatedDate < DateTime.Now && x.CreatedDate.Year == DateTime.Now.Year && x.IsDelete == false)
                        .Get().ToList();


                foreach (var item in standings.Select(x => x.UserId).Distinct())
                {

                    var standingObj = new StandingDto();
                    standingObj.TeamPoint = standings.Where(x => x.UserId == item).Sum(x => x.Team.TeamPoint);
                    standingObj.UserName = GetUserName(standings.FirstOrDefault(x => x.UserId == item).User);
                    standingObj.NumberOfContest = standings.Where(x => x.UserId == item).Count();
                    standingObj.PlayerType = isPlayer ? standings.Where(x => x.UserId == item)
                                        .Select(x => x.User.AspNetUserRoles.FirstOrDefault().Role.NormalizedName)
                                        .FirstOrDefault() : string.Empty;
                    standingObj.Avtar = _repoUserDetail.Query().Filter(x => x.UserId == item).Get().FirstOrDefault().Avtar;
                    listStandings.Add(standingObj);

                }
                var data = listStandings.OrderByDescending(x => x.TeamPoint).Select(s => new StandingDto()
                {
                    TeamPoint = s.TeamPoint,
                    //TeamPoint = 0,
                    UserName = s.UserName,
                    NumberOfContest = s.NumberOfContest,
                    Rank = listStandings.Count(x => x.TeamPoint > s.TeamPoint) + 1,
                    //Rank = 0,
                    PlayerType = s.PlayerType,
                    Avtar = s.Avtar
                });

                standingList = getAll ? data.ToList() : data.Take(5).ToList();

                resultTemp = standingList.OrderByDescending(x => x.TeamPoint).Select(s => new PlayStandingLiteDto()
                {
                    Rank = s.Rank,
                    TeamPoint = s.TeamPoint,
                    UserName = s.UserName,
                    UserPic = s.Avtar
                });

                if (redisHelper.RedisConnected())
                {
                    redisHelper.SaveRedisPlayerPoints(resultTemp);
                }
                return await Task.FromResult(resultTemp);
            }
            else
            {
                return await Task.FromResult(redisHelper.ReadRedisPlayerPoints());
            }


            #endregion

        }

        public async Task<string> UpdateRedisCache()
        {
            IEnumerable<PlayStandingLiteDto> resultTemp;
            MyRedisConnectorHelper redisHelper = new MyRedisConnectorHelper(_appSettings);
            if (!redisHelper.RedisConnected())
            {
                return await Task.FromResult("Not Connected Redis Cache");
            }
            // update current event draw
            #region Update Redis Cache EventDraw
            var resp = await Core.WebProxy.APIResponse($"event_current");
            var curEventDtos = JsonConvert.DeserializeObject<List<CurrentEventDto>>(resp);
            if (curEventDtos != null && curEventDtos.Count > 0)
            {
                var curPBRID = curEventDtos[0].EventDto.PBRID;
                var eventExists = _repoEvent.Query().Filter(x => x.Pbrid == curPBRID).Get().FirstOrDefault();
                if (eventExists != null && eventExists.PerfTimeUTC >= DateTime.UtcNow)
                {
                    var eveId = eventExists.Id;
                    var temp = _repoeventDraw.Query().Filter(x => x.EventId == eveId).Get();
                    if (temp.Count() > 0)
                    {
                        var riderIds = temp.Select(x => x.RiderId).ToList();
                        var bullIds = _repoEventBull.Query().Filter(x => x.EventId == eveId).Get().Select(y => y.BullId).ToList();

                        var temp1 = _repoEvent.Query().Filter(d => d.StartDate.Year == DateTime.Now.Year && !string.IsNullOrEmpty(d.EventResult)).Get().SelectMany(m => JsonConvert.DeserializeObject<IEnumerable<EventResultDto>>(m.EventResult));

                        var evenResults = temp1.Where(x => riderIds.Any(id => id == x.rider_id)).GroupBy(r => r.rider_id).Select(r => new { key = r.Key, point = r.Sum(g => g.rr_rider_score) });
                        var evenResultsBull = temp1.GroupBy(r => r.pbid).Select(r => new { key = r.Key, point = r.Sum(g => g.rr_bull_score) });

                        var drawEvent = (from ed in _repoeventDraw.Query().Filter(x => x.EventId == eveId).Get()
                                         join r in _repoRider.Query().Get() on ed.RiderId equals r.RiderId
                                         join er in _repoEventRider.Query().Filter(x => x.EventId == eveId).Get() on r.Id equals er.RiderId
                                         join b in _repoBull.Query().Get() on ed.BullId equals b.BullId
                                         join eb in _repoEventBull.Query().Filter(x => x.EventId == eveId).Get() on b.Id equals eb.BullId
                                         where ed.EventId == eveId
                                         select new TeamDrawDto
                                         {
                                             Round = ed.Round,
                                             QRProb = ed.QRProb ?? 0,
                                             EventId = ed.EventId,
                                             RiderId = r.Id,
                                             RiderName = r.Name,
                                             RiderTier = er.EventTier,
                                             RiderIsDropout = er.IsDropout ?? false,
                                             WorldRanking = r.Cwrp ?? 9999,
                                             RiderPower = r.RiderPower,
                                             RRTotalpoint = evenResults.FirstOrDefault(k => k.key == r.RiderId)?.point ?? 0,
                                             RiderAvatar = _riderService.GetRiderPic(r.RiderId, _appSettings.MainSiteURL).Result,
                                             RiderIsSelected = false,

                                             BullId = b.Id,
                                             BullName = ed.BullName,
                                             BullTier = eb.EventTier,
                                             BullIsDropout = eb.IsDropout ?? false,
                                             BullOwner = b.Owner,
                                             BullAverageMark = b.AverageMark,
                                             BullPowerRating = b.PowerRating,
                                             BullRankRideScore = evenResultsBull.FirstOrDefault(r => r.key == b.BullId)?.point ?? 0,
                                             BullAvatar = _bullService.GetBullPic(b.BullId, _appSettings.MainSiteURL).Result,
                                             BullIsSelected = false,
                                         }).Distinct().ToList();
                        redisHelper.SaveRedisEventDrawList(drawEvent, eveId);

                        //update extra bulls
                        var drawBullIds = drawEvent.Select(y => y.BullId).ToList();
                        //get remaining bulls
                        var bulls = _repoEventBull.Query()
                          .Filter(x => x.EventId == eveId && x.BullId > 0 && !drawBullIds.Any(id => id == x.BullId))
                          .Includes(x => x.Include(z => z.Bull))
                           .Get().Select(y => new TeamBullDrawDto
                           {
                               BullId = y.BullId,
                               BullName = y.Bull.Name,
                               BullTier = y.EventTier,
                               BullIsDropout = y.IsDropout ?? false,
                               BullOwner = y.Bull.Owner,
                               BullAverageMark = y.Bull.AverageMark,
                               BullPowerRating = y.Bull.PowerRating,
                               BullIsSelected = false,
                               BullAvatar = _bullService.GetBullPic(y.BullId, _appSettings.MainSiteURL).Result,
                               BullRankRideScore = evenResultsBull.FirstOrDefault(r => r.key == y.BullId)?.point ?? 0,
                           }).ToList();
                        redisHelper.SaveRedisExtraBullsList(bulls, eveId);
                    }

                }
            }
            #endregion
            #region Update EventResult

            var lastEvent = _repoEvent.Query().Filter(x => x.PerfTime < DateTime.Now && !string.IsNullOrEmpty(x.EventResult)).OrderBy(x => x.OrderByDescending(y => y.PerfTime)).Get().FirstOrDefault();

            if (lastEvent != null)
            {
                var lastEventId = lastEvent.Id;
                if (!redisHelper.ExistRedisEventResultList(lastEventId) || redisHelper.ReadRedisEventResultList(lastEventId).Count == 0)
                {
                    var bulls = _repoBull.Query().Get();
                    var tempResult = _repoEvent.Query().Filter(d => d.Id == lastEventId && !string.IsNullOrEmpty(d.EventResult)).Get().SelectMany(m => JsonConvert.DeserializeObject<IEnumerable<EventResultDto>>(m.EventResult)).OrderByDescending(x => x.ride_score).ToList();
                    foreach (var rrTemp in tempResult)
                    {
                        rrTemp.bull_name = bulls.Where(y => y.BullId == rrTemp.pbid).Select(x => x.Name).FirstOrDefault();
                        rrTemp.bull_avatar = _bullService.GetBullPic(rrTemp.pbid, _appSettings.MainSiteURL).Result;
                        rrTemp.rider_avatar = _riderService.GetRiderPic(rrTemp.rider_id, _appSettings.MainSiteURL).Result;
                    }

                    redisHelper.SaveRedisEventResultList(tempResult, lastEventId);
                }
            }

            #endregion
            // current standing for homepage
            var joinContestCount = _repoJoinContest.Query().Filter(r => r.IsDelete == false).Get().ToList().Count;
                
            List<StandingDto> standingList = new List<StandingDto>();
            var standings = new List<JoinedContest>();
            var listStandings = new List<StandingDto>();
            bool isPlayer = false;

            standings = _repoJoinContest.Query()
                    .Includes(y => y.Include(u => u.User)
                    .ThenInclude(ud => ud.UserDetail)
                    .Include(t => t.Team)).Filter(d => d.User.UserDetail.Count(r => r.IsActive == true) > 0)
                    .Filter(x => x.CreatedDate < DateTime.Now && x.CreatedDate.Year == DateTime.Now.Year && x.IsDelete == false)
                    .Get().ToList();


            foreach (var item in standings.Select(x => x.UserId).Distinct())
            {

                var standingObj = new StandingDto();
                standingObj.TeamPoint = standings.Where(x => x.UserId == item).Sum(x => x.Team.TeamPoint);
                standingObj.UserName = GetUserName(standings.FirstOrDefault(x => x.UserId == item).User);
                standingObj.NumberOfContest = standings.Where(x => x.UserId == item).Count();
                standingObj.PlayerType = isPlayer ? standings.Where(x => x.UserId == item)
                                    .Select(x => x.User.AspNetUserRoles.FirstOrDefault().Role.NormalizedName)
                                    .FirstOrDefault() : string.Empty;
                standingObj.Avtar = _repoUserDetail.Query().Filter(x => x.UserId == item).Get().FirstOrDefault().Avtar;
                listStandings.Add(standingObj);

            }
            var data = listStandings.OrderByDescending(x => x.TeamPoint).Select(s => new StandingDto()
            {
                TeamPoint = s.TeamPoint,
                //TeamPoint = 0,
                UserName = s.UserName,
                NumberOfContest = s.NumberOfContest,
                Rank = listStandings.Count(x => x.TeamPoint > s.TeamPoint) + 1,
                //Rank = 0,
                PlayerType = s.PlayerType,
                Avtar = s.Avtar
            });

            standingList = data.Take(5).ToList();

            resultTemp = standingList.OrderByDescending(x => x.TeamPoint).Select(s => new PlayStandingLiteDto()
            {
                Rank = s.Rank,
                TeamPoint = s.TeamPoint,
                UserName = s.UserName,
                UserPic = s.Avtar
            });

            redisHelper.SaveRedisPlayerPoints(resultTemp);
            redisHelper.SaveRedisJoinedContestCount(joinContestCount);

            // last event standing for homepage
            List<PlayStandingLiteDto> playContestStanding = new List<PlayStandingLiteDto>();
            List<PlayStandingLiteDto> playContestStandingListTemp = new List<PlayStandingLiteDto>();
            int eventId = 0;
            var eventDetail = _repoEvent.Query()
                .Filter(x => x.PerfTime < DateTime.Now
                      && x.IsActive == true && x.IsDelete == false && x.EventResult != null && x.Type != "15/15")
                .OrderBy(x => x.OrderByDescending(xx => xx.PerfTime))
                .Get()
                .FirstOrDefault();

            if (eventDetail != null)
            {
                eventId = eventDetail.Id;
            }

            var contestList = (from c in _repoContest.Query().Get()
                               join jc in _repoJoinContest.Query().Get() on c.Id equals jc.ContestId
                               join ud in _repoUserDetail.Query().Get() on jc.UserId equals ud.UserId
                               join team in _repoTeam.Query().Get() on jc.TeamId equals team.Id
                               where team.EventId == eventId && team.IsDelete == false && ud.IsPaidMember != true
                               select new
                               {
                                   ContestId = c.Id,
                                   ContestTitle = c.Title,
                                   ud.UserName,
                                   team.TeamPoint,
                                   userpic = ud.Avtar
                               });
            
            foreach (var item in contestList.Distinct())
            {
                var contests = contestList.OrderBy(x => x.ContestId).Where(x => x.ContestId == item.ContestId).Distinct();

                if (!playContestStanding.Any(x => x.ContestId == item.ContestId))
                {
                    playContestStanding = contests.OrderByDescending(x => x.TeamPoint).Select(s => new PlayStandingLiteDto()
                    {
                        ContestId = s.ContestId,
                        ContestTitle = s.ContestTitle,
                        Rank = contests.Count(x => x.TeamPoint > s.TeamPoint) + 1,
                        //Rank = 0,
                        TeamPoint = s.TeamPoint,
                        //TeamPoint = 0,
                        UserName = s.UserName,
                        UserPic = s.userpic
                    }).ToList();
                    playContestStandingListTemp.AddRange(playContestStanding);
                }
            }
                
            redisHelper.SaveRedisPlayContestStandingList(playContestStandingListTemp.Take(5).ToList(), 5);
            redisHelper.SaveRedisPlayContestStandingList(playContestStandingListTemp.ToList(), 0);
            redisHelper.SaveRedisEventID(eventId);
            redisHelper.SaveRedisContestListCount(contestList.ToList().Count);
            return await Task.FromResult("Updated Redis Cache");
        }

        public async Task<List<PlayStandingLiteDto>> GetAllPlayerPointsOfEventContest()
        {
            List<PlayStandingLiteDto> playContestStanding = new List<PlayStandingLiteDto>();
            List<PlayStandingLiteDto> playContestStandingList = new List<PlayStandingLiteDto>();
            int eventId = 0;

            var eventDetail = _repoEvent.Query()
           .Filter(x => x.PerfTime < DateTime.Now && x.EventResult != null)
           .OrderBy(x => x.OrderByDescending(xx => xx.PerfTime))
           .Get()
           .FirstOrDefault();

            if (eventDetail != null)
            {
                eventId = eventDetail.Id;
            }

            var contestList = (from c in _repoContest.Query().Get()
                               join jc in _repoJoinContest.Query().Get() on c.Id equals jc.ContestId
                               join ud in _repoUserDetail.Query().Get() on jc.UserId equals ud.UserId
                               join team in _repoTeam.Query().Get() on jc.TeamId equals team.Id
                               where team.EventId == eventId && team.IsDelete == false
                               select new
                               {
                                   ContestId = c.Id,
                                   ContestTitle = c.Title,
                                   ud.UserName,
                                   team.TeamPoint,
                               });

            foreach (var item in contestList.Distinct())
            {
                var contests = contestList.OrderBy(x => x.ContestId).Where(x => x.ContestId == item.ContestId).Distinct().ToList();

                if (!playContestStanding.Any(x => x.ContestId == item.ContestId))
                {
                    playContestStanding = contests.OrderByDescending(x => x.TeamPoint).Select(s => new PlayStandingLiteDto()
                    {
                        ContestId = s.ContestId,
                        ContestTitle = s.ContestTitle,
                        Rank = contests.Count(x => x.TeamPoint > s.TeamPoint) + 1,
                        //Rank = 0,
                        TeamPoint = s.TeamPoint,
                        //TeamPoint = 0,
                        UserName = s.UserName
                    }).ToList();
                    playContestStandingList.AddRange(playContestStanding);
                }
            }
            return await Task.FromResult(playContestStandingList);
        }

        public async Task<List<PlayStandingLiteDto>> GetLastEventStatndingPlayerPoints(int count = 0)
        {
            List<PlayStandingLiteDto> playContestStanding = new List<PlayStandingLiteDto>();
            List<PlayStandingLiteDto> playContestStandingList = new List<PlayStandingLiteDto>();
            int eventId = 0;

            var eventDetail = _repoEvent.Query()
           .Filter(x => x.PerfTime < DateTime.Now && x.PerfTime > DateTime.Now.AddMonths(-1)
                 && x.IsActive == true && x.IsDelete == false && x.EventResult != null && x.Type != "15/15")
           .OrderBy(x => x.OrderByDescending(xx => xx.PerfTime))
           .Get()
           .FirstOrDefault();

            if (eventDetail != null)
            {
                eventId = eventDetail.Id;
            }
            var joinedContestList = _repoJoinContest.Query().Get();
            var contestList = (from c in _repoContest.Query().Get()
                               join jc in _repoJoinContest.Query().Get() on c.Id equals jc.ContestId
                               join ud in _repoUserDetail.Query().Get() on jc.UserId equals ud.UserId
                               join team in _repoTeam.Query().Get() on jc.TeamId equals team.Id
                               where team.EventId == eventId && team.IsDelete == false && ud.IsPaidMember != true
                               select new PlayStandingLiteDto()
                               {
                                   ContestId = c.Id,
                                   ContestTitle = c.Title,
                                   UserName = ud.UserName,
                                   TeamPoint = team.TeamPoint,
                                   UserPic = !string.IsNullOrEmpty(ud.Avtar) ? (ud.Avtar.Contains("https://") ? ud.Avtar : (ud.Avtar != "/images/RR/user-n.png" ? (_appSettings.MainSiteURL + "/images/profilePicture/" + ud.Avtar) : _appSettings.MainSiteURL + "/images/home/team-icon.png")) : _appSettings.MainSiteURL + "/images/home/team-icon.png",
                                   NumberOfContests = joinedContestList.Where(x => x.UserId == ud.UserId).Count(),
                               }).OrderByDescending(x => x.TeamPoint).ToList();

            foreach (var temp in contestList)
            {
                temp.Rank = contestList.Count(x => x.TeamPoint > temp.TeamPoint) + 1;
            }

            playContestStandingList = count == 0 ? contestList : contestList.Take(count).ToList();

            return await Task.FromResult(playContestStandingList);
        }
        public async Task<string> GetLastEventName()
        {
            var eventDetail = _repoEvent.Query()
                .Filter(x => x.PerfTime < DateTime.Now
                        && x.IsActive == true && x.IsDelete == false && x.EventResult != null && x.Type != "15/15")
                .OrderBy(x => x.OrderByDescending(xx => xx.PerfTime))
                .Get()
                .FirstOrDefault();

            var eventTitle = "";
            if (eventDetail != null)
            {
                eventTitle = eventDetail.Title;
            }
            return await Task.FromResult(eventTitle);
        }
        public async Task<List<PlayStandingLiteDto>> GetLastEventStatndingFreePlayerPoints(int count = 0)
        {
            List<PlayStandingLiteDto> playContestStanding = new List<PlayStandingLiteDto>();
            List<PlayStandingLiteDto> playContestStandingListTemp = new List<PlayStandingLiteDto>();
            int eventId = 0;
            
            MyRedisConnectorHelper redisHelper = new MyRedisConnectorHelper(_appSettings);

            if (!redisHelper.ExistRedisPlayContestStandingList(count) || count == 0)
            {
                var eventDetail = _repoEvent.Query()
                .Filter(x => x.PerfTime < DateTime.Now 
                        && x.IsActive == true && x.IsDelete == false && x.EventResult != null && x.Type != "15/15")
                .OrderBy(x => x.OrderByDescending(xx => xx.PerfTime))
                .Get()
                .FirstOrDefault();

                if (eventDetail != null)
                {
                    eventId = eventDetail.Id;
                }
                var joinedContestList = _repoJoinContest.Query().Get();

                var contestList = (from c in _repoContest.Query().Get()
                                   join jc in _repoJoinContest.Query().Get() on c.Id equals jc.ContestId
                                   join ud in _repoUserDetail.Query().Get() on jc.UserId equals ud.UserId
                                   join team in _repoTeam.Query().Get() on jc.TeamId equals team.Id
                                   where team.EventId == eventId && team.IsDelete == false && ud.IsPaidMember != true
                                   select new PlayStandingLiteDto()
                                   {
                                       ContestId = c.Id,
                                       ContestTitle = c.Title,
                                       UserName = ud.UserName,
                                       TeamPoint = team.TeamPoint,
                                       UserPic = !string.IsNullOrEmpty(ud.Avtar) ? (ud.Avtar.Contains("https://") ? ud.Avtar : (ud.Avtar != "/images/RR/user-n.png" ? (_appSettings.MainSiteURL + "/images/profilePicture/" + ud.Avtar) : _appSettings.MainSiteURL + "/images/home/team-icon.png")) : _appSettings.MainSiteURL + "/images/home/team-icon.png",
                                       NumberOfContests = joinedContestList.Where(x => x.UserId == ud.UserId).Count(),
                                   }).OrderByDescending(x => x.TeamPoint).ToList();

                foreach (var temp in contestList)
                {
                    temp.Rank = contestList.Count(x => x.TeamPoint > temp.TeamPoint) + 1;
                }
                /*foreach (var item in contestList.Distinct())
                {

                    var contests = contestList.OrderBy(x => x.ContestId).Where(x => x.ContestId == item.ContestId).Distinct();

                    if (!playContestStanding.Any(x => x.ContestId == item.ContestId))
                    {
                        playContestStanding = contests.OrderByDescending(x => x.TeamPoint).Select(s => new PlayStandingLiteDto()
                        {
                            ContestId = s.ContestId,
                            ContestTitle = s.ContestTitle,
                            Rank = contests.Count(x => x.TeamPoint > s.TeamPoint) + 1,
                            TeamPoint = s.TeamPoint,
                            UserName = s.UserName,
                            UserPic = s.userpic,
                            NumberOfContests = s.NumberOfContests,
                        }).ToList();
                        playContestStandingListTemp.AddRange(playContestStanding);
                    }
                }*/

                playContestStandingListTemp = count == 0 ? contestList : contestList.Take(count).ToList();
                if (redisHelper.RedisConnected())
                {
                    redisHelper.SaveRedisPlayContestStandingList(playContestStandingListTemp, count);
                }
                return await Task.FromResult(playContestStandingListTemp);
            }
            else
            {
                return await Task.FromResult(redisHelper.ReadRedisPlayContestStandingList(count));
            }

        }
        public async Task<List<PlayStandingLiteDto>> GetTopReferred(int count = 0)
        {
            List<PlayStandingLiteDto> topReferred = new List<PlayStandingLiteDto>();
            var joinedContestList = _repoJoinContest.Query().Get();
            var eventDetail = _repoUserDetail.Query()
                .Filter(x => x.ReferredCustomers > 0
                        && x.IsActive == true && x.IsDelete == false && (x.CustomReferralCode == null || x.CustomReferralCode == ""))
                .OrderBy(x => x.OrderByDescending(xx => xx.ReferredCustomers))
                .Get();
            int i = 1;
            foreach (var ud in eventDetail)
            {
                var tempPlayStanding = new PlayStandingLiteDto() {
                    Rank = eventDetail.Count(x => x.ReferredCustomers > ud.ReferredCustomers) + 1,
                    TeamPoint = (decimal)(ud.ReferredCustomers != null ? ud.ReferredCustomers * _appSettings.VoucherifyReferralPoints : 0),
                    UserName = ud.UserName,
                    UserPic = !string.IsNullOrEmpty(ud.Avtar) ? (ud.Avtar.Contains("https://") ? ud.Avtar : (ud.Avtar != "/images/RR/user-n.png" ? (_appSettings.MainSiteURL + "/images/profilePicture/" + ud.Avtar) : _appSettings.MainSiteURL + "/images/home/team-icon.png")) : _appSettings.MainSiteURL + "/images/home/team-icon.png",
                    NumberOfContests = joinedContestList.Where(x => x.UserId == ud.UserId).Count(),
                };
                topReferred.Add(tempPlayStanding);
                i++;
            }
            return await Task.FromResult(count == 0 ? topReferred : topReferred.Take(count).ToList());
        }
        
        public async Task<List<StandingDto>> GetStandings(bool isPlayer = false, int start = 0, int length = 10, int column = 0, string searchStr = "", string sort = "")
        {
            List<StandingDto> standing = new List<StandingDto>();
            List<StandingDto> standingList = new List<StandingDto>();
            var standings = new List<JoinedContest>();
            if (isPlayer)
            {
                standings = _repoJoinContest.Query()
                .Includes(y => y.Include(u => u.User)
                .ThenInclude(ur => ur.AspNetUserRoles)
                .ThenInclude(r => r.Role)
                .Include(ud => ud.User.UserDetail)
                .Include(t => t.Team))
                .Filter(x => x.IsDelete == false)
                .Get().ToList();
            }
            else
            {
                standings = _repoJoinContest.Query()
                     .Includes(y => y.Include(u => u.User)
                     .ThenInclude(ud => ud.UserDetail)
                     .Include(t => t.Team))
                     .Filter(x => x.CreatedDate < DateTime.Now && x.CreatedDate.Year == DateTime.Now.Year && x.IsDelete == false && x.User.UserDetail.Count(r => r.IsActive == true) > 0)
                     .Get().ToList();
            }
            //.Filter(x => x.CreatedDate < DateTime.Now && x.CreatedDate > DateTime.Now.AddYears(-1) && x.IsDelete == false && x.User.UserDetail.Count(r=>r.IsActive==true) > 0)

            var listStandings = new List<StandingDto>();

            if (isPlayer)
            {
                foreach (var item in standings)
                {
                    if (!listStandings.Any(x => x.UserId == item.UserId))
                    {
                        var standingObj = new StandingDto();
                        standingObj.UserId = item.UserId;
                        standingObj.TeamId = item.TeamId;
                        standingObj.TeamPoint = standings.Where(x => x.UserId == item.UserId).Sum(x => x.Team.TeamPoint);
                        standingObj.UserName = GetUserName(standings.FirstOrDefault(x => x.UserId == item.UserId).User);
                        standingObj.NumberOfContest = standings.Where(x => x.UserId == item.UserId).Count();
                        standingObj.PlayerType = isPlayer ? standings.Where(x => x.UserId == item.UserId)
                             .Select(x => x.User.AspNetUserRoles.FirstOrDefault() != null ? x.User.AspNetUserRoles.FirstOrDefault().Role.NormalizedName : "")
                             .FirstOrDefault() : string.Empty;
                        standingObj.Avtar = item.User.UserDetail.FirstOrDefault(x => x.UserId == item.UserId)?.Avtar;
                        listStandings.Add(standingObj);
                    }
                }
                foreach (var item in listStandings)
                {
                    //var playerStandings = listStandings.OrderBy(x => x.PlayerType)
                    //     .Where(x => x.PlayerType == item.PlayerType)
                    //     .ToList();

                    if (!standingList.Any(x => x.PlayerType == item.PlayerType))
                    {
                        standing = listStandings.OrderByDescending(x => x.TeamPoint).Select(s => new StandingDto()
                        {
                            Avtar = !string.IsNullOrEmpty(s.Avtar) ? (s.Avtar.Contains("https://") ? s.Avtar : (s.Avtar != "/images/RR/user-n.png" ? (_appSettings.MainSiteURL + "/images/profilePicture/" + s.Avtar) : _appSettings.MainSiteURL + "/images/home/team-icon.png")) : _appSettings.MainSiteURL + "/images/home/team-icon.png",
                            UserId = item.UserId,
                            Rank = listStandings.Count(x => x.TeamPoint > s.TeamPoint) + 1,
                            // Rank = 0,
                            TeamPoint = s.TeamPoint,
                            //TeamPoint =0,
                            UserName = s.UserName,
                            PlayerType = s.PlayerType,
                            NumberOfContest = s.NumberOfContest
                        }).ToList();
                        standingList.AddRange(standing);
                    }
                }
            }
            else
            {
                foreach (var item in standings.Select(x => x.UserId).Distinct())
                {
                    var standingObj = new StandingDto();
                    standingObj.Avtar = standings.Where(x => x.UserId == item).Select(x => x.User.UserDetail.Count > 0 ? x.User.UserDetail.FirstOrDefault().Avtar : "").FirstOrDefault();
                    standingObj.TeamPoint = standings.Where(x => x.UserId == item).Sum(x => x.Team.TeamPoint);
                    standingObj.UserName = GetUserName(standings.FirstOrDefault(x => x.UserId == item).User);
                    standingObj.NumberOfContest = standings.Where(x => x.UserId == item).Count();
                    standingObj.PlayerType = isPlayer ? standings.Where(x => x.UserId == item)
                                        .Select(x => x.User.AspNetUserRoles.FirstOrDefault() != null ? x.User.AspNetUserRoles.FirstOrDefault().Role.NormalizedName : "")
                                        .FirstOrDefault() : string.Empty;
                    standingObj.UserId = item;
                    listStandings.Add(standingObj);
                }
                standingList = listStandings.OrderByDescending(x => x.TeamPoint).Select(s => new StandingDto()
                {
                    Avtar = !string.IsNullOrEmpty(s.Avtar) ? (s.Avtar.Contains("https://") ? s.Avtar : (s.Avtar != "/images/RR/user-n.png" ? (_appSettings.MainSiteURL + "/images/profilePicture/" + s.Avtar) : _appSettings.MainSiteURL + "/images/home/team-icon.png")) : _appSettings.MainSiteURL + "/images/home/team-icon.png",
                    TeamPoint = s.TeamPoint,
                    //TeamPoint =0,
                    UserName = s.UserName,
                    NumberOfContest = s.NumberOfContest,
                    Rank = listStandings.Count(x => x.TeamPoint > s.TeamPoint) + 1,
                    // Rank = 0,
                    PlayerType = s.PlayerType,
                    UserId = s.UserId
                }).ToList();
            }
            switch (column)
            {
                case 0:
                    standingList = (sort == "desc" ? standingList.OrderByDescending(xx => xx.Rank) : standingList.OrderBy(xx => xx.Rank)).ToList();
                    break;
                case 1:
                    standingList = (sort == "desc" ? standingList.OrderByDescending(xx => xx.UserName) : standingList.OrderBy(xx => xx.UserName)).ToList();
                    break;
                case 2:
                    standingList = (sort == "desc" ? standingList.OrderByDescending(xx => xx.NumberOfContest) : standingList.OrderBy(xx => xx.NumberOfContest)).ToList();
                    break;
                case 3:
                    standingList = (sort == "desc" ? standingList.OrderByDescending(xx => xx.TeamPoint) : standingList.OrderBy(xx => xx.TeamPoint)).ToList();
                    break;
                default:
                    standingList = (sort == "desc" ? standingList.OrderByDescending(xx => xx.Rank) : standingList.OrderBy(xx => xx.Rank)).ToList();
                    break;
            }
            return await Task.FromResult(standingList.Where(x => !string.IsNullOrEmpty(x.UserName) && x.UserName.ToLower().Contains(searchStr.ToLower())).ToList());

        }

        public async Task<List<StandingDto>> GetYearStandingsOfCurrentUser(string userId, int start = 0, int length = 10, int column = 0, string searchStr = "", string sort = "")
        {
            List<StandingDto> standing = new List<StandingDto>();
            List<StandingDto> standingList = new List<StandingDto>();
            var standings = new List<JoinedContest>();

            standings = _repoJoinContest.Query()
                 .Includes(y => y.Include(u => u.User)
                 .ThenInclude(ud => ud.UserDetail)
                 .Include(t => t.Team))
                 .Filter(x => x.CreatedDate < DateTime.Now && x.CreatedDate.Year == DateTime.Now.Year && x.IsDelete == false && x.UserId == userId)
                 .Get().ToList();

            var listStandings = new List<StandingDto>();
            //.Filter(x => x.CreatedDate < DateTime.Now && x.CreatedDate > DateTime.Now.AddYears(-1) && x.IsDelete == false && x.User.UserDetail.Count(r=>r.IsActive==true) > 0)

            foreach (var item in standings.Select(x => x.UserId).Distinct())
            {
                var standingObj = new StandingDto();
                standingObj.Avtar = standings.Where(x => x.UserId == item).Select(x => x.User.UserDetail.FirstOrDefault().Avtar).FirstOrDefault();
                standingObj.TeamPoint = standings.Where(x => x.UserId == item).Sum(x => x.Team.TeamPoint);
                standingObj.UserName = GetUserName(standings.FirstOrDefault(x => x.UserId == item).User);
                standingObj.NumberOfContest = standings.Where(x => x.UserId == item).Count();
                standingObj.PlayerType = string.Empty;
                listStandings.Add(standingObj);
            }
            standingList = listStandings.OrderByDescending(x => x.TeamPoint).Select(s => new StandingDto()
            {
                Avtar = !string.IsNullOrEmpty(s.Avtar) ? (s.Avtar.Contains("https://") ? s.Avtar : (s.Avtar != "/images/RR/user-n.png" ? (_appSettings.MainSiteURL + "/images/profilePicture/" + s.Avtar) : _appSettings.MainSiteURL + "/images/home/team-icon.png")) : _appSettings.MainSiteURL + "/images/home/team-icon.png",
                TeamPoint = s.TeamPoint,
                UserName = s.UserName,
                NumberOfContest = s.NumberOfContest,
                Rank = listStandings.Count(x => x.TeamPoint > s.TeamPoint) + 1,
                PlayerType = s.PlayerType
            }).ToList();

            switch (column)
            {
                case 0:
                    standingList = (sort == "desc" ? standingList.OrderByDescending(xx => xx.Rank) : standingList.OrderBy(xx => xx.Rank)).ToList();
                    break;
                case 1:
                    standingList = (sort == "desc" ? standingList.OrderByDescending(xx => xx.UserName) : standingList.OrderBy(xx => xx.UserName)).ToList();
                    break;
                case 2:
                    standingList = (sort == "desc" ? standingList.OrderByDescending(xx => xx.NumberOfContest) : standingList.OrderBy(xx => xx.NumberOfContest)).ToList();
                    break;
                case 3:
                    standingList = (sort == "desc" ? standingList.OrderByDescending(xx => xx.TeamPoint) : standingList.OrderBy(xx => xx.TeamPoint)).ToList();
                    break;
                default:
                    standingList = (sort == "desc" ? standingList.OrderByDescending(xx => xx.Rank) : standingList.OrderBy(xx => xx.Rank)).ToList();
                    break;
            }
            return await Task.FromResult(standingList.Where(x => x.UserName.ToLower().Contains(searchStr.ToLower())).ToList());
        }

        public async Task<Tuple<List<StandingDto>, List<StandingDto>, List<StandingDto>>> GetStandingsApi(bool isPlayer = false)
        {
            List<StandingDto> standing = new List<StandingDto>();
            List<StandingDto> standingList = new List<StandingDto>();
            var noviceStandings = new List<StandingDto>();
            var intermediateStandings = new List<StandingDto>();
            var proStandings = new List<StandingDto>();
            var standings = new List<JoinedContest>();
            if (isPlayer)
            {
                standings = _repoJoinContest.Query().Filter(x => x.IsDelete == false)
                 .Includes(y => y.Include(u => u.User)
                 .ThenInclude(ur => ur.AspNetUserRoles)
                 .ThenInclude(r => r.Role)
                 .Include(ud => ud.User.UserDetail)
                 .Include(t => t.Team))
                 .Get().ToList();
            }
            else
            {
                standings = _repoJoinContest.Query()
                     .Filter(x => x.CreatedDate < DateTime.Now && x.CreatedDate.Year > DateTime.Now.Year && x.IsDelete == false)
                     .Includes(y => y.Include(u => u.User)
                     .ThenInclude(ud => ud.UserDetail)
                     .Include(t => t.Team))
                     .Get().ToList();
            }

            //.Filter(x => x.CreatedDate < DateTime.Now && x.CreatedDate.Year == DateTime.Now.Year && x.IsDelete == false && x.User.UserDetail.Count(r=>r.IsActive==true) > 0)

            var listStandings = new List<StandingDto>();

            if (isPlayer)
            {
                foreach (var item in standings)
                {
                    if (!listStandings.Any(x => x.UserId == item.UserId))
                    {
                        var standingObj = new StandingDto();
                        standingObj.UserId = item.UserId;
                        standingObj.TeamId = item.TeamId;
                        standingObj.TeamPoint = standings.Where(x => x.UserId == item.UserId).Sum(x => x.Team.TeamPoint);
                        standingObj.UserName = GetUserName(standings.FirstOrDefault(x => x.UserId == item.UserId).User);
                        standingObj.NumberOfContest = standings.Where(x => x.UserId == item.UserId).Count();
                        standingObj.PlayerType = isPlayer ? standings.Where(x => x.UserId == item.UserId)
                             .Select(x => x.User.AspNetUserRoles.FirstOrDefault() != null ? x.User.AspNetUserRoles.FirstOrDefault().Role.NormalizedName : "")
                             .FirstOrDefault() : string.Empty;
                        listStandings.Add(standingObj);
                    }
                }
                foreach (var item in listStandings)
                {
                    var playerStandings = listStandings.OrderBy(x => x.PlayerType)
                         .Where(x => x.PlayerType == item.PlayerType)
                         .ToList();

                    if (!standingList.Any(x => x.PlayerType == item.PlayerType))
                    {
                        standing = playerStandings.OrderByDescending(x => x.TeamPoint).Select(s => new StandingDto()
                        {
                            UserId = item.UserId,
                            Rank = playerStandings.Count(x => x.TeamPoint > s.TeamPoint) + 1,
                            // Rank = 0,
                            TeamPoint = s.TeamPoint,
                            //TeamPoint = 0,
                            UserName = s.UserName,
                            PlayerType = s.PlayerType,
                            NumberOfContest = s.NumberOfContest
                        }).ToList();
                        standingList.AddRange(standing);
                    }
                }
            }
            else
            {
                foreach (var item in standings.Select(x => x.UserId).Distinct())
                {
                    var standingObj = new StandingDto();
                    standingObj.TeamPoint = standings.Where(x => x.UserId == item).Sum(x => x.Team.TeamPoint);
                    standingObj.UserName = GetUserName(standings.FirstOrDefault(x => x.UserId == item).User);
                    standingObj.NumberOfContest = standings.Where(x => x.UserId == item).Count();
                    standingObj.PlayerType = isPlayer ? standings.Where(x => x.UserId == item)
                                        .Select(x => x.User.AspNetUserRoles.FirstOrDefault() != null ? x.User.AspNetUserRoles.FirstOrDefault().Role.NormalizedName : "")
                                        .FirstOrDefault() : string.Empty;
                    listStandings.Add(standingObj);
                }
                standingList = listStandings.OrderByDescending(x => x.TeamPoint).Select(s => new StandingDto()
                {
                    TeamPoint = s.TeamPoint,
                    //TeamPoint = 0,
                    UserName = s.UserName,
                    NumberOfContest = s.NumberOfContest,
                    Rank = listStandings.Count(x => x.TeamPoint > s.TeamPoint) + 1,
                    //Rank = 0,
                    PlayerType = s.PlayerType
                }).ToList();
            }
            if (standingList.FirstOrDefault(x => x.PlayerType == "NOVICE PLAYER") != null)
            {

                foreach (var item in standingList.Where(x => x.PlayerType == "NOVICE PLAYER"))
                {
                    noviceStandings.Add(item);
                }
            }
            if (standingList.FirstOrDefault(x => x.PlayerType == "INTERMEDIATE PLAYER") != null)
            {

                foreach (var item in standingList.Where(x => x.PlayerType == "INTERMEDIATE PLAYER"))
                {
                    intermediateStandings.Add(item);
                }
            }
            if (standingList.FirstOrDefault(x => x.PlayerType == "PRO PLAYER") != null)
            {

                foreach (var item in standingList.Where(x => x.PlayerType == "PRO PLAYER"))
                {
                    proStandings.Add(item);
                }
            }
            if (noviceStandings.Count > 0 || intermediateStandings.Count > 0 || proStandings.Count > 0)
            {
                return await Task.FromResult(new Tuple<List<StandingDto>, List<StandingDto>, List<StandingDto>>(noviceStandings, intermediateStandings, proStandings));
            }
            return await Task.FromResult(new Tuple<List<StandingDto>, List<StandingDto>, List<StandingDto>>(standingList, null, null));
        }

        public async Task<List<Team>> GetJoinedTeamsByEventIdWinningService(int eventId)
        {
            var teamList = new List<Team>();
            teamList = (from team in _repoTeam.Query().Filter(x => x.EventId == eventId && x.IsDelete == false)
                        .Includes(t => t.Include(tb => tb.TeamBull).Include(tr => tr.TeamRider).Include(tr => tr.JoinedContest)).Get()
                            //join jc in _repoJoinContest.Query().Get() on team.Id equals jc.TeamId
                            //where team.EventId == eventId && team.IsDelete == false
                        select new Team()
                        {
                            EventId = team.EventId,
                            Id = team.Id,
                            TeamBull = team.TeamBull,
                            TeamRider = team.TeamRider,
                            UserId = team.UserId,
                            TeamPoint = team.TeamPoint,
                            JoinedContest = team.JoinedContest,
                        }).ToList();

            return await Task.FromResult(teamList);
        }
        private string GetUserName(Data.AspNetUsers user)
        {
            string username = "";

            if (user == null)
                return username;

            if (user.UserDetail?.Count > 0)
                username = user.UserDetail.FirstOrDefault().UserName;

            if (string.IsNullOrEmpty(username))
                username = user.UserName.Split('@')[0];

            return username;
        }

        public async Task DeleteTeam(int TeamId)
        {
            var getTeamBull = _repoTeamBull.Query()
                 .Filter(x => x.TeamId == TeamId).Get().ToList();
            var getTeamRider = _repoTeamRider.Query()
                .Filter(x => x.TeamId == TeamId).Get().ToList();
            var joinContest = _repoJoinContest.Query()
                 .Filter(x => x.TeamId == TeamId).Get().ToList();

            await _repoTeamBull.DeleteCollection(getTeamBull);
            await _repoTeamRider.DeleteCollection(getTeamRider);
            await _repoJoinContest.DeleteCollection(joinContest);
            await _repoTeam.DeleteAsync(TeamId);
        }

        /// <summary>
        /// Dispose Team And Event Draw Service 
        /// </summary>
        public void Dispose()
        {
            if (_repoeventDraw != null)
            {
                _repoeventDraw.Dispose();
            }
            if (_repoTeam != null)
            {
                _repoTeam.Dispose();
            }

            if (_repoEvent != null)
            {
                _repoEvent.Dispose();
            }
            if (_repoJoinContest != null)
            {
                _repoJoinContest.Dispose();
            }
            if (_repoTeamBull != null)
            {
                _repoTeamBull.Dispose();
            }
            if (_repoTeamRider != null)
            {
                _repoTeamRider.Dispose();
            }
            if (_repoEventBull != null)
            {
                _repoEventBull.Dispose();
            }
            if (_repoEventRider != null)
            {
                _repoEventRider.Dispose();
            }
            if (_repoUserDetail != null)
            {
                _repoUserDetail.Dispose();
            }
            if (_repoContest != null)
            {
                _repoContest.Dispose();
            }
        }

        public async Task<int> GetTeamIdByEventIdUserId(int eventId, string userId)
        {
            var team = _repoTeam.Query().Filter(x => x.IsDelete == false && x.EventId == eventId && x.UserId == userId).Get().FirstOrDefault();
            int teamId = team != null ? team.Id : 0;
            return await Task.FromResult(teamId);
        }
    }
}
