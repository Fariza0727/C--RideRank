using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RR.AdminData;
using RR.Core;
using RR.Data;
using RR.Dto;
using RR.Dto.Team;
using RR.Mapper;
using RR.Repo;
using RR.StaticData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RR.Service
{
    public class LongTermTeamService : ILongTermTeamService
    {
        #region Constructor

        private readonly IRepository<LongTermTeam, RankRideContext> _repoLongTermTeam;
        private readonly IRepository<LongTermTeamBull, RankRideContext> _repoTeamBull;
        private readonly IRepository<LongTermTeamRider, RankRideContext> _repoTeamRider;
        private readonly IRepository<Bull, RankRideStaticContext> _repoBull;
        private readonly IRepository<Rider, RankRideStaticContext> _repoRider;
        private readonly IRepository<UserDetail, RankRideContext> _repoUserDetail;
        private readonly IHostingEnvironment _env;
        private readonly IRepository<EventDraw, RankRideStaticContext> _repoeventDraw;
        private readonly IRepository<JoinedContest, RankRideContext> _repoJoinContest;
        private readonly IRepository<Event, RankRideStaticContext> _repoEvent;
        private readonly IRepository<EventBull, RankRideStaticContext> _repoEventBull;
        private readonly IRepository<EventRider, RankRideStaticContext> _repoEventRider;
        private readonly IRepository<Contest, RankRideAdminContext> _repoContest;
        private readonly IBullRiderPicturesService _bullRiderPictures;
        private AppSettings _appSettings;

        public LongTermTeamService(IRepository<EventDraw, RankRideStaticContext> repoeventDraw,
             IRepository<LongTermTeam, RankRideContext> repoTeam,
             IRepository<LongTermTeamBull, RankRideContext> repoTeamBull,
             IRepository<LongTermTeamRider, RankRideContext> repoTeamRider,
             IRepository<Bull, RankRideStaticContext> repoBull,
             IRepository<Rider, RankRideStaticContext> repoRider,
             IRepository<UserDetail, RankRideContext> repoUserDetail,
             IHostingEnvironment env,

             IRepository<Event, RankRideStaticContext> repoEvent,
             IRepository<JoinedContest, RankRideContext> repoJoinContest,
             IRepository<EventBull, RankRideStaticContext> repoEventBull,
             IRepository<EventRider, RankRideStaticContext> repoEventRider,
             IRepository<Contest, RankRideAdminContext> repoContest,
             IBullRiderPicturesService bullRiderPictures,
             IOptions<AppSettings> appSettings)
        {
            _repoeventDraw = repoeventDraw;
            _repoLongTermTeam = repoTeam;
            _repoEvent = repoEvent;
            _repoJoinContest = repoJoinContest;
            _repoTeamBull = repoTeamBull;
            _repoTeamRider = repoTeamRider;
            _repoEventBull = repoEventBull;
            _repoEventRider = repoEventRider;
            _repoUserDetail = repoUserDetail;
            _env = env;
            _repoContest = repoContest;
           _bullRiderPictures = bullRiderPictures;
            _repoBull = repoBull;
            _repoRider = repoRider;
            _appSettings = appSettings.Value;
        }

        #endregion
        public async Task UpdateBrandIcon(LongTermTeam team, string brand, IFormFile File)
        {

            if (team != null && File != null && File.Length > 0)
            {
                var filePath = string.Concat(_appSettings.ProfilePicPath.Replace("profilePicture", ""));
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);

                string fileExt = Path.GetExtension(File.FileName);
                string tempfileName = string.Concat(Guid.NewGuid().ToString(), fileExt);

                if (!(new string[] { ".jpg", ".png", ".jpeg" }).Contains(fileExt))
                {
                    tempfileName = Path.ChangeExtension(tempfileName, ".jpg");
                    fileExt = ".jpg";
                }

                var fileSaveAs = string.Concat(filePath, tempfileName);
                using (var stream1 = new FileStream(fileSaveAs, FileMode.Create))
                {
                    await File.CopyToAsync(stream1);
                }

                team.TeamIcon = "/images/" + tempfileName;
                team.TeamBrand = brand;

            }
        }


        public async Task<LongTermTeamInfoDto> GetTeam(string userId)
        {
            var entity_ = _repoLongTermTeam.Query().Filter(r => r.UserId == userId).Get().SingleOrDefault();
            if (entity_ != null)
            {
                return await Task.FromResult(new LongTermTeamInfoDto
                {
                    Id = entity_.Id,
                    CreatedBy = entity_.CreatedBy,
                    CreatedDate = entity_.CreatedDate,
                    ExpiredDate = entity_.ExpiredDate,
                    IsDelete = entity_.IsDelete,
                    TeamBrand = entity_.TeamBrand,
                    TeamIcon = entity_.TeamIcon,
                    TeamPoint = entity_.TeamPoint,
                    UpdatedBy = entity_.UpdatedBy,
                    UpdatedDate = entity_.UpdatedDate,
                    UserId = entity_.UserId
                });
            }

            return default(LongTermTeamInfoDto);
        }

        public async Task<LongTermTeamInfoDto> GetTeam(int teamId)
        {
            var entity_ = _repoLongTermTeam.Query().Filter(r => r.Id == teamId).Get().SingleOrDefault();
            if (entity_ != null)
            {
                return await Task.FromResult(new LongTermTeamInfoDto
                {
                    Id = entity_.Id,
                    CreatedBy = entity_.CreatedBy,
                    CreatedDate = entity_.CreatedDate,
                    ExpiredDate = entity_.ExpiredDate,
                    IsDelete = entity_.IsDelete,
                    TeamBrand = entity_.TeamBrand,
                    TeamIcon = entity_.TeamIcon,
                    TeamPoint = entity_.TeamPoint,
                    UpdatedBy = entity_.UpdatedBy,
                    UpdatedDate = entity_.UpdatedDate,
                    UserId = entity_.UserId
                });
            }

            return default(LongTermTeamInfoDto);
        }

        public async Task<LongTermTeamFormationDto> LongTermTeamById(int teamId, int pageno, int pageSize, bool isBull = false, string search = "", int column = 0, string sort = "", string userId = "")
        {
            LongTermTeamFormationDto formationDto = new LongTermTeamFormationDto();

            LongTermTeam existTeam = new LongTermTeam();
            existTeam = _repoLongTermTeam.Query().Filter(r => r.UserId == userId).Get().SingleOrDefault();

            if (teamId > 0)
                existTeam = _repoLongTermTeam.Query().Filter(r => r.Id == teamId).Get().SingleOrDefault();

            existTeam = existTeam ?? new LongTermTeam();

            teamId = existTeam.Id;
            formationDto.TeamAvatar = existTeam.TeamIcon;
            formationDto.TeamBrand = existTeam.TeamBrand;
            formationDto.TeamId = teamId;
            formationDto.BullArray = new DataResult<List<LTTeamBullFormationDto>, List<LTTeamBullFormationDto>, List<LTTeamBullFormationDto>>();
            formationDto.RiderArray = new DataResult<List<LTTeamRiderFormationDto>, List<LTTeamRiderFormationDto>, List<LTTeamRiderFormationDto>>();

            formationDto.BullList = new List<LTTeamBullFormationDto>();
            formationDto.RiderList = new List<LTTeamRiderFormationDto>();

            try
            {
                var teamBull = _repoTeamBull.Query().Get();
                var teamRider = _repoTeamRider.Query().Get();

                if (isBull)
                {
                    var bpredicate = PredicateBuilder.True<Bull>()
                          .And(x => x.BullId > 0);

                    if (!string.IsNullOrEmpty(search))
                        bpredicate = bpredicate.And(r => r.Name.Contains(search));

                    int totalbullCount = 0;
                    var Querybulls = _repoBull.Query()
                      .Filter(bpredicate);

                    if (column == 1)
                        Querybulls = (sort == "desc" ? Querybulls.OrderBy(x => x.OrderByDescending(xx => xx.Name)) : Querybulls.OrderBy(x => x.OrderBy(xx => xx.Name)));

                    var bulls = Querybulls.GetPage(pageno, pageSize, out totalbullCount);

                    var addedbullsIds = teamBull.Where(t => t.TeamId == teamId).Select(n => n.BullId);

                    var addedBulls = bulls.Where(r => addedbullsIds.Contains(r.Id))
                      .Select(bl => new LTTeamBullFormationDto
                      {
                          TeamId = existTeam.Id,
                          IsSelected = true,
                          BullId = bl.Id,
                          BullName = bl.Name,
                          BullTier = teamBull.FirstOrDefault(x => x.BullId == bl.Id && x.TeamId == existTeam.Id)?.BullTier, 
                          BullAvatar = (_bullRiderPictures.GetBullPic(bl.Id)).Result
                          
                      });

                    var newBulls = bulls.Where(r => !addedbullsIds.Contains(r.Id))
                    .Select(bl => new LTTeamBullFormationDto
                    {
                        TeamId = 0,
                        IsSelected = false,
                        BullId = bl.Id,
                        BullName = bl.Name,
                        BullAvatar = (_bullRiderPictures.GetBullPic(bl.Id)).Result,
                        BullTier = GetTier(),
                    });

                    formationDto.BullList = addedBulls.Concat(newBulls);

                    formationDto.totalBulls = totalbullCount;
                }
                else
                {
                    int totalriderCount = 0;

                    var rpredicate = PredicateBuilder.True<Rider>()
                            .And(x => x.RiderId > 0);

                    if (!string.IsNullOrEmpty(search))
                        rpredicate = rpredicate.And(r => r.Name.ToLower().Trim().Contains(search.ToLower().Trim()));

                    var Queryriders = _repoRider.Query().Filter(rpredicate);

                    if (column == 1)
                        Queryriders = (sort == "desc" ? Queryriders.OrderBy(x => x.OrderByDescending(xx => xx.Name)) : Queryriders.OrderBy(x => x.OrderBy(xx => xx.Name)));


                    var riders = Queryriders.GetPage(pageno, pageSize, out totalriderCount);
                    var addedRidersIds = teamRider.Where(t => t.TeamId == teamId).Select(n => n.RiderId);

                    var addedRiderss = riders.Where(r => addedRidersIds.Contains(r.Id))
                     .Select(bl => new LTTeamRiderFormationDto
                     {
                         TeamId = existTeam.Id,
                         IsSelected = true,
                         RiderId = bl.Id,
                         RiderName = bl.Name,
                         RiderAvatar = (_bullRiderPictures.GetRiderPic(bl.Id)).Result,
                         RiderTier = teamRider.FirstOrDefault(x => x.RiderId == bl.Id && x.TeamId == existTeam.Id)?.RiderTier,
                     });

                    var newRiders = riders.Where(r => !addedRidersIds.Contains(r.Id))
                    .Select(bl => new LTTeamRiderFormationDto
                    {
                        TeamId = 0,
                        IsSelected = false,
                        RiderId = bl.Id,
                        RiderName = bl.Name,
                        RiderAvatar = (_bullRiderPictures.GetRiderPic(bl.Id)).Result,
                        RiderTier = GetTier(),
                    });

                    formationDto.RiderList = addedRiderss.Concat(newRiders);

                    formationDto.totalRiders = totalriderCount;
                }

            }
            catch (Exception ex)
            {


            }


            return await Task.FromResult(formationDto);
        }

        public async Task<int> CreateTeam(IEnumerable<LongTermTeamDto> teamDto, CreateLongTermTeamRequestDto requestDto)
        {

            var bigTeamContest = teamDto.Where(x => x.TeamNumber == 1 && x.TeamId == requestDto.TeamId);
            var otherContest = teamDto.Where(x => x.TeamNumber == 2 && x.TeamId == requestDto.TeamId);

            if (bigTeamContest != null && bigTeamContest.Count() > 0)
            {
                var teamModel = new LongTermTeam();

                string iDate = "12/31/" + DateTime.Now.Year;
                DateTime oDate = Convert.ToDateTime(iDate);

                teamModel = new LongTermTeam
                {
                    UserId = requestDto.UserId,
                    CreatedDate = DateTime.Now,
                    IsDelete = false,
                    CreatedBy = requestDto.UserId,
                    UpdatedBy = requestDto.UserId,
                    UpdatedDate = DateTime.Now,
                    ExpiredDate = oDate
                };
                foreach (var team in bigTeamContest)
                {
                    if (team.BullId > 0)
                    {
                        teamModel.LongTermTeamBull.Add(new LongTermTeamBull
                        {
                            BullId = team.BullId,
                            IsSubstitute = team.IsSubstitute,
                            BullTier = team.Tier,
                            TeamId = requestDto.TeamId
                        });
                    }
                    else
                    {
                        teamModel.LongTermTeamRider.Add(new LongTermTeamRider
                        {
                            RiderId = team.RiderId,
                            IsSubstitute = team.IsSubstitute,
                            RiderTier = team.Tier,
                            TeamId = requestDto.TeamId
                        });
                    }
                }
                await _repoLongTermTeam.InsertGraphAsync(teamModel);
                await UpdateBrandIcon(teamModel, requestDto.BrandName, requestDto.IconFile);
                return teamModel.Id;
            }
            if (otherContest != null && otherContest.Count() > 0)
            {
                var teamModel = new LongTermTeam();

                string iDate = "12/31/" + DateTime.Now.Year;
                DateTime oDate = Convert.ToDateTime(iDate);

                teamModel = new LongTermTeam
                {
                    UserId = requestDto.UserId,
                    CreatedDate = DateTime.Now,
                    IsDelete = false,
                    CreatedBy = requestDto.UserId,
                    UpdatedBy = requestDto.UserId,
                    UpdatedDate = DateTime.Now,
                    ExpiredDate = oDate

                };
                foreach (var team in otherContest)
                {
                    if (team.BullId > 0)
                    {
                        teamModel.LongTermTeamBull.Add(new LongTermTeamBull
                        {
                            BullId = team.BullId,
                            IsSubstitute = team.IsSubstitute,
                            BullTier = team.Tier,
                            TeamId = requestDto.TeamId
                        });
                    }
                    else
                    {
                        teamModel.LongTermTeamRider.Add(new LongTermTeamRider
                        {
                            RiderId = team.RiderId,
                            IsSubstitute = team.IsSubstitute,
                            RiderTier = team.Tier,
                            TeamId = requestDto.TeamId
                        });
                    }
                }

                await UpdateBrandIcon(teamModel, requestDto.BrandName, requestDto.IconFile);
                await _repoLongTermTeam.InsertGraphAsync(teamModel);
                return teamModel.Id;
            }

            return 0;
        }

        public async Task<int> CreateTeamApi(LongTermTeamFormationDto teamFormationDto)
        {

            if (teamFormationDto.TeamNumber == 1)
            {
                var teamModel = new LongTermTeam();
                string iDate = "12/31/" + DateTime.Now.Year;
                DateTime oDate = Convert.ToDateTime(iDate);

                teamModel = new LongTermTeam
                {
                    UserId = teamFormationDto.UserId,
                    CreatedDate = DateTime.Now,
                    IsDelete = false,
                    CreatedBy = teamFormationDto.UserId,
                    UpdatedBy = teamFormationDto.UserId,
                    UpdatedDate = DateTime.Now,
                    ExpiredDate = oDate
                };
                teamModel.LongTermTeamBull.Add(new LongTermTeamBull
                {
                    BullId = teamFormationDto.BullTier1,
                    BullTier = 1
                });
                teamModel.LongTermTeamBull.Add(new LongTermTeamBull
                {
                    BullId = teamFormationDto.BullTier2,
                    BullTier = 2
                });
                teamModel.LongTermTeamBull.Add(new LongTermTeamBull
                {
                    BullId = teamFormationDto.BullTier3,
                    BullTier = 3
                });
                foreach (var team in teamFormationDto.RiderTier1)
                {
                    teamModel.LongTermTeamRider.Add(new LongTermTeamRider
                    {
                        RiderId = team,
                        RiderTier = 1
                    });
                }
                foreach (var team in teamFormationDto.RiderTier2)
                {
                    teamModel.LongTermTeamRider.Add(new LongTermTeamRider
                    {
                        RiderId = team,
                        RiderTier = 2

                    });
                }
                foreach (var team in teamFormationDto.RiderTier3)
                {
                    teamModel.LongTermTeamRider.Add(new LongTermTeamRider
                    {
                        RiderId = team,
                        RiderTier = 3
                    });
                }
                await UpdateBrandIcon(teamModel, teamFormationDto.TeamBrand, teamFormationDto.Icon);
                await _repoLongTermTeam.InsertGraphAsync(teamModel);
                return teamModel.Id;
            }
            if (teamFormationDto.TeamNumber == 2)
            {
                var teamModel = new LongTermTeam();

                string iDate = "12/31/" + DateTime.Now.Year;
                DateTime oDate = Convert.ToDateTime(iDate);

                teamModel = new LongTermTeam
                {
                    UserId = teamFormationDto.UserId,
                    CreatedDate = DateTime.Now,
                    IsDelete = false,
                    CreatedBy = teamFormationDto.UserId,
                    UpdatedBy = teamFormationDto.UserId,
                    UpdatedDate = DateTime.Now,
                    ExpiredDate = oDate

                };
                foreach (var team in teamFormationDto.BullArray.Result1)
                {
                    teamModel.LongTermTeamBull.Add(new LongTermTeamBull
                    {
                        BullId = team.BullId
                    });
                }
                foreach (var team in teamFormationDto.BullArray.Result2)
                {
                    teamModel.LongTermTeamBull.Add(new LongTermTeamBull
                    {
                        BullId = team.BullId
                    });
                }
                foreach (var team in teamFormationDto.BullArray.Result3)
                {
                    teamModel.LongTermTeamBull.Add(new LongTermTeamBull
                    {
                        BullId = team.BullId
                    });
                }
                foreach (var team in teamFormationDto.RiderArray.Result1)
                {
                    teamModel.LongTermTeamRider.Add(new LongTermTeamRider
                    {
                        RiderId = team.RiderId
                    });
                }
                foreach (var team in teamFormationDto.RiderArray.Result2)
                {
                    teamModel.LongTermTeamRider.Add(new LongTermTeamRider
                    {
                        RiderId = team.RiderId
                    });
                }
                foreach (var team in teamFormationDto.RiderArray.Result3)
                {
                    teamModel.LongTermTeamRider.Add(new LongTermTeamRider
                    {
                        RiderId = team.RiderId
                    });
                }

                await UpdateBrandIcon(teamModel, teamFormationDto.TeamBrand, teamFormationDto.Icon);
                await _repoLongTermTeam.InsertGraphAsync(teamModel);
                return teamModel.Id;
            }

            return 0;
        }

        public async Task<int> CreateTeamApi(LongTermTeamFormationApiDto teamFormationDto)
        {
            if (teamFormationDto.TeamNumber == 1)
            {
                var teamModel = new LongTermTeam();
                string iDate = "12/31/" + DateTime.Now.Year;
                DateTime oDate = Convert.ToDateTime(iDate);

                teamModel = new LongTermTeam
                {
                    UserId = teamFormationDto.UserId,
                    CreatedDate = DateTime.Now,
                    IsDelete = false,
                    CreatedBy = teamFormationDto.UserId,
                    UpdatedBy = teamFormationDto.UserId,
                    UpdatedDate = DateTime.Now,
                    ExpiredDate = oDate
                };
                teamModel.LongTermTeamBull.Add(new LongTermTeamBull
                {
                    BullId = Convert.ToInt32(teamFormationDto.BullTier1),
                    BullTier = 1
                });
                teamModel.LongTermTeamBull.Add(new LongTermTeamBull
                {
                    BullId = Convert.ToInt32(teamFormationDto.BullTier2),
                    BullTier = 2
                });
                teamModel.LongTermTeamBull.Add(new LongTermTeamBull
                {
                    BullId = Convert.ToInt32(teamFormationDto.BullTier3),
                    BullTier = 3
                });
                foreach (var team in teamFormationDto.RiderTier1.Split(','))
                {
                    teamModel.LongTermTeamRider.Add(new LongTermTeamRider
                    {
                        RiderId = Convert.ToInt32(team),
                        RiderTier = 1
                    });
                }
                foreach (var team in teamFormationDto.RiderTier2.Split(','))
                {
                    teamModel.LongTermTeamRider.Add(new LongTermTeamRider
                    {
                        RiderId = Convert.ToInt32(team),
                        RiderTier = 2

                    });
                }
                foreach (var team in teamFormationDto.RiderTier3.Split(','))
                {
                    teamModel.LongTermTeamRider.Add(new LongTermTeamRider
                    {
                        RiderId = Convert.ToInt32(team),
                        RiderTier = 3
                    });
                }
                await UpdateBrandIcon(teamModel, teamFormationDto.TeamBrand, teamFormationDto.Icon);
                await _repoLongTermTeam.InsertGraphAsync(teamModel);
                return teamModel.Id;
            }
            if (teamFormationDto.TeamNumber == 2)
            {
                var teamModel = new LongTermTeam();

                string iDate = "12/31/" + DateTime.Now.Year;
                DateTime oDate = Convert.ToDateTime(iDate);

                teamModel = new LongTermTeam
                {
                    UserId = teamFormationDto.UserId,
                    CreatedDate = DateTime.Now,
                    IsDelete = false,
                    CreatedBy = teamFormationDto.UserId,
                    UpdatedBy = teamFormationDto.UserId,
                    UpdatedDate = DateTime.Now,
                    ExpiredDate = oDate

                };
                foreach (var team in teamFormationDto.BullArray.Result1)
                {
                    teamModel.LongTermTeamBull.Add(new LongTermTeamBull
                    {
                        BullId = team.BullId
                    });
                }
                foreach (var team in teamFormationDto.BullArray.Result2)
                {
                    teamModel.LongTermTeamBull.Add(new LongTermTeamBull
                    {
                        BullId = team.BullId
                    });
                }
                foreach (var team in teamFormationDto.BullArray.Result3)
                {
                    teamModel.LongTermTeamBull.Add(new LongTermTeamBull
                    {
                        BullId = team.BullId
                    });
                }
                foreach (var team in teamFormationDto.RiderArray.Result1)
                {
                    teamModel.LongTermTeamRider.Add(new LongTermTeamRider
                    {
                        RiderId = team.RiderId
                    });
                }
                foreach (var team in teamFormationDto.RiderArray.Result2)
                {
                    teamModel.LongTermTeamRider.Add(new LongTermTeamRider
                    {
                        RiderId = team.RiderId
                    });
                }
                foreach (var team in teamFormationDto.RiderArray.Result3)
                {
                    teamModel.LongTermTeamRider.Add(new LongTermTeamRider
                    {
                        RiderId = team.RiderId
                    });
                }

                await UpdateBrandIcon(teamModel, teamFormationDto.TeamBrand, teamFormationDto.Icon);
                await _repoLongTermTeam.InsertGraphAsync(teamModel);
                return teamModel.Id;
            }

            return 0;
        }

        int[] AllowedValues = new int[] { 1, 2, 3 };
        Random rand = new Random();
        static int prevTier = 0;
        private int GetTier()
        {
            prevTier = (prevTier + 1);
            if (prevTier > 3)
                prevTier = 1;

            return prevTier;
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
            await _repoLongTermTeam.DeleteAsync(TeamId);
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
            if (_repoLongTermTeam != null)
            {
                _repoLongTermTeam.Dispose();
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

        public Task<bool> IsTeamExist(string userId)
        {
            return Task.FromResult(_repoLongTermTeam.Query().Filter(r => r.UserId == userId).Get().SingleOrDefault() != null);
        }

        public async Task<LongTermTeamFormationLiteDto> LongTermTeamById(string userId)
        {
            LongTermTeamFormationLiteDto formationDto = new LongTermTeamFormationLiteDto();

            formationDto.BullArray = new DataResult<List<LTTeamBullFormationDto>, List<LTTeamBullFormationDto>, List<LTTeamBullFormationDto>>();
            formationDto.RiderArray = new DataResult<List<LTTeamRiderFormationDto>, List<LTTeamRiderFormationDto>, List<LTTeamRiderFormationDto>>();
            formationDto.BullArray.Result1 = new List<LTTeamBullFormationDto>();
            formationDto.BullArray.Result2 = new List<LTTeamBullFormationDto>();
            formationDto.BullArray.Result3 = new List<LTTeamBullFormationDto>();
            formationDto.RiderArray.Result1 = new List<LTTeamRiderFormationDto>();
            formationDto.RiderArray.Result2 = new List<LTTeamRiderFormationDto>();
            formationDto.RiderArray.Result3 = new List<LTTeamRiderFormationDto>();


            LongTermTeam existTeam = new LongTermTeam();
            existTeam = _repoLongTermTeam.Query().Filter(r => r.UserId == userId).Get().SingleOrDefault();


            existTeam = existTeam ?? new LongTermTeam();

            var teamId = existTeam.Id;
            formationDto.UserId = userId;
            formationDto.TeamAvatar = _appSettings.MainSiteURL + existTeam.TeamIcon;
            formationDto.TeamBrand = existTeam.TeamBrand;
            formationDto.TeamId = teamId;
            formationDto.BullList = new List<LTTeamBullFormationDto>();
            formationDto.RiderList = new List<LTTeamRiderFormationDto>();

            try
            {

                #region Bulls
                var teamBull = _repoTeamBull.Query().Get();
                   

                   

                var addedbullsIds = teamBull.Where(t => t.TeamId == teamId).Select(n => n.BullId);
                var bpredicate = PredicateBuilder.True<Bull>()
                                .And(x => x.BullId > 0 && addedbullsIds.Contains(x.Id));

                var bulls = _repoBull.Query()
                         .Filter(bpredicate).Get();

                var Teambulls = bulls.Select(bl => new LTTeamBullFormationDto
                      {
                          TeamId = existTeam.Id,
                          IsSelected = true,
                          BullId = bl.Id,
                          BullName = bl.Name,
                          BullTier = teamBull.FirstOrDefault(x => x.BullId == bl.Id && x.TeamId == existTeam.Id)?.BullTier,
                          BullAvatar = (_bullRiderPictures.GetBullPic(bl.Id, _appSettings.MainSiteURL)).Result
                }).ToList();

                    //formationDto.BullList = Teambulls.Item1;
                    //for bull list
                    foreach (var item in Teambulls)
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

                #endregion

                #region Riders

                var teamRider = _repoTeamRider.Query().Get();
                    

                    
                    var addedRidersIds = teamRider.Where(t => t.TeamId == teamId).Select(n => n.RiderId);
                    var rpredicate = PredicateBuilder.True<Rider>()
                            .And(x => x.RiderId > 0 && addedRidersIds.Contains(x.Id));
                    var riders = _repoRider.Query().Filter(rpredicate).Get();

                var Teamriders = riders.Select(bl => new LTTeamRiderFormationDto
                     {
                         TeamId = existTeam.Id,
                         IsSelected = true,
                         RiderId = bl.Id,
                         RiderName = bl.Name,
                         RiderTier = teamRider.FirstOrDefault(x => x.RiderId == bl.Id && x.TeamId == existTeam.Id)?.RiderTier,
                         RiderAvatar = (_bullRiderPictures.GetRiderPic(bl.Id, _appSettings.MainSiteURL)).Result
                     }).ToList();



                    //for rider list
                    foreach (var item in Teamriders)
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

                #endregion


                var bullsList = _repoBull.Query().Get().Select(r => getBullDto(r, Teambulls)).ToList();
                var riderList = _repoRider.Query().Get().Select(r => GetRiderDTO(r, Teamriders)).ToList();


                formationDto.BullList = bullsList;
                formationDto.RiderList = riderList;

            }
            catch (Exception ex)
            {

            }

            return await Task.FromResult(formationDto);
        }

        private LTTeamBullFormationDto getBullDto(Bull bull, IEnumerable<LTTeamBullFormationDto> teamBulls)
        {
            var teamBull = teamBulls.Where(x => x.BullId == bull.BullId).SingleOrDefault();
            if (teamBull != null)
            {
                return new LTTeamBullFormationDto
                {
                    IsSelected = true,
                    BullId = bull.BullId,
                    BullName = bull.Name,
                    TeamId = teamBull.TeamId,
                    BullTier = teamBull.BullTier,
                    BullAvatar = (_bullRiderPictures.GetBullPic(bull.Id, _appSettings.MainSiteURL)).Result
                };
            }
            else
            {
                return new LTTeamBullFormationDto
                {
                    IsSelected = false,
                    BullId = bull.BullId,
                    BullName = bull.Name,
                    BullTier = GetTier(),
                    BullAvatar = (_bullRiderPictures.GetBullPic(bull.Id, _appSettings.MainSiteURL)).Result
                };
            }


        }

        private LTTeamRiderFormationDto GetRiderDTO(Rider rider, IEnumerable<LTTeamRiderFormationDto> teamRiders)
        {

            var teamRider = teamRiders.Where(x => x.RiderId == rider.RiderId).SingleOrDefault();
            if (teamRider != null)
            {
                return new LTTeamRiderFormationDto
                {
                    IsSelected = true,
                    RiderId = rider.RiderId,
                    RiderName = rider.Name,
                    TeamId = teamRider.TeamId,
                    RiderTier = teamRider.RiderTier,
                    RiderAvatar = (_bullRiderPictures.GetRiderPic(rider.Id, _appSettings.MainSiteURL)).Result

                };
            }
            else
            {
                return new LTTeamRiderFormationDto
                {
                    IsSelected = false,
                    RiderId = rider.RiderId,
                    RiderName = rider.Name,
                    RiderTier = GetTier(),
                    RiderAvatar = (_bullRiderPictures.GetRiderPic(rider.Id, _appSettings.MainSiteURL)).Result
                };
            }
        }


       
    }
}
