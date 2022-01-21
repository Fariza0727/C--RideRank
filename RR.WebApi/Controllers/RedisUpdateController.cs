using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RR.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisUpdateController : Controller
    {
        #region Constructor

        private readonly ITeamService _teamService;
        private readonly IBullService _bullService;
        private readonly IRiderService _riderService;

        public RedisUpdateController(ITeamService teamService, IBullService bullService, IRiderService riderService)
        {
            _teamService = teamService;
            _bullService = bullService;
            _riderService = riderService;
        }

        #endregion
        [HttpGet]
        [Route("update-cache")]
        public string UpdateCache()
        {
            Task.Run(() => _teamService.UpdateRedisCache());
            Task.Run(() => _bullService.UpdateRedisCache());
            Task.Run(() => _riderService.UpdateRedisCache());
            
            return "Updated Successfully";
        }
    }
}
