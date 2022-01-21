using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RR.Core;
using RR.Dto;
using RR.Service;
using RR.Service.User;

namespace RR.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyAccountController : BaseController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IContestService _contestService;
        private readonly ITeamService _teamService;
        private readonly IFavoriteBullsRidersService _favoriteBullsRiders;
        private readonly IUserService _userService;
        private readonly IContestUserWinnerService _contestUWService;

        public MyAccountController(
           IContestService contestService,
           ITeamService teamService,
           IFavoriteBullsRidersService favoriteBullsRiders,
           IUserService userService,
           IContestUserWinnerService contestUWService,
           IOptions<AppSettings> appSettings) :
            base(appSettings)
        {

            _contestService = contestService;
            _teamService = teamService;
            _favoriteBullsRiders = favoriteBullsRiders;
            _userService = userService;
            _contestUWService = contestUWService;
        }


        /// <summary>
        /// Get User Contest History
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns>User contest</returns>
        [HttpGet]
        [Route("contesthistory/{userid}")]
        public async Task<OkObjectResult> GetContestHistory(string userid)
        {
            var data = await _contestService.GetContestOfCurrentUser(userid);
            return Ok(new ApiResponse
            {
                Data = data,
                IpAddress = Helpers.GetIpAddress(),
                Success = true,
                Message = ""
            });
        }

        /// <summary>
        /// Get user contest detail
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="contestId"></param>
        /// <param name="teamId"></param>
        /// <returns>User contest details</returns>
        [HttpPost]
        [Route("usercontestdetail")]
        public async Task<OkObjectResult> GetUserContestDetail([FromForm] string userId, [FromForm]int contestId, [FromForm]int teamId)
        {
            try
            {
                var userInfo = await _userService.GetUserDetail(userId);
                var contestDetail = await _contestService.GetContestById(contestId);

                var joinedContestDetailOfUser = await _contestService.GetContestDetailOfCurrentUser(contestDetail.EventId, contestId, teamId);
                var UserContest = new UserContestDetailDto();
                UserContest.Username = userInfo.UserName;
                UserContest.Title = contestDetail.Title;
                UserContest.JoiningFee = contestDetail.JoiningFee;

                var tiers = joinedContestDetailOfUser.Item1.Select(r => r.RiderTier).Distinct();
                for (int i = 0; i < tiers.Count(); i++)
                {
                    var tier_ = (i + 1);
                    UserContest.Tiers.Add(new ContestTiers
                    {
                        Title = "TIER " + tier_,
                        Riders = joinedContestDetailOfUser.Item1.Where(d => d.RiderTier == tier_).Select(r => r),
                        Bulls = joinedContestDetailOfUser.Item2.Where(d => d.BullTier == tier_).Select(r => r),
                    });
                }

                return Ok(new ApiResponse
                {
                    Data = UserContest,
                    IpAddress = Helpers.GetIpAddress(),
                    Success = true,
                    Message = ""
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse
                {
                    Data = null,
                    IpAddress = Helpers.GetIpAddress(),
                    Success = false,
                    Message = "Oops! something wrong"
                });

            }

        }


        /// <summary>
        /// Get year end standing
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>User contest</returns>
        [HttpGet]
        [Route("yearendstanding/{userId}")]
        public async Task<OkObjectResult> GetYearEndStanding(string userId)
        {
            try
            {
                var yearStandings = await _teamService.GetStandings(false, 0, 5000, 0, "", "");
                var userStanding = yearStandings.Where(d => d.UserId == userId);
                return Ok(new ApiResponse
                {
                    Data = new { mystanding = userStanding, yearendstanding = yearStandings },
                    IpAddress = Helpers.GetIpAddress(),
                    Success = true,
                    Message = ""
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse
                {
                    Data = null,
                    IpAddress = Helpers.GetIpAddress(),
                    Success = false,
                    Message = "Oops! something wrong"
                });

            }

        }

        /// <summary>
        /// Get favorite bull/riders
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>User contest</returns>
        [HttpGet]
        [Route("favoritebullriders/{userId}")]
        public async Task<OkObjectResult> GetFavoriteBullRiders(string userId)
        {
            try
            {
                var data = await _favoriteBullsRiders.GetUserFavoriteBullsRiders(userId);
                return Ok(new ApiResponse
                {
                    Data = data,
                    IpAddress = Helpers.GetIpAddress(),
                    Success = true,
                    Message = ""
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse
                {
                    Data = null,
                    IpAddress = Helpers.GetIpAddress(),
                    Success = false,
                    Message = "Oops! something wrong"
                });

            }

        }

        /// <summary>
        /// Add favorite rider 
        /// </summary>
        /// <param name="riderId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addfavoriterider")]
        public async Task<OkObjectResult> AddFavoriteRider([FromForm] int riderId, [FromForm]string userId)
        {

            if (string.IsNullOrEmpty(userId) || riderId == 0)
                return Ok(new ApiResponse
                {
                    Data = null,
                    IpAddress = Helpers.GetIpAddress(),
                    Success = false,
                    Message = "Warning!! riderId and userid requried"
                });

            try
            {
                await _favoriteBullsRiders.AddRiderAsFavorite(new FavoriteBullRidersDto
                {
                    UserId = userId,
                    RiderId = riderId
                });
                return Ok(new ApiResponse
                {
                    IpAddress = Helpers.GetIpAddress(),
                    Success = true,
                    Message = "Rider successfully added as favorite"
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse
                {
                    IpAddress = Helpers.GetIpAddress(),
                    Success = true,
                    Message = "Oops!! something wrong"
                });

            }

        }

        /// <summary>
        /// Add favorite bull
        /// </summary>
        /// <param name="bullId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addfavoritebull")]
        public async Task<OkObjectResult> AddFavoriteBull([FromForm]int bullId, [FromForm]string userId)
        {

            if (string.IsNullOrEmpty(userId) || bullId == 0)
                return Ok(new ApiResponse
                {
                    Data = null,
                    IpAddress = Helpers.GetIpAddress(),
                    Success = false,
                    Message = "Warning!! bullId and userId requried"
                });

            try
            {
                await _favoriteBullsRiders.AddBullAsFavorite(new FavoriteBullRidersDto
                {
                    UserId = userId,
                    BullId = bullId
                });
                return Ok(new ApiResponse
                {
                    IpAddress = Helpers.GetIpAddress(),
                    Success = true,
                    Message = "Bull successfully added as favorite"
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse
                {
                    IpAddress = Helpers.GetIpAddress(),
                    Success = true,
                    Message = "Oops!! something wrong"
                });

            }

        }

        /// <summary>
        /// remove favorite rider
        /// </summary>
        /// <param name="riderId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("removefavoriterider")]
        public async Task<OkObjectResult> RemoveFavoriteRider([FromForm]int riderId, [FromForm]string userId)
        {

            if (string.IsNullOrEmpty(userId) || riderId == 0)
                return Ok(new ApiResponse
                {
                    Data = null,
                    IpAddress = Helpers.GetIpAddress(),
                    Success = false,
                    Message = "Warning!! riderId and userid requried"
                });

            try
            {
                await _favoriteBullsRiders.RemoveUserFavoriteRider(userId, riderId);
                return Ok(new ApiResponse
                {
                    IpAddress = Helpers.GetIpAddress(),
                    Success = true,
                    Message = "Rider successfully removed from favorite"
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse
                {
                    IpAddress = Helpers.GetIpAddress(),
                    Success = true,
                    Message = "Oops!! something wrong"
                });

            }

        }

        /// <summary>
        /// remove favorite bull
        /// </summary>
        /// <param name="bullId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("removefavoritebull")]
        public async Task<OkObjectResult> RemoveFavoriteBull([FromForm]int bullId, [FromForm]string userId)
        {

            if (string.IsNullOrEmpty(userId) || bullId == 0)
                return Ok(new ApiResponse
                {
                    Data = null,
                    IpAddress = Helpers.GetIpAddress(),
                    Success = false,
                    Message = "Warning!! bullId and userId requried"
                });

            try
            {
                await _favoriteBullsRiders.RemoveUserFavoriteBulls(userId, bullId);
                return Ok(new ApiResponse
                {
                    IpAddress = Helpers.GetIpAddress(),
                    Success = true,
                    Message = "Bull successfully removed from favorite"
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse
                {
                    IpAddress = Helpers.GetIpAddress(),
                    Success = true,
                    Message = "Oops!! something wrong"
                });

            }

        }


        [HttpGet]
        [Route("userawards/{userId}")]
        public async Task<OkObjectResult> GetUserAwards(string userId)
        {
            
            if (string.IsNullOrEmpty(userId))
                return Ok(new ApiResponse
                {
                    Data = null,
                    IpAddress = Helpers.GetIpAddress(),
                    Success = false,
                    Message = "Warning!! userId requried"
                });

            try
            {
                var awardsDetail = await _contestUWService.GetUserWinnings(userId);
                
                return Ok(new ApiResponse
                {
                    Data = awardsDetail,
                    IpAddress = Helpers.GetIpAddress(),
                    Success = true,
                    Message = awardsDetail?.Count > 0 ?"" : "Oops!!! You didn't win any contest yes. join more contest for wining prizes"
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse
                {
                    IpAddress = Helpers.GetIpAddress(),
                    Success = true,
                    Message = "Oops!! something wrong"
                });

            }

        }
        
    }
}