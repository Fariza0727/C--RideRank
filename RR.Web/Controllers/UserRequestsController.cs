using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RR.Dto;
using RR.Service;

namespace RR.Web.Controllers
{
    [Authorize(Roles = "TM, PTM,FTM,NTM")]
    public class UserRequestsController : Controller
    {
        private readonly IUserRequestsServices _requestsServices;
        private readonly ILongTermTeamService _longTermTeamService;

        public UserRequestsController(IUserRequestsServices requestsServices, ILongTermTeamService longTermTeamService)
        {
            _requestsServices = requestsServices;
            _longTermTeamService = longTermTeamService;
        }

        [HttpPost]
        [Route("send-request-longtermteamupdate/{teamId}")]
        public async Task<JsonResult> SendRequestLongTermTeamUpdate(int teamId)
        {
            var team = await _longTermTeamService.GetTeam(teamId);
            if (team !=null && team.Id > 0)
            {
                var AspNetUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var request_ = await _requestsServices.SentRequest(new UserRequestsDto
                {
                    CreatedBy = AspNetUserId,
                    CreatedDate = DateTime.Now,
                    RequestMessage = $"You have received a request for ( <b> {team.TeamBrand} </b> ) team updation.",
                    Message = $"Your request has been in under reviewed.",
                    Title = "Team update request",
                    UserId = AspNetUserId,
                    LongTermTeamId = teamId
                });

                return Json(new { status = true });
            }

            return Json(new { status = false, message = "Invalid team" });
        }

    }
}