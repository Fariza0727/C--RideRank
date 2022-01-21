using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RR.Dto;
using RR.Dto.Team;
using RR.Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : Controller
    {
        #region Constructor

        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITeamService _teamService;
        private readonly ILongTermTeamService _longtermteamService;
        private readonly IContestService _contestService;

        public TeamController(UserManager<IdentityUser> userManager,
             ITeamService teamService,
             IContestService contestService,
             ILongTermTeamService longtermteamService)
        {
            _userManager = userManager;
            _teamService = teamService;
            _contestService = contestService;
            _longtermteamService = longtermteamService;
        }

        #endregion

        /// <summary>
        /// Team Formation Index Page
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("team-formation")]
        public async Task<OkObjectResult> TeamFormation([FromForm] string userId, [FromForm] int contestId, [FromForm] int eventId = 0)
        {
            TeamFormationDto formationDto = new TeamFormationDto();
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                var joinedContest = await _contestService.GetJoinedContestListByEventId(eventId, user != null ? user.Id : "");

                formationDto = await _teamService.EventPlayersByIdApi(eventId, contestId, 0);
                formationDto.IsAlreadyJoined = joinedContest.Contains(contestId) ? true : false;
                return Ok(new ApiResponse
                {
                    Data = formationDto,
                    IpAddress = Helpers.GetIpAddress(),
                    Success = true,
                    Message = ""
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse
                {
                    Message = ex.Message,
                    IpAddress = Helpers.GetIpAddress(),
                    Success = false
                });
            }
        }

        /// <summary>
        /// Create Team of Player
        /// </summary>
        /// <param name="teamData">The Team data</param>
        /// <param name="eventId">Event Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("createteam")]
        public async Task<OkObjectResult> CreateTeam([FromForm] TeamFormationDto teamFormationDto)
        {
            try
            {
                int teamId = await _teamService.CreateTeamApi(teamFormationDto);

                return Ok(new ApiResponse
                {
                    Data = teamId,
                    Message = "Team Created Successfully",
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse
                {
                    Message = ex.Message,
                    Success = false
                });
            }
        }


        /// <summary>
        /// Create long term team
        /// </summary>
        /// <param name="teamData">The Team data</param>
        /// <param name="eventId">Event Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("createlongtermteam")]
        public async Task<OkObjectResult> CreateLongTermTeam([FromForm] LongTermTeamFormationApiDto teamFormationDto)
        {
            try
            {
                bool isExist = await _longtermteamService.IsTeamExist(teamFormationDto.UserId);
                if (isExist)
                {
                    return Ok(new ApiResponse
                    {
                        Message = "you have already created a team!",
                        Success = false,
                        IpAddress = Helpers.GetIpAddress(),
                    });
                }

                var file = HttpContext.Request.Form;
                if (file.Files.Count > 0)
                {
                    teamFormationDto.Icon = file.Files[0];
                }

                int teamId = await _longtermteamService.CreateTeamApi(teamFormationDto);

                return Ok(new ApiResponse
                {
                    Data = teamId,
                    Message = "Team Created Successfully",
                    Success = true,
                    IpAddress = Helpers.GetIpAddress(),
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse
                {
                    Message = ex.Message,
                    Success = false,
                    IpAddress = Helpers.GetIpAddress(),
                });
            }
        }

        /// <summary>
        /// get long term team
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="OnlyBull"></param>
        /// <param name="OnlyRiders"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getlongtermteam/{UserId}")]
        public async Task<OkObjectResult> GetLongTermTeam(string UserId)
        {
            try
            {
                var data = await _longtermteamService.LongTermTeamById(UserId);
                return Ok(new ApiResponse
                {
                    Data = data,
                    Success = true,
                    IpAddress = Helpers.GetIpAddress(),
                    Message = ""
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse
                {
                    Message = ex.Message,
                    Success = false
                });
            }
        }

        [HttpGet]
        [Route("yearStandings")]
        public async Task<OkObjectResult> YearStandings()
        {
            var yearStandings = await _teamService.GetStandings();
            return Ok(new ApiResponse
            {
                Data = yearStandings,
                Success = true,
                IpAddress = Helpers.GetIpAddress(),
                Message = ""
            });
        }
    }
}