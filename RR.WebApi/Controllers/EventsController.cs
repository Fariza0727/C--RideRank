using Microsoft.AspNetCore.Mvc;
using RR.Dto;
using RR.Service;
using System.Threading.Tasks;

namespace RR.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : Controller
    {
        #region Constructor

        private readonly IEventService _eventService;
        private readonly ITeamService _teamService;

        public EventsController(IEventService eventService,
             ITeamService teamService)
        {
            _eventService = eventService;
            _teamService = teamService;
        }

        #endregion

        [HttpGet]
        [Route("events")]
        public async Task<OkObjectResult> GetEvents()
        {
            var eventResults = await _eventService.GetAllEvents();
            return Ok(new ApiResponse
            {
                Data = eventResults,
                IpAddress = Helpers.GetIpAddress(),
                Success = true,
                Message = ""
            });
        }

        [HttpGet]
        [Route("playerStandings")]
        public async Task<OkObjectResult> GetStandings()
        {
            var playerStandings = await _teamService.GetStandingsApi(true);
            return Ok(new ApiResponse
            {
                Data = playerStandings,
                IpAddress = Helpers.GetIpAddress(),
                Success = true,
                Message = ""
            });
        }

        /// <summary>
        /// This method is used for getting all riders associated 
        /// with bulls for specific event
        /// </summary>
        /// <param name="id">An Id of event</param>
        /// <returns>Event Draw View</returns>
        [HttpGet]
        [Route("event-draw/{id}")]
        public async Task<OkObjectResult> EventDraw(int id)
        {
            try
            {
                var eventDrawData = await _eventService.GetEventDrawById(id);
                return Ok(new ApiResponse
                {
                    Data = eventDrawData,
                    IpAddress = Helpers.GetIpAddress(),
                    Success = true,
                    Message = ""
                });
            }
            catch (System.Exception ex)
            {
                return Ok(new ApiResponse
                {
                    Message = ex.Message,
                    IpAddress = Helpers.GetIpAddress(),
                    Success = false
                });
            }

        }

        [HttpGet]
        [Route("getyearendstandings")]
        public async Task<OkObjectResult> GetYearEndStandings()
        {
            var newsResult = await _teamService.GetPlayerPointsOfEvent(true);
            return Ok(new ApiResponse
            {
                Data = newsResult,
                IpAddress = Helpers.GetIpAddress(),
                Success = true,
                Message = ""
            });
        }

        [HttpGet]
        [Route("getlasteventstandings")]
        public async Task<OkObjectResult> GetLastEventStandings()
        {
            var newsResult = await _teamService.GetLastEventStatndingPlayerPoints();
            return Ok(new ApiResponse
            {
                Data = newsResult,
                IpAddress = Helpers.GetIpAddress(),
                Success = true,
                Message = ""
            });
        }

    }
}